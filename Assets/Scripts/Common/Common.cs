using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine.UI;
using System.Threading.Tasks;

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
    private static string videoExtensions ="*mp4|*.avi|*.mov|*.m4v";
    /// <summary>
    /// ���ڻ�ȡ���·��ɸѡ������
    /// </summary>
    private static string ExcelExtensions = "*.xlsx|*.xls|*.xlsm";
    /// <summary>
    /// ���ڻ�ȡͼƬ·��ɸѡ������
    /// </summary>
    private static string pictureExtensions = "*.jpg|*.png|*.bmp";
    /// <summary>
    /// ��Ƶ·��
    /// </summary>
    public static string[] videoFileName;

    public static string[] imageFileName;
    public static async void Init()
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
        videoFileName = GetVideoFiles(FilePath.VideoPath);
        imageFileName = GetTexturePath(FilePath.PhotoPath);     
    }

    public static string[] GetVideoFiles(string path)
    {
        List<string> temp = new List<string>();
        try
        {
            DirectoryInfo directoryInfo_1 = new DirectoryInfo(Application.streamingAssetsPath + "/" + path);

            FileInfo[] files;

            string[] ImgType = videoExtensions.Split('|');

            for (int j = 0; j < ImgType.Length; j++)
            {
                files = directoryInfo_1.GetFiles(ImgType[j]);

                for (int i = 0; i < files.Length; i++)
                {
                    temp.Add(files[i].FullName);
                }
            }
 
        }
        catch
        {
            Debug.LogError("Videos�ļ��в�����,����");
        }
        return FileSort(temp).ToArray();

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
          
            DirectoryInfo directoryInfo_1 = new DirectoryInfo(Application.streamingAssetsPath + "/" + path);

            FileInfo[] files;

            string[] ImgType = ExcelExtensions.Split('|');

            for (int j = 0; j < ImgType.Length; j++)
            {
                files = directoryInfo_1.GetFiles(ImgType[j]);

                for (int i = 0; i < files.Length; i++)
                {
                    temp.Add(files[i].FullName);
                }
            }
        }
        catch
        {
            Debug.LogError("excel�ļ��в�����,����");
        }
        return FileSort(temp).ToArray();

    }

    public static string[] GetTexturePath(string FoldPath)
    {   

        List<string> temp = new List<string>();
        try
        {
            DirectoryInfo directoryInfo_1 = new DirectoryInfo(Application.streamingAssetsPath + "/" + FoldPath);

            FileInfo[] files;

            string[] ImgType = pictureExtensions.Split('|');

            for (int j = 0; j < ImgType.Length; j++)
            {
                files = directoryInfo_1.GetFiles(ImgType[j]);

                for (int i = 0; i < files.Length; i++)
                {
                    temp.Add(files[i].FullName);
                }
            }
        }
        catch (Exception)
        {

            Debug.LogError("Photos�ļ��в�����,����");
        }
      
        return FileSort(temp).ToArray();

    }

    private static async Task LoadByFSAsync(string path, RawImage image)
    {
        byte[] result;

        using (FileStream SourceStream = File.Open(path, FileMode.Open))
        {
            result = new byte[SourceStream.Length];
            await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);
        }

        Texture2D tx = new Texture2D(2, 1);

        tx.LoadImage(result);

        float widthRatio = tx.width / image.rectTransform.sizeDelta.x;
        float heightRatio = tx.height / image.rectTransform.sizeDelta.y;

        if (widthRatio / heightRatio > 1.1)
        {
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(tx.width / widthRatio, tx.height / widthRatio);
        }
        else
        {
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(tx.width / heightRatio, tx.height / heightRatio);
        }

        image.texture = tx;
    }
    public static List<string> FileSort(List<string> path)
    {
        string temp;
        for (int i = 0; i < path.Count - 1; i++)
        {
            for (int j = 0; j < path.Count - 1 - i; j++)
            {
                if (CustomSort(path[j], path[j + 1]))
                {
                    temp = path[j];
                    path[j] = path[j + 1];
                    path[j + 1] = temp;
                }
            }
        }
        return path;
    }
    public static bool CustomSort(string str1, string str2)
    {
        int result1 = Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(str1, @"[^0-9]+", ""));
        int result2 = Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(str2, @"[^0-9]+", ""));
        if (result1 > result2)
            return true;
        else
            return false;
    }
}
