using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIpanel : MonoBehaviour
{
    public bool isOpen = false;
    public virtual void Init()
    {
        gameObject.SetActive(isOpen);
    }
    /// <summary>
    /// 打开界面
    /// </summary>
    public virtual void Open()
    {
        HOpen();
    }
    /// <summary>
    /// 关闭界面
    /// </summary>
    public virtual void Close()
    {
        HClose();
    }
    /// <summary>
    /// 有效果的打开
    /// </summary>
    public virtual void HOpen()
    {
        isOpen = true;
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 有效果的关闭
    /// </summary>
    public virtual void HClose()
    {
        isOpen = false;
        gameObject.SetActive(false);
    }
}


