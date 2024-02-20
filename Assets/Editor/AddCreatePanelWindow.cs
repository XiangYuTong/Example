using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System;

public class AddCreatePanelWindow : EditorWindow
{
    private static string panelName;
    private static string tip = "";
    private static string uiFrefabPath = "UI/PanelPrefab";
    private static string scriptModelPath= "Assets/Scripts/UI/ModelPanel.cs";
    private static string _spawnPath = "Assets/Scripts/UI/";
    private static string scriptModelContent;

    [MenuItem("Tools/创建一个新的UIPanel")]
    private static void Init()
    {
        var window = GetWindow(typeof(AddCreatePanelWindow),true,"输入UIPanel的名字");
        window.Show();
        scriptModelContent = File.ReadAllText(scriptModelPath);
    }
    private void OnGUI()
    {
        panelName = EditorGUILayout.TextField("Panel的名字:", panelName);
        GUI.color = Color.red;
        GUILayout.Label(tip);
        GUI.color = Color.white;
        if (GUILayout.Button("创建"))
        {
            if (string.IsNullOrEmpty(panelName))
            {
                tip = "名字不能为空，请重新输入！";
                return;
            }
            GameObject uimgr = FindObjectOfType<UIMgr>().gameObject;
            if (uimgr == null)
            {
                tip = "场景中不存在UIMgr,请添加！";
                return;
            }
            GameObject panel = GameObject.Instantiate(Resources.Load<GameObject>(uiFrefabPath));
            if (panel == null)
            {
                tip = "UIPanel预制体为空，请检查！";
                return;
            }
            if (string.IsNullOrEmpty(scriptModelContent))
            {
                tip = "模板内容为空,请检查！";
                return;
            }
            panel.transform.parent = uimgr.transform.GetChild(0).transform;
            panel.name = panelName;


            var path = _spawnPath + panelName + ".cs";
            //替换类名
            scriptModelContent = scriptModelContent.Replace("ModelPanel", panelName);

            if (scriptModelContent != null)
            {
                //替换命名空间
                //scriptModelContent = scriptModelContent.Replace("TemplateNameSpace", NameSpace);
            }
            //将组织好的内容写入文件
            File.WriteAllText(path, scriptModelContent, Encoding.UTF8);
            //刷新一下资源，不然创建好文件后第一时间不会显示
            AssetDatabase.Refresh();

            //panel.AddComponent(Type.GetType(panelName));
        }

    }

}
