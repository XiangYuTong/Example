/*
* @Author: 小羊
* @Description:Udp信号接收
* @Date: 2023年04月18日 星期二 20:04:11
* @Modify:
*/

using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
using System;

public class UdpReceiver : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    private string message;
    private Thread t;
    private bool isReceive = false;
    private object lockObject = new object();
    void Start()
    {
        udpClient = new UdpClient(Common.udpport);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0); // 0 表示任意端口
        // 创建并启动线程
        t = new Thread(new ThreadStart(ReceiveUDP));
        t.Start();
    }
    private void FixedUpdate()
    {
        if (isReceive)
        {
            lock (lockObject)
            {
                isReceive = false;

            }

        }
     
    }
    /// <summary>
    /// 接收信号
    /// </summary>
    private void ReceiveUDP()
    {
        while (true)
        {
            message = System.Text.Encoding.UTF8.GetString(udpClient.Receive(ref remoteEndPoint));
            Debug.Log("Received message: " + message + " from " + remoteEndPoint);
            isReceive = true;
        }
    }

    void OnApplicationQuit()
    {
        if (this.t != null)
        {
            this.t.Abort();
        }

        if (this.udpClient != null)
        {
            this.udpClient.Close();
        }

        //udpClient.Close();
    }


}