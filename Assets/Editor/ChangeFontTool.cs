using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class ChangeFontTool : EditorWindow
{
    // Start is called before the first frame update
    private GameObject selectGameObject;
    private TMP_FontAsset changeTmpFont;
    private Font changeTextFont;
    [MenuItem("Tools/替换字体")]
    private static void Init()
    {
        var window = GetWindow(typeof(ChangeFontTool), true, "替换字体");
        window.Show();
    }
    private void OnGUI()
    {
        GUILayout.Label("目标物体:");
        selectGameObject = (GameObject)EditorGUILayout.ObjectField(selectGameObject, typeof(GameObject), true, GUILayout.MinWidth(100f));
        GUILayout.Label("替换的TMP字体:");
        changeTmpFont = (TMP_FontAsset)EditorGUILayout.ObjectField(changeTmpFont, typeof(TMP_FontAsset), true, GUILayout.MinWidth(100f));
        GUILayout.Label("替换的Text字体:");
        changeTextFont = (Font)EditorGUILayout.ObjectField(changeTextFont, typeof(Font), true, GUILayout.MinWidth(100f));
        if (GUILayout.Button("替换"))
        {
            if (selectGameObject == null)
            {
                Debug.LogError("选中物体为空，请将物体拖入指定位置！");
                return;
            }
            if (changeTmpFont != null)
            {
                TextMeshProUGUI[] Tmps = selectGameObject.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var item in Tmps)
                {
                    item.font = changeTmpFont;
                    EditorUtility.SetDirty(item);
                }
            }
            if (changeTextFont != null)
            {
                Text[] texts = selectGameObject.GetComponentsInChildren<Text>();
                foreach (var item in texts)
                {
                    item.font = changeTextFont;
                    EditorUtility.SetDirty(item);
                }
            }
            if (changeTmpFont == null && changeTextFont == null)
            {
                Debug.LogError("请至少放入一种字体！");
            }
        }
        if (GUILayout.Button("全场景替换"))
        {
            if (changeTmpFont != null)
            {
                TextMeshProUGUI[] Tmps = FindObjectsOfType<TextMeshProUGUI>();
                foreach (var item in Tmps)
                {
                    item.font = changeTmpFont;
                    EditorUtility.SetDirty(item);
                }
            }
            if (changeTextFont != null)
            {
                Text[] texts = FindObjectsOfType<Text>();
                foreach (var item in texts)
                {
                    item.font = changeTextFont;
                    EditorUtility.SetDirty(item);
                }
            }
            if (changeTmpFont == null && changeTextFont == null)
            {
                Debug.LogError("请至少放入一种字体！");
            }
        }
    }
}
