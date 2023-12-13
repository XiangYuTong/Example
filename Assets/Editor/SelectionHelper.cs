using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
// using Game;
// using Nirvana;

internal class SelectionHelper
{
    private static string selectObjPath;
    private static GameObject selectPrefab;

    [InitializeOnLoadMethod]
    private static void Start()
    {
        //在Hierarchy面板按空格键相当于开关GameObject
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;

        //在Project面板按空格键相当于Show In Explorer
        EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
    }

    private static void ProjectWindowItemOnGUI(string guid, Rect selectionRect)
    {
        if (Event.current.type == UnityEngine.EventType.KeyDown
            && Event.current.keyCode == KeyCode.Space
            && selectionRect.Contains(Event.current.mousePosition))
        {
            string strPath = AssetDatabase.GUIDToAssetPath(guid);
            if (Event.current.alt)
            {
                UnityEngine.Debug.Log(strPath);
                Event.current.Use();
                return;
            }

            if (Path.GetExtension(strPath) == string.Empty) //文件夹
            {
                Process.Start(Path.GetFullPath(strPath));
            }
            else //文件
            {
                Process.Start("explorer.exe", "/select," + Path.GetFullPath(strPath));
            }

            Event.current.Use();
        }
    }

    private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        Event e = Event.current;
        if (e.type == UnityEngine.EventType.KeyDown)
        {
            switch (e.keyCode)
            {
                case KeyCode.Space:
                    ToggleGameObjcetActiveSelf();
                    e.Use();
                    break;
            }
        }
    }

    internal static void ToggleGameObjcetActiveSelf()
    {
        Undo.RecordObjects(Selection.gameObjects, "Active");
        foreach (var go in Selection.gameObjects)
        {
            go.SetActive(!go.activeSelf);
        }
    }

}