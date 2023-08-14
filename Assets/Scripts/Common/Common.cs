using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;

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
    public static void Init()
    {
        Setting set = new Setting();
        set.Open(FilePath.SettingPath);
        max = Int16.Parse(set.ReadValue("max", "0"));
        topmost= Int16.Parse(set.ReadValue("topmost", "1"));
        left= Int16.Parse(set.ReadValue("left", "0"));
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
    }
}
