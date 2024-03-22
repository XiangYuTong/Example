using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using UnityEngine.UI;

/// <summary>
/// �������
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;//����

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                GameManager ins = FindObjectOfType<GameManager>();
                if (ins == null)
                {
                    UnityEngine.Debug.LogError("������û��GameManager����������");
                }
                else
                {
                    _instance = ins;
                }
            }
            return _instance;
        }
    }
    public enum NetType
    {
        None, UdpSender, UdpReceiver,UdpBoth, TcpServer, TcpClient
    }
  

    [Header("����ģʽ")]
    public NetType netType;
    [Header("ʱ�����������")]
    public bool timerEnable;
    [Header("�Զ��������տ���")]
    public bool GCSwitch;
    // Start is called before the first frame update
    /// <summary>
    /// ����ϵͳ��ʼ��
    /// </summary>
    private void Awake()
    {
        Init();
    }
    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        FilePath.Init();//��ʼ�������ļ�·��
        Common.Init();//��ʼ������
        DataMgr.Instance.Init();
        WindowMode.Instance.SetWindowMode();//��Ļ��ʼ��  
        if (timerEnable)
            TimerMgr.Instance.Init();//��ʼ����ʱ������
        NetInit();//��ʼ������ģʽ
        PoolMgr.Instance.Init();//��ʼ�������
        UIMgr.Instance.Init();//UI��ʼ��
    }

    public void UdpMessageHandle(string message)
    {
        UnityEngine.Debug.Log(message);
    }

    // Update is called once per frame
    void Update()
    {   
        if(GCSwitch)
            OnUpdateResourceGC();

        if (Input.GetKeyDown(KeyCode.S))
        {
            UdpSender.Instance.Sendmessage("���");
        }
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
                UdpSender.Instance.Init();
                break;
            case NetType.UdpReceiver:
                UdpReceiver.Instance.Init();
                break;
            case NetType.UdpBoth:
                UdpSender.Instance.Init();
                UdpReceiver.Instance.Init();
                break;
            case NetType.TcpServer:
                TcpServerMgr.Instance.Init();      
                break;
            case NetType.TcpClient:
                TcpClientMgr.Instance.Init();
                break;
            default:
                break;
        }
    }
}
