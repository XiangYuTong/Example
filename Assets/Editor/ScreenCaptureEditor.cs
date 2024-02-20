using System.IO;
using UnityEditor;
using UnityEngine;

public class ScreenCaptureEditor : EditorWindow
{
    private static string directory = "Screenshots/Capture/";
    private static string latestScreenshotPath = "";
    private bool initDone = false;

    private GUIStyle BigText;

    void InitStyles()
    {
        initDone = true;
        BigText = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };
    }

    private void OnGUI()
    {
        if (!initDone)
        {
            InitStyles();
        }

        GUILayout.Label("Screen Capture Tools", BigText);
        if (GUILayout.Button("���Ž�ͼ"))
        {
            TakeScreenshot();
        }
        GUILayout.Label("��ǰ�ֱ��ʣ� " + GetResolution());

        if (GUILayout.Button("���ļ���"))
        {
            ShowFolder();
        }
        GUILayout.Label("����·��: " + directory);
    }

    [MenuItem("Tools/Screenshots/�򿪴��� &`", false, 0)]
    public static void ShowWindow()
    {
        GetWindow(typeof(ScreenCaptureEditor));
    }

    [MenuItem("Tools/Screenshots/�洢·�� &2", false, 2)]
    private static void ShowFolder()
    {
        if (File.Exists(latestScreenshotPath))
        {
            EditorUtility.RevealInFinder(latestScreenshotPath);
            return;
        }
        Directory.CreateDirectory(directory);
        EditorUtility.RevealInFinder(directory);
    }

    [MenuItem("Tools/Screenshots/���Ž�ͼ &1", false, 1)]
    private static void TakeScreenshot()
    {
        Directory.CreateDirectory(directory);
        var currentTime = System.DateTime.Now;
        var filename = currentTime.ToString().Replace('/', '-').Replace(':', '_') + ".png";
        var path = directory + filename;
        ScreenCapture.CaptureScreenshot(path);
        latestScreenshotPath = path;
        Debug.Log($"��ͼ·��: <b>{path}</b> �ֱ��ʣ� <b>{GetResolution()}</b>");
    }

    private static string GetResolution()
    {
        Vector2 size = Handles.GetMainGameViewSize();
        Vector2Int sizeInt = new Vector2Int((int)size.x, (int)size.y);
        return $"{sizeInt.x}x{sizeInt.y}";
    }

}