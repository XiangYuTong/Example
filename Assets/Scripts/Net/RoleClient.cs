using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


public class RoleClient
{
    Timer timer;
    public RoleClient(Socket socket, List<RoleClient> otherClients)
    {
        m_OtherClients = otherClients;
        m_ReceiveBuffer = new byte[1024 * 512];
        m_ReceiveStream = new MemoryStream();
        m_ReceiveQueue = new Queue<byte[]>();
        m_SendQueue = new Queue<byte[]>();
        m_Socket = socket;
        m_IsConnected = true;
        //m_ReceiveThread = new Thread(CheckReceive);
        timer = new Timer(CheckReceiveBuffer, 0, 0, 200);
        //m_ReceiveThread.Start();
        StartReceive();
    }

    public void Send(ushort msgCode, byte[] buffer)
    {
        byte[] sendMsgBuffer = null;

        using (MemoryStream ms = new MemoryStream())
        {
            int msgLen = buffer.Length;
            byte[] lenBuffer = BitConverter.GetBytes((ushort)msgLen);
            byte[] msgCodeBuffer = BitConverter.GetBytes(msgCode);
            ms.Write(lenBuffer, 0, lenBuffer.Length);
            ms.Write(msgCodeBuffer, 0, msgCodeBuffer.Length);
            ms.Write(buffer, 0, msgLen);
            sendMsgBuffer = ms.ToArray();
        }

        lock (m_SendQueue)
        {
            m_SendQueue.Enqueue(sendMsgBuffer);
            CheckSendBuffer();
        }
    }

    public void Close(bool isForce = false)
    {
        try { m_Socket.Shutdown(SocketShutdown.Both); }
        catch { }

        if (isForce)
        {
            IPEndPoint endPoint = m_Socket.RemoteEndPoint as IPEndPoint;
            Debug.Log("强制关闭与客户端：" + endPoint.Address.ToString() + "的连接");
        }

        m_IsConnected = false;
        m_Socket.Close();
        m_ReceiveStream.SetLength(0);
        m_ReceiveQueue.Clear();
        m_SendQueue.Clear();
        timer.Dispose();

        if (m_OtherClients != null)
        {
            m_OtherClients.Remove(this);
        }

        timer = null;
        m_SendQueue = null;
        m_ReceiveQueue = null;
        m_ReceiveStream = null;
        m_ReceiveBuffer = null;
        m_OtherClients = null;
    }

    private void StartReceive()
    {
        if (!m_IsConnected) return;
        m_Socket.BeginReceive(m_ReceiveBuffer, 0, m_ReceiveBuffer.Length, SocketFlags.None, OnReceive, m_Socket);
    }

    private void OnReceive(IAsyncResult ir)
    {
        if (!m_IsConnected) return;
        try
        {
            int length = m_Socket.EndReceive(ir);
            if (length < 1)
            {
                IPEndPoint endPoint = m_Socket.RemoteEndPoint as IPEndPoint;
                Debug.Log("客户端：" + endPoint.Address.ToString() + "已断开连接");
                Close();
                return;
            }

            m_ReceiveStream.Position = m_ReceiveStream.Length;
            m_ReceiveStream.Write(m_ReceiveBuffer, 0, length);

            if (m_ReceiveStream.Length < 3)
            {
                StartReceive();
                return;
            }

            while (true)
            {
                m_ReceiveStream.Position = 0;
                byte[] msgLenBuffer = new byte[2];
                m_ReceiveStream.Read(msgLenBuffer, 0, 2);
                int msgLen = BitConverter.ToUInt16(msgLenBuffer, 0) + 2;
                int fullLen = 2 + msgLen;

                if (m_ReceiveStream.Length < fullLen)
                {
                    break;
                }

                byte[] msgBuffer = new byte[msgLen];
                m_ReceiveStream.Position = 2;
                m_ReceiveStream.Read(msgBuffer, 0, msgLen);

                lock (m_ReceiveQueue)
                {
                    m_ReceiveQueue.Enqueue(msgBuffer);
                }

                int remainLen = (int)m_ReceiveStream.Length - fullLen;

                if (remainLen < 1)
                {
                    m_ReceiveStream.Position = 0;
                    m_ReceiveStream.SetLength(0);
                    break;
                }

                m_ReceiveStream.Position = fullLen;
                byte[] remainBuffer = new byte[remainLen];
                m_ReceiveStream.Read(remainBuffer, 0, remainLen);
                m_ReceiveStream.Position = 0;
                m_ReceiveStream.SetLength(0);
                m_ReceiveStream.Write(remainBuffer, 0, remainLen);
                remainBuffer = null;
            }
        }
        catch
        {
            IPEndPoint endPoint = m_Socket.RemoteEndPoint as IPEndPoint;
            Debug.Log("客户端：" + endPoint.Address.ToString() + "已断开连接");
            Close();
            return;
        }

        StartReceive();
    }

    private void CheckSendBuffer()
    {
        lock (m_SendQueue)
        {
            if (m_SendQueue.Count > 0)
            {
                byte[] buffer = m_SendQueue.Dequeue();
                m_Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, m_Socket);
            }
        }
    }

    private void CheckReceiveBuffer(object state)
    {
        lock (m_ReceiveQueue)
        {
            if (m_ReceiveQueue.Count < 1) return;
            byte[] buffer = m_ReceiveQueue.Dequeue();
            byte[] msgContent = new byte[buffer.Length - 2];
            ushort msgCode = 0;

            using (MemoryStream ms = new MemoryStream(buffer))
            {
                byte[] msgCodeBuffer = new byte[2];
                ms.Read(msgCodeBuffer, 0, msgCodeBuffer.Length);
                msgCode = BitConverter.ToUInt16(msgCodeBuffer, 0);
                ms.Read(msgContent, 0, msgContent.Length);
            }

           Debug.Log("消息编号：" + msgCode + ",内容：" + Encoding.UTF8.GetString(msgContent));
        }
    }

    private void SendCallback(IAsyncResult ir)
    {
        m_Socket.EndSend(ir);
        CheckSendBuffer();
    }

    private bool m_IsConnected = false;
    private Queue<byte[]> m_ReceiveQueue = null;
    private Queue<byte[]> m_SendQueue = null;
    private MemoryStream m_ReceiveStream = null;
    private byte[] m_ReceiveBuffer = null;
    private Socket m_Socket = null;
    private List<RoleClient> m_OtherClients = null;
}
