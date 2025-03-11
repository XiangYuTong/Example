using System;
using UnityEngine;

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
    bool Islate = false;
    public GameObject Tip;

    [Header("����ģʽ")]
    public NetType netType;
    [Header("ʱ�����������")]
    public bool timerEnable;
    [Header("�Զ��������տ���")]
    public bool GCSwitch;
    [Header("UDP�Ƿ��첽����")]
    public bool isUdpAsyn = true;
    //ʱ�����Ƿ��
    private bool isOpenLocker = false;
    // Start is called before the first frame update
    /// <summary>
    /// ����ϵͳ��ʼ��
    /// </summary>
    private void Awake()
    {   
        if(isOpenLocker)
            CheckLocker();
        if (Islate == false)
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
    private void CheckLocker()
    {
        Tip.SetActive(false);
        //DateTime minTime = Convert.ToDateTime("2019-8-1 15:29:00");
        DateTime maxTime = Convert.ToDateTime("2024-9-18 11:31:00");
        if (DateTime.Now >= maxTime)
        {
            //����ʹ��ʱ���ڣ���ֱ���˳�����
            Islate = true;
        }
        try
        {
            //����ʹ��ʱ�䣬����������������,ֱ���˳�����
            if (Islate)
            {
                Tip.SetActive(true);
                //Invoke("OnExit", 5);//��ʱ�˳��������˳�ǰ��ʾ��ʾ��Ϣ
            }
        }
        catch
        {
            Islate = false;
        }
    }
    private void OnExit()
    {
        Application.Quit();
    }
    public void UdpMessageHandle(string message)
    {
        UnityEngine.Debug.Log(message);
        if (String.IsNullOrEmpty(message))
            return;
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
                UdpReceiver.Instance.Init(isUdpAsyn);
                break;
            case NetType.UdpBoth:
                UdpSender.Instance.Init();
                UdpReceiver.Instance.Init(isUdpAsyn);
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
