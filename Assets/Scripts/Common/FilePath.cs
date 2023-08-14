using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FilePath
{
    public static string DataPath = Application.streamingAssetsPath;
    public static string SettingPath = "/Setting/Setting.txt";
    public static string VideoPath = "/Videos";

    public static void Init()
    {
        SettingPath = DataPath + SettingPath;
        VideoPath = DataPath + VideoPath;

    }
}
