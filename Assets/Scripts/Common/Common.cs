using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class Common
{
    /// <summary>
    /// 2全屏 3为定时强制定位
    /// </summary>
    public static int max = 2;
    /// <summary>
    /// 是否置顶 0不置顶  1置顶
    /// </summary>
    public static int topmost = 1;
    /// <summary>
    /// x
    /// </summary>
    public static int left = 0;
    /// <summary>
    /// y
    /// </summary>
    public static int top = 0;
    /// <summary>
    /// 宽
    /// </summary>
    public static int width = 0;
    /// <summary>
    /// 高
    /// </summary>
    public static int height = 0;
    /// <summary>
    /// #启动模式 1 是正常启动 0是最小化启动
    /// </summary>
    public static int startmode = 1;
    /// <summary>
    /// #是否隐藏鼠标 0不隐藏 1隐藏
    /// </summary>
    public static int hidecursor = 0;
    /// <summary>
    /// #是否隐藏任务栏 0不隐藏 1 隐藏
    /// </summary>
    public static int hidetaskbar = 1;
    /// <summary>
    /// #是否调试模式
    /// </summary>
    public static int debug = 0;
    /// <summary>
    /// #返回屏保时间 0为不启动 单位秒
    /// </summary>
    public static int backtime = 0;
    /// <summary>
    /// #httpserver监听端口
    /// </summary>
    public static int httpport = 8030;
    /// <summary>
    /// httpip地址
    /// </summary>
    public static string httpip;
    /// <summary>
    /// #udpserver监听端口
    /// </summary>
    public static int udpport = 8886;
    /// <summary>
    /// UDPIP地址
    /// </summary>
    public static string udpip;
    /// <summary>
    /// #tcpserver监听端口
    /// </summary>
    public static int tcpport = 4210;
    /// <summary>
    /// TCPIP地址
    /// </summary>
    public static string tcpip;
    /// <summary>
    /// 用于获取视频路径筛选的数组
    /// </summary>
    private static List<string> videoExtensions = new List<string>() { ".mp4", ".avi", ".mov", ".m4v" };
    /// <summary>
    /// 用于获取表格路径筛选的数组
    /// </summary>
    private static List<string> ExcelExtensions = new List<string>() { ".xlsx", ".xls", ".xlsm", };
    /// <summary>
    /// 视频路径
    /// </summary>
    public static string[] videoFileName;
    public static void Init()
    {
        Setting set = new Setting();
        set.Open(FilePath.SettingPath);
        max = Int16.Parse(set.ReadValue("max", "0"));
        topmost = Int16.Parse(set.ReadValue("topmost", "1"));
        left = Int16.Parse(set.ReadValue("left", "0"));
        top = Int16.Parse(set.ReadValue("top", "0"));
        width = Int16.Parse(set.ReadValue("width", "1920"));
        height = Int16.Parse(set.ReadValue("height", "1080"));
        startmode = Int16.Parse(set.ReadValue("startmode", "0"));
        hidecursor = Int16.Parse(set.ReadValue("hidecursor", "0"));
        hidetaskbar = Int16.Parse(set.ReadValue("hidetaskbar", "1"));
        debug = Int16.Parse(set.ReadValue("debug", "0"));
        backtime = Int16.Parse(set.ReadValue("backtime", "300"));
        httpport = Int16.Parse(set.ReadValue("httpport", "8020"));
        httpip = set.ReadValue("httpip", "127.0.0.1");
        udpport = Int16.Parse(set.ReadValue("udpport", "8886"));
        udpip = set.ReadValue("udpip", "127.0.0.1");
        tcpport = Int16.Parse(set.ReadValue("tcpport", "4020"));
        tcpip = set.ReadValue("tcpip", "127.0.0.1");
        if (string.Equals(tcpip, "127.0.0.1"))
        {
            tcpip = GetLocalIPv4();
        }
        GetVideoFiles(FilePath.VideoPath, out videoFileName);
    }

    public static void GetVideoFiles(string path, out string[] resultFileName)
    {
        List<string> temp = new List<string>();
        try
        {
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);


            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                // 判断文件扩展名是否在视频文件扩展名列表中
                if (videoExtensions.Contains(System.IO.Path.GetExtension(file).ToLower()))
                {
                    // 如果文件扩展名是.mp4，则将其添加到videoFiles数组中
                    // resultFileName[i] = file;
                    temp.Add(file);
                }
                //else if ( System.IO.Path.GetExtension(file).ToLower() == ".avi" || System.IO.Path.GetExtension(file).ToLower() == ".mov"|| System.IO.Path.GetExtension(file).ToLower() == ".m4v")
                //{
                //    // 如果文件扩展名不是.mp4，则尝试获取其他格式的视频文件
                //    //resultFileName[i] = file;
                //    temp.Add(file);
                //}
            }
        }
        catch
        {
            Debug.LogError("Videos文件夹不存在,请检查");
        }
        resultFileName = temp.ToArray();

    }
    public static string GetLocalIPv4()
    {
        string ipAddress = "";
        try
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip.ToString();
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("IP 获取失败");
        }
        return ipAddress;
    }
    public static string[] GetExcelFiles(string path)
    {
        List<string> temp = new List<string>();
        try
        {
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);


            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                // 判断文件扩展名是否在视频文件扩展名列表中
                if (ExcelExtensions.Contains(System.IO.Path.GetExtension(file).ToLower()))
                {
                    // 如果文件扩展名是.mp4，则将其添加到videoFiles数组中
                    // resultFileName[i] = file;
                    temp.Add(file);
                }
                //else if ( System.IO.Path.GetExtension(file).ToLower() == ".avi" || System.IO.Path.GetExtension(file).ToLower() == ".mov"|| System.IO.Path.GetExtension(file).ToLower() == ".m4v")
                //{
                //    // 如果文件扩展名不是.mp4，则尝试获取其他格式的视频文件
                //    //resultFileName[i] = file;
                //    temp.Add(file);
                //}
            }
        }
        catch
        {
            Debug.LogError("excel文件夹不存在,请检查");
        }
        return temp.ToArray();

    }
}
