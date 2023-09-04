using System.Collections;
using UnityEngine;

/// <summary>
/// 点开屏保后长时间不操作
/// </summary>
public class LongTimeNoOperationOneTime : MonoBehaviour
{   
    private static LongTimeNoOperationOneTime _instance;

    public static LongTimeNoOperationOneTime Instance
    {
        get
        {
            if (_instance == null)
            {
                LongTimeNoOperationOneTime ins = FindObjectOfType<LongTimeNoOperationOneTime>();
                if (ins == null)
                {
                    Debug.LogError("场景中没有LongTimeNoOperationOneTime组件，请添加");
                }
                else
                {
                    _instance = ins;
                }
            }
            return _instance;
        }
        
    }


    private bool check = false;//是否开始检测
    public float lasterTime;//上次的时间
    public float nowTime;//当前时间

    public void Update()
    {
        if (check && Common.backtime > 0) CheckOperate();
    }

    /// <summary>
    /// 进行无操作监测
    /// </summary>
    public void CheckOperate()
    {
        //当前时间
        nowTime = Time.time;

        //如果有操作则更新上次操作时间为此时
        if (Application.isEditor)//在编辑器环境下
        {
            //若点击鼠标左键/有操作，则更新触摸时间
            if (Input.GetMouseButtonDown(0)) lasterTime = nowTime;
        }
        //非编辑器环境下，触屏操作
        //Input.touchCount在pc端没用，只在移动端生效
        //Application.isMobilePlatform在pc和移动端都生效
        else if (Application.isMobilePlatform)
        {
            //有屏幕手指 接触
            if (Input.touchCount > 0) lasterTime = nowTime;//更新触摸时间
        }

        //判断无操作时间是否达到指定时长，若达到指定时长无操作，则执行TakeOperate
        float offsetTime = Mathf.Abs(nowTime - lasterTime);
        if (offsetTime > Common.backtime) TakeOperate();

    }

    /// <summary>
    /// 当长时间无操作时执行这个操作
    /// </summary>
    public void TakeOperate()
    {
        Debug.Log("执行");
        check = false;
    }
    public void StartTime()
    {
        check = true;
        lasterTime = Time.time;
    }
}