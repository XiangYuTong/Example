using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class Common
{
    /// <summary>
    /// 2ȫ�� 3Ϊ��ʱǿ�ƶ�λ
    /// </summary>
    public static int max = 2;
    /// <summary>
    /// �Ƿ��ö� 0���ö�  1�ö�
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
    /// ��
    /// </summary>
    public static int width = 0;
    /// <summary>
    /// ��
    /// </summary>
    public static int height = 0;
    /// <summary>
    /// #����ģʽ 1 ���������� 0����С������
    /// </summary>
    public static int startmode = 1;
    /// <summary>
    /// #�Ƿ�������� 0������ 1����
    /// </summary>
    public static int hidecursor = 0;
    /// <summary>
    /// #�Ƿ����������� 0������ 1 ����
    /// </summary>
    public static int hidetaskbar = 1;
    /// <summary>
    /// #�Ƿ����ģʽ
    /// </summary>
    public static int debug = 0;
    /// <summary>
    /// #��������ʱ�� 0Ϊ������ ��λ��
    /// </summary>
    public static int backtime = 0;
    /// <summary>
    /// #httpserver�����˿�
    /// </summary>
    public static int httpport = 8030;
    /// <summary>
    /// httpip��ַ
    /// </summary>
    public static string httpip;
    /// <summary>
    /// #udpserver�����˿�
    /// </summary>
    public static int udpport = 8886;
    /// <summary>
    /// UDPIP��ַ
    /// </summary>
    public static string udpip;
    /// <summary>
    /// #tcpserver�����˿�
    /// </summary>
    public static int tcpport = 4210;
    /// <summary>
    /// TCPIP��ַ
    /// </summary>
    public static string tcpip;
    /// <summary>
    /// ���ڻ�ȡ��Ƶ·��ɸѡ������
    /// </summary>
    private static List<string> videoExtensions = new List<string>() { ".mp4", ".avi", ".mov", ".m4v" };
    /// <summary>
    /// ���ڻ�ȡ���·��ɸѡ������
    /// </summary>
    private static List<string> ExcelExtensions = new List<string>() { ".xlsx", ".xls", ".xlsm", };
    /// <summary>
    /// ��Ƶ·��
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
                // �ж��ļ���չ���Ƿ�����Ƶ�ļ���չ���б���
                if (videoExtensions.Contains(System.IO.Path.GetExtension(file).ToLower()))
                {
                    // ����ļ���չ����.mp4��������ӵ�videoFiles������
                    // resultFileName[i] = file;
                    temp.Add(file);
                }
                //else if ( System.IO.Path.GetExtension(file).ToLower() == ".avi" || System.IO.Path.GetExtension(file).ToLower() == ".mov"|| System.IO.Path.GetExtension(file).ToLower() == ".m4v")
                //{
                //    // ����ļ���չ������.mp4�����Ի�ȡ������ʽ����Ƶ�ļ�
                //    //resultFileName[i] = file;
                //    temp.Add(file);
                //}
            }
        }
        catch
        {
            Debug.LogError("Videos�ļ��в�����,����");
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
            Debug.LogError("IP ��ȡʧ��");
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
                // �ж��ļ���չ���Ƿ�����Ƶ�ļ���չ���б���
                if (ExcelExtensions.Contains(System.IO.Path.GetExtension(file).ToLower()))
                {
                    // ����ļ���չ����.mp4��������ӵ�videoFiles������
                    // resultFileName[i] = file;
                    temp.Add(file);
                }
                //else if ( System.IO.Path.GetExtension(file).ToLower() == ".avi" || System.IO.Path.GetExtension(file).ToLower() == ".mov"|| System.IO.Path.GetExtension(file).ToLower() == ".m4v")
                //{
                //    // ����ļ���չ������.mp4�����Ի�ȡ������ʽ����Ƶ�ļ�
                //    //resultFileName[i] = file;
                //    temp.Add(file);
                //}
            }
        }
        catch
        {
            Debug.LogError("excel�ļ��в�����,����");
        }
        return temp.ToArray();

    }
}
