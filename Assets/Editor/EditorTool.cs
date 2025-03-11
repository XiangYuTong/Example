using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

public class EditorTool : Editor
{
    [MenuItem("GameObject/Test/我的自定义方法",false,1)]
    private static void MyCustomMethod()
    {
        // 获取当前选中的游戏对象
        foreach (GameObject obj in Selection.gameObjects)
        {
            Debug.Log(obj.name);
        }

    }

}
