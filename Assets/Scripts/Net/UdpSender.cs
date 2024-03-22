using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

public class UdpSender : MonoBehaviour
{
    private static UdpSender _instance;//单例

    public static UdpSender Instance
    {
        get
        {
            if (_instance == null)
            {
                UdpSender ins = FindObjectOfType<UdpSender>();
                if (ins == null)
                {
                    Debug.LogError("场景中没有UdpSender组件，已经自动生成");
                    GameObject go = new GameObject(nameof(UdpSender));
                    go.transform.parent = GameManager.instance.transform;
                    _instance = go.AddComponent<UdpSender>();
                }
                else
                {
                    _instance = ins;
                }
            }
            return _instance;
        }
    }

    private UdpClient client;//客户端
    List<IPEndPoint> remoteEP_list = new List<IPEndPoint>();//需要发送的终端Ip列表
    public static string[] ips;       //IP集合
    public static int port = 8886;                //端口号.
    string filePath;
    public void Init()
    {
        DataInit();
        MyStart();
    }
    /// <summary>
    /// 数据初始化
    /// </summary>
    public void DataInit()
    {
        //修改端口
        port = Common.udpport;
        //
        ips = Setting.ReadTextSplit(Common.udpip, '_');
        foreach (var item in ips)
        {
            IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Parse(item), port);
            remoteEP_list.Add(ipendpoint);
        }
    }
    /// <summary>
    /// 启动客户端
    /// </summary>
    public void MyStart()
    {
        client = new UdpClient();
    }

    // 发送消息
    public void Sendmessage(string _message)
    {
        // 模拟发送数据
        Debug.Log("发送信息：" + _message);
        //string message = "Hello, world!";
        byte[] data = Encoding.UTF8.GetBytes(_message);

        // 对所有的终端发送数据包
        foreach (var item in remoteEP_list)
        {
            client.Send(data, data.Length, item);
        }

    }

    // 关闭连接
    void OnDisable()
    {
        if (client != null)
        {
            client.Close();
        }
    }
    private void OnDestroy()
    {
        if (client != null)
        {
            client.Close();
        }
    }
}