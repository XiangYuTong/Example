using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

public class TcpServerMgr : MonoBehaviour
{
    private static TcpServerMgr _instance;
    private static Socket m_Socket;
    private static Thread m_AcceptThread;
    private static List<RoleClient> m_Clients;

    public static TcpServerMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                TcpServerMgr ins = FindObjectOfType<TcpServerMgr>();
                if (ins == null)
                {
                    Debug.LogError("场景中没有TcpServerMgr组件，已经自动生成");
                    GameObject go = new GameObject(nameof(TcpServerMgr));
                    go.transform.parent = GameManager.instance.transform;
                    _instance = go.AddComponent<TcpServerMgr>();
                }
                else
                {
                    _instance = ins;
                }
            }
            return _instance;
        }
    }
    public void Init()
    {
        m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_AcceptThread = new Thread(OnAccept);//另起一个线程进入阻塞状态等待客户端连接
        m_Clients = new List<RoleClient>();//所有连接的客户端
        m_Socket.Bind(new IPEndPoint(IPAddress.Parse(Common.tcpip), Common.tcpport)) ;//绑定ip端口
        m_Socket.Listen(50);//最大监听数50
        m_AcceptThread.Start();//进入等待连接状态
        AppDomain.CurrentDomain.ProcessExit += OnApplicatonQuit;
        Debug.Log("服务器启动成功！");
        Debug.Log("监听IP：" + Common.tcpip.ToString() + "，端口：" + Common.tcpport.ToString());
        //广播代码
    }
    private static void OnApplicatonQuit(object sender, EventArgs e)
    {
        for (int i = m_Clients.Count - 1; i > -1; i--)
        {
            m_Clients[i].Close();
        }

        m_AcceptThread.Abort();
        m_Clients.Clear();
    }
    static void OnAccept()
    {
        while (true)
        {
            try
            {
                Socket client = m_Socket.Accept();//尝试接收一个客户端的连接
                IPEndPoint clientPoint = client.RemoteEndPoint as IPEndPoint;
                m_Clients.Add(new RoleClient(client, m_Clients));//为客户端建立一个请求处理实例，并加入到表中
                Debug.Log("客户端:" + clientPoint.Address.ToString() + "已经连接！");
            }
            catch
            {
                continue;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
