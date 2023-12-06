/*
* @Author: 小羊
* @Description: Json数据管理器
* @Date: 2023年06月08日 星期四 12:06:44
* @Modify:
*/
using System;
using LitJson;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// 序列化和反序列化Json时  使用的是哪种方案
/// </summary>
public enum JsonType
{
    JsonUtlity,
    LitJson,
    JsonConvert,
}

/// <summary>
/// Json数据管理类 主要用于进行 Json的序列化存储到硬盘 和 反序列化从硬盘中读取到内存中
/// </summary>
public class JsonManager
{
    /// <summary>
    /// 存储 Json 的读写文件夹位置
    /// </summary>
    private static readonly string FILE_DIRECTORY = Application.streamingAssetsPath + "/Json/";
    
    /// <summary>
    /// 存储 Json 的数据文件夹位置
    /// </summary>
    private static readonly string DATA_DIRECTORY = Application.streamingAssetsPath + "/Json/";

    /// <summary>
    /// 文件后缀名，默认为 .json
    /// </summary>
    private static readonly string FILE_SUFFIX = ".json";
    
    public static JsonManager Instance { get; set; } = new JsonManager();

    private JsonManager() { }

    // 存储Json数据 序列化
    public void SaveData(object data, string fileName, JsonType type = JsonType.LitJson) {
        // 确定存储路径
        string path = FILE_DIRECTORY + fileName + FILE_SUFFIX;
        // 序列化 得到Json字符串
        string jsonStr = "";
        switch (type) {
            case JsonType.JsonUtlity:
                jsonStr = JsonUtility.ToJson(data);
                break;
            case JsonType.LitJson:
                jsonStr = JsonMapper.ToJson(data);
                break;
            case JsonType.JsonConvert:
                File.Create(path).Dispose();
                jsonStr = JsonConvert.SerializeObject(data, Formatting.Indented);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        // 把序列化的Json字符串 存储到指定路径的文件中
        File.WriteAllText(path, jsonStr);
    }

    // 读取指定文件中的 Json数据 反序列化
    public T LoadData<T>(string fileName, JsonType type = JsonType.LitJson) where T : new() {
        // 确定从哪个路径读取
        // 首先先判断 默认数据文件夹中是否有我们想要的数据 如果有 就从中获取
        string path = DATA_DIRECTORY + fileName + FILE_SUFFIX;
        // 先判断 是否存在这个文件
        // 如果不存在默认文件 就从 读写文件夹中去寻找
        if (!File.Exists(path))
            path = FILE_DIRECTORY + fileName + FILE_SUFFIX;
        // 如果读写文件夹中都还没有 那就返回一个默认对象
        if (!File.Exists(path))
            return new T();

        // 进行反序列化
        string jsonStr = File.ReadAllText(path);
        // 数据对象
        T data = default(T);
        switch (type) {
            case JsonType.JsonUtlity:
                data = JsonUtility.FromJson<T>(jsonStr);
                break;
            case JsonType.LitJson:
                data = JsonMapper.ToObject<T>(jsonStr);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        // 把对象返回出去
        return data;
    }
}
