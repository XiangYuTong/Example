/*
* @Author: 小羊
* @Description:发送UDP消息
* @Date: 2023年06月08日 星期四 17:06:33
* @Modify:
*/

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.IO;
using UnityEditor.Experimental.GraphView;

public class UdpSender : MonoBehaviour
{
    public static UdpSender Instance;
    private UdpClient client;
    IPEndPoint remoteEP ;
    public static string ip;       //IP
    public static  int port = 8886;                //端口号.
    string filePath;
    private void Awake()
    {
        Instance = this;
        ip = Common.udpip;
        port=Common.udpport;
        MyStart();
    }

    public void MyStart()
    {
        client = new UdpClient();
        // 创建一个 IPv4 地址为 127.0.0.1，端口号为 8886 的 IPEndPoint
        remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
    }

    // 发送消息
    public void Sendmessage(string _message) {
        // 模拟发送数据
        Debug.Log("发送信息：" + _message);
        //string message = "Hello, world!";
        byte[] data = Encoding.UTF8.GetBytes(_message);
        
        // 发送数据包
        client.Send(data, data.Length, remoteEP);
    }

    // 关闭连接
    void OnDisable() {
        if (client != null) {
            client.Close();
        }
    }


}