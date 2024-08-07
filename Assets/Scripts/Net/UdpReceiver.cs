using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System;
using System.Collections;
using QFramework;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Threading;

public class UdpReceiver : MonoBehaviour
{
    private static UdpReceiver _instance;
    public static UdpReceiver Instance
    {
        get
        {
            if (_instance == null)
            {
                UdpReceiver ins = FindObjectOfType<UdpReceiver>();
                if (ins == null)
                {
                    Debug.LogError("场景中没有UdpReceiver组件，已经自动生成");
                    GameObject go = new GameObject(nameof(UdpReceiver));
                    go.transform.parent = GameManager.instance.transform;
                    _instance = go.AddComponent<UdpReceiver>();
                }
                else
                {
                    _instance = ins;
                }
            }
            return _instance;
        }
    }
    private UdpClient udpClient;
    private Socket serverSocket;
    private static int udpPort = 8886;
    private bool openReceive = true;
    private bool isOpenReceive = false;
    private EndPoint epSender;
    private IPEndPoint remoteEndPoint;
    private Thread t;
    private object lockObject = new object();
    public string receiveString = "";
    private byte[] ReceiveData = new byte[1024];
    [Header("Udp数据接收回调事件")]
    public CallBack<string> OnUdpMessageHandle;
    public bool isUdpAsyn;
    public void Init(bool arg)
    {
        isUdpAsyn = arg;
        udpPort = Common.udpport;
        if (isUdpAsyn)
        {
            UDPCreate();
            OnUdpMessageHandle += GameManager.instance.UdpMessageHandle;
            EventCenter.AddListener<string>(EventTypes.UdpMessageHandle, OnUdpMessageHandle);
        }
        else
        {
            udpClient = new UdpClient(udpPort);
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 0); // 0 表示任意端口                                                             // 创建并启动线程
            t = new Thread(new ThreadStart(ReceiveUDP));
            t.Start();
        }
    }


    public void UDPCreate()
    {
        StartCoroutine(ToBeginSocket0());
    }
    /// <summary>
    /// 初始化Socket的协程
    /// </summary>
    /// <returns></returns>
    IEnumerator ToBeginSocket0()
    {
        yield return new WaitForSeconds(0.1f);
        ToBeginSocket();
    }
    /// <summary>
    /// 初始化Socket
    /// </summary>
    void ToBeginSocket()
    {
        if (openReceive)
        {
            //服务器Socket对实例化
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //Socket对象服务器的IP和端口固定
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, udpPort));
            //监听的端口和地址
            epSender = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
            //开始异步接收数据
            serverSocket.BeginReceiveFrom(ReceiveData, 0, ReceiveData.Length, SocketFlags.None, ref epSender, new AsyncCallback(ReceiveFromClients), epSender);
        }
    }
    /// <summary>
    /// 异步加载，处理数据
    /// </summary>
    /// <param name="iar"></param>
    void ReceiveFromClients(IAsyncResult iar)
    {
        int reve = serverSocket.EndReceiveFrom(iar, ref epSender);
        //数据处理
        string str = System.Text.Encoding.UTF8.GetString(ReceiveData, 0, reve);
        //把得到的数据传给数据处理中心
        serverSocket.BeginReceiveFrom(ReceiveData, 0, ReceiveData.Length, SocketFlags.None, ref epSender, new AsyncCallback(ReceiveFromClients), epSender);
        receiveString = str;
        if (!string.IsNullOrEmpty(receiveString))
        {
            EventCenter.Broadcast(EventTypes.UdpMessageHandle, receiveString);
        }

        //BytesToStruct(iar,epSender);
        //reciveText.text = str;
    }
    private void Update()
    {
        if (isOpenReceive)
        {
            lock (lockObject)
            {
                GameManager.instance.UdpMessageHandle(receiveString);
                isOpenReceive = false;
            }

        }
    }
    private void ReceiveUDP()
    {
        while (true)
        {
            receiveString = System.Text.Encoding.UTF8.GetString(udpClient.Receive(ref remoteEndPoint));
            Debug.Log("Received message: " + receiveString + " from " + remoteEndPoint);
            isOpenReceive = true;
        }
    }
    /// <summary>
    /// 关闭Socket
    /// </summary>
    public void SocketQuit()
    {
        if (serverSocket != null)
        {
            serverSocket.Close();
        }
    }

    /// <summary>
    /// 应用关闭时关闭Socket
    /// </summary>
    private void OnApplicationQuit()
    {
        SocketQuit();
        if (this.t != null)
        {
            this.t.Abort();
        }

        if (this.udpClient != null)
        {
            this.udpClient.Close();
        }
    }

    /// <summary>
    /// 当关闭此对象时关闭Socket
    /// </summary>
    private void OnDisable()
    {
        SocketQuit();
        if (this.t != null)
        {
            this.t.Abort();
        }

        if (this.udpClient != null)
        {
            this.udpClient.Close();
        }
    }

}