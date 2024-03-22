using System.Collections.Generic;
using UnityEngine;


public class UIMgr : MonoBehaviour
{
    private static UIMgr _instance;//单例

    public static UIMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                UIMgr ins = FindObjectOfType<UIMgr>();
                if (ins == null)
                {
                    Debug.LogError("场景中没有UIMgr组件，请添加");
                }
                else
                {
                    _instance = ins;
                }
            }
            return _instance;
        }
    }
    Dictionary<string, UIpanel> ui_dict = new Dictionary<string, UIpanel>(); // 存放Uipanel的字典


    /// <summary>
    /// 获取子物体下的所有panel加到字典中,并且初始化
    /// </summary>
    public void Init()
    {
        UIpanel[] panel = transform.GetComponentsInChildren<UIpanel>(true);

        for (int i = 0; i < panel.Length; i++)
        {
            string name = panel[i].GetType().Name;
            if (!ui_dict.ContainsKey(name))
            {
                ui_dict.Add(name, panel[i]);
            }
            else
            {
                Debug.LogError("存在相同的Panel，请检查");
            }
        }
        foreach (var item in ui_dict)
        {
            item.Value.Init();
        }
    }
    /// <summary>
    /// 获取字典中的panel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetPanel<T>() where T : UIpanel
    {
        return (T)ui_dict[typeof(T).Name];
    }
    /// <summary>
    /// 直接打开字典中的panel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Open<T>() where T : UIpanel
    {
        ui_dict[typeof(T).Name].Open();
        return (T)ui_dict[typeof(T).Name];
    }
    /// <summary>
    /// 直接关闭字典中的panel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Close<T>() where T : UIpanel
    {
        ui_dict[typeof(T).Name].Close();
        return (T)ui_dict[typeof(T).Name];
    }
    /// <summary>
    /// 通过Close关闭所有开着的Plane
    /// </summary>
    public void CloseAll()
    {
        foreach (var item in ui_dict)
        {
            if (item.Value.isOpen)
                item.Value.Close();
        } 
    }
    /// <summary>
    /// 直接通过setactivity去关闭所有开着的Plane
    /// </summary>
    public void CloseAllWithoutNotify()
    {
        foreach (var item in ui_dict)
        {
            if (item.Value.isOpen)
            {
                item.Value.isOpen = false;
                item.Value.gameObject.SetActive(false);
            }
                
        }
    }
    // Update is called once per frame
    void Update()
    {
     
    }
    [ContextMenu("自动注册所有脚本")]
    public void RegisterPanel()
    {
        Debug.Log("注册");
        foreach (var item in ui_dict)
        {

        }
    }
}

