using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// �������
/// </summary>
public class gameEnter : MonoBehaviour
{
    public enum NetType
    {
        None, UdpSender, UdpReceiver, TcpSender, TcpReceiver
    }
    [Header("����ģʽ")]
    public NetType netType;
    // Start is called before the first frame update
    /// <summary>
    /// ����ϵͳ��ʼ��
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        FilePath.Init();//��ʼ�������ļ�·��
        Common.Init();//��ʼ������
        WindowMode.Instance.SetWindowMode();
        //ApplicationSetting.Init();//ϵͳ��ʼ��
        NetInit();//��ʼ������ģʽ
        UIMgr.instance.Init();//UI��ʼ��
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdateResourceGC();
    }
    private float lastGCTime;
    //�Զ���������
    void OnUpdateResourceGC()
    {
        if (Time.time - lastGCTime > 60)
        {
            lastGCTime = Time.time;
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }
    void OnApplicationQuit()
    {

        if (!Application.isEditor)
        {
            WindowMode.Instance.ShowTaskbar();
            Process.GetCurrentProcess().Kill();
        }


    }
    /// <summary>
    /// ��ʼ������ģʽ
    /// </summary>
    private void NetInit()
    {
        switch (netType)
        {
            case NetType.None:
                break;
            case NetType.UdpSender:
                this.gameObject.AddComponent<UdpSender>();
                break;
            case NetType.UdpReceiver:
                this.gameObject.AddComponent<UdpReceiver>();
                break;
            case NetType.TcpSender:
                break;
            case NetType.TcpReceiver:
                break;
            default:
                break;
        }
    }




}