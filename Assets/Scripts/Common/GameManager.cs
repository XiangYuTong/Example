using System;
using UnityEngine;

/// <summary>
/// 程序入口
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;//单例

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                GameManager ins = FindObjectOfType<GameManager>();
                if (ins == null)
                {
                    UnityEngine.Debug.LogError("场景中没有GameManager组件，请添加");
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

    [Header("网络模式")]
    public NetType netType;
    [Header("时间管理器开关")]
    public bool timerEnable;
    [Header("自动垃圾回收开关")]
    public bool GCSwitch;
    [Header("UDP是否异步接受")]
    public bool isUdpAsyn = true;
    //时间锁是否打开
    private bool isOpenLocker = false;
    // Start is called before the first frame update
    /// <summary>
    /// 所有系统初始化
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
        FilePath.Init();//初始化所有文件路径
        Common.Init();//初始化常量
        DataMgr.Instance.Init();
        WindowMode.Instance.SetWindowMode();//屏幕初始化  
        if (timerEnable)
            TimerMgr.Instance.Init();//初始化延时管理器
        NetInit();//初始化网络模式
        PoolMgr.Instance.Init();//初始化对象池
        UIMgr.Instance.Init();//UI初始化
    }
    private void CheckLocker()
    {
        Tip.SetActive(false);
        //DateTime minTime = Convert.ToDateTime("2019-8-1 15:29:00");
        DateTime maxTime = Convert.ToDateTime("2024-9-18 11:31:00");
        if (DateTime.Now >= maxTime)
        {
            //不在使用时间内，会直接退出程序
            Islate = true;
        }
        try
        {
            //限制使用时间，如果不在这个区间内,直接退出程序
            if (Islate)
            {
                Tip.SetActive(true);
                //Invoke("OnExit", 5);//延时退出，可在退出前显示提示消息
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
            UdpSender.Instance.Sendmessage("你好");
        }
    }
    private float lastGCTime;
    //自动垃圾回收
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
    /// 初始化网络模式
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
