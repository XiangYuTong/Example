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
                    Debug.LogError("������û��TcpServerMgr������Ѿ��Զ�����");
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
        m_AcceptThread = new Thread(OnAccept);//����һ���߳̽�������״̬�ȴ��ͻ�������
        m_Clients = new List<RoleClient>();//�������ӵĿͻ���
        m_Socket.Bind(new IPEndPoint(IPAddress.Parse(Common.tcpip), Common.tcpport)) ;//��ip�˿�
        m_Socket.Listen(50);//��������50
        m_AcceptThread.Start();//����ȴ�����״̬
        AppDomain.CurrentDomain.ProcessExit += OnApplicatonQuit;
        Debug.Log("�����������ɹ���");
        Debug.Log("����IP��" + Common.tcpip.ToString() + "���˿ڣ�" + Common.tcpport.ToString());
        //�㲥����
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
                Socket client = m_Socket.Accept();//���Խ���һ���ͻ��˵�����
                IPEndPoint clientPoint = client.RemoteEndPoint as IPEndPoint;
                m_Clients.Add(new RoleClient(client, m_Clients));//Ϊ�ͻ��˽���һ��������ʵ���������뵽����
                Debug.Log("�ͻ���:" + clientPoint.Address.ToString() + "�Ѿ����ӣ�");
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
