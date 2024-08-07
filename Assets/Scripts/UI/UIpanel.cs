using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
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
    public virtual void Show(Tween tween)
    {
        Open();
        tween?.Play();
    }
    public virtual void Hide(Tween tween)
    {
        tween?.OnComplete(Open);
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


