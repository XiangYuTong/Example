using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// 程序入口
/// </summary>
public class gameEnter : MonoBehaviour
{
    public enum NetType
    {
        None, UdpSender, UdpReceiver, TcpSender, TcpReceiver
    }
    [Header("网络模式")]
    public NetType netType;
    // Start is called before the first frame update
    /// <summary>
    /// 所有系统初始化
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        FilePath.Init();//初始化所有文件路径
        Common.Init();//初始化常量
        WindowMode.Instance.SetWindowMode();
        //ApplicationSetting.Init();//系统初始化
        NetInit();//初始化网络模式
        UIMgr.instance.Init();//UI初始化
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
    void OnApplicationQuit()
    {

        if (!Application.isEditor)
        {
            WindowMode.Instance.ShowTaskbar();
            Process.GetCurrentProcess().Kill();
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
