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
    /// �򿪽���
    /// </summary>
    public virtual void Open()
    {
        HOpen();
    }
    /// <summary>
    /// �رս���
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
    /// ��Ч���Ĵ�
    /// </summary>
    public virtual void HOpen()
    {
        isOpen = true;
        gameObject.SetActive(true);
    }
    /// <summary>
    /// ��Ч���Ĺر�
    /// </summary>
    public virtual void HClose()
    {
        isOpen = false;
        gameObject.SetActive(false);
    }

   
}


