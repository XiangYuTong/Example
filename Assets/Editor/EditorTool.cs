using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

public class EditorTool : Editor
{
    [MenuItem("GameObject/Test/�ҵ��Զ��巽��",false,1)]
    private static void MyCustomMethod()
    {
        // ��ȡ��ǰѡ�е���Ϸ����
        foreach (GameObject obj in Selection.gameObjects)
        {
            Debug.Log(obj.name);
        }

    }

}
