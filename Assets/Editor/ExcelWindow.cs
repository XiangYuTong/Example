/* 一张xlsx里面有多张表格，因为实际游戏中使用的是json文件，所以生成的时候我只用一张xlsx
 * 一张sheet就是一个数据集合 
 * 根据sheet表字段生成对应的数据集合json
 * 创建json文件同时创建实体类
 */
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System;
using UnityEditor;
using UnityEngine;
using DG.Tweening.Plugins.Core.PathCore;
using System.Security.Cryptography;
using Unity.VisualScripting;

public class ExcelWindow : EditorWindow
{
    private string txt_ExcelName = "data.xlsx"; //表格名称
    private string txt_JsonSavePath = "Json";
    private string txt_EntitySavePath = "Entity";
    private Vector2 scrollPos;
    [MenuItem("Tools/ExcelToJson")]
    static void OpenWindow()
    {
        ExcelWindow window = (ExcelWindow)EditorWindow.GetWindow(typeof(ExcelWindow));
        window.Show();
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("表格名称: ");
        txt_ExcelName = GUILayout.TextField(txt_ExcelName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Json保存路径: ");
        txt_JsonSavePath = GUILayout.TextField(txt_JsonSavePath);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("生成实体类"))
        {
            CreateEntities();
        }
        if (GUILayout.Button("生成Json"))
        {
            ExcelToJson();
        }
        if (GUILayout.Button("生成Json数据读取管理器"))
        {
            CreateJsonDataMgr();
        }
        if (GUILayout.Button("生成Excel数据读取管理器"))
        {
            CreateExcelDataMgr();
        }
        EditorGUILayout.EndScrollView();
    }
    private void  CreateJsonDataMgr()
    {
        if(!string.IsNullOrEmpty(txt_JsonSavePath))
        {
            string dir = $"{Application.dataPath}/Scripts/Game";
            string path = $"{dir}/DataMgr.cs";

            string filepath = $"{Application.streamingAssetsPath}/{txt_JsonSavePath}";

           

            //string[] files = Directory.GetFiles(filepath, "*.json", SearchOption.AllDirectories);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine("public class DataMgr : MonoBehaviour");
            sb.AppendLine("{");
            sb.AppendLine("\tprivate static DataMgr _instance;");
            sb.AppendLine("\tpublic static DataMgr Instance");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tget");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\t if (_instance == null)");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\tDataMgr ins = FindObjectOfType<DataMgr>();");
            sb.AppendLine("\t\t\t\tif (ins == null)");
            sb.AppendLine("\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\tDebug.LogError(\"场景中没有DataMgr组件，请添加\");");
            sb.AppendLine("\t\t\t\t}");
            sb.AppendLine("\t\t\t\telse");
            sb.AppendLine("\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\t _instance = ins;");
            sb.AppendLine("\t\t\t\t}");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t\treturn _instance;");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");

            DirectoryInfo directory = new DirectoryInfo(filepath);

            if (directory.Exists)
            {
                FileInfo[] files = directory.GetFiles("*.json");

                foreach (FileInfo file in files)
                {

                    string tempName = file.Name.Replace(".json", "");
                    sb.AppendLine($"\tpublic List<{tempName}> {tempName}_list;");
                }
            }
            else
            {
                //Debug.LogError("指定的文件夹不存在: " + folderPath);
            }


            sb.AppendLine("\tpublic void Init(){\t\n");
            if (directory.Exists)
            {
                FileInfo[] files = directory.GetFiles("*.json");

                foreach (FileInfo file in files)
                {

                    string tempName = file.Name.Replace(".json", "");
                    sb.AppendLine($"\t\t{tempName}_list = JsonManager.Instance.LoadData<List<{tempName}>>(\"{tempName}\");");
                }
            }
            else
            {
                //Debug.LogError("指定的文件夹不存在: " + folderPath);
            }
            sb.AppendLine("\t}\n");

            sb.AppendLine("\tpublic void SaveAll(JsonType type = JsonType.LitJson){\n");

            if (directory.Exists)
            {
                FileInfo[] files = directory.GetFiles("*.json");

                foreach (FileInfo file in files)
                {

                    string tempName = file.Name.Replace(".json", "");
                    sb.AppendLine($"\t\tJsonManager.Instance.SaveData({tempName}_list,\"{tempName}\",type);");
                }
            }
            else
            {
                //Debug.LogError("指定的文件夹不存在: " + folderPath);
            }

            sb.AppendLine("\t}");


            sb.AppendLine("\tpublic void Save(object data, string fileName,JsonType type = JsonType.LitJson){\n");
            sb.AppendLine($"\t\tJsonManager.Instance.SaveData(data,fileName,type);");
            sb.AppendLine("\t}");








            sb.AppendLine("}");
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (!File.Exists(path))
                {
                    File.Create(path).Dispose(); //避免资源占用
                }
                File.WriteAllText(path, sb.ToString());
                AssetDatabase.Refresh();
            }
            catch (System.Exception e)
            {
                
            }
           
        }
    }

    private void CreateExcelDataMgr()
    {
        if (!string.IsNullOrEmpty(txt_ExcelName))
        {
            string filepath = Application.streamingAssetsPath + "/Excel/" + txt_ExcelName;
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                using (ExcelPackage ep = new ExcelPackage(fs))
                {
                    //获得所有工作表
                    ExcelWorksheets workSheets = ep.Workbook.Worksheets;
                    //遍历所有工作表
                    

                    string dir = $"{Application.dataPath}/Scripts/Game";
                    string path = $"{dir}/DataMgr.cs";

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("using System.Collections;");
                    sb.AppendLine("using System.Collections.Generic;");
                    sb.AppendLine("using UnityEngine;");
                    sb.AppendLine();
                    sb.AppendLine("public class DataMgr : MonoBehaviour");
                    sb.AppendLine("{");
                    sb.AppendLine("\tprivate static DataMgr _instance;");
                    sb.AppendLine("\tpublic static DataMgr Instance");
                    sb.AppendLine("\t{");
                    sb.AppendLine("\t\tget");
                    sb.AppendLine("\t\t{");
                    sb.AppendLine("\t\t\t if (_instance == null)");
                    sb.AppendLine("\t\t\t{");
                    sb.AppendLine("\t\t\t\tDataMgr ins = FindObjectOfType<DataMgr>();");
                    sb.AppendLine("\t\t\t\tif (ins == null)");
                    sb.AppendLine("\t\t\t\t{");
                    sb.AppendLine("\t\t\t\t\tDebug.LogError(\"场景中没有DataMgr组件，请添加\");");
                    sb.AppendLine("\t\t\t\t}");
                    sb.AppendLine("\t\t\t\telse");
                    sb.AppendLine("\t\t\t\t{");
                    sb.AppendLine("\t\t\t\t\t _instance = ins;");
                    sb.AppendLine("\t\t\t\t}");
                    sb.AppendLine("\t\t\t}");
                    sb.AppendLine("\t\t\treturn _instance;");
                    sb.AppendLine("\t\t}");
                    sb.AppendLine("\t}");

                    for (int i = 1; i <= workSheets.Count; i++)
                    {
                        Debug.Log(workSheets[i].Name);
                        sb.AppendLine($"\tpublic List<{workSheets[i].Name}> {workSheets[i].Name}_list;");
                    }




                    sb.AppendLine("\tpublic void Init(){\t\n");

                    for (int i = 1; i <= workSheets.Count; i++)
                    {
                        Debug.Log(workSheets[i].Name);
                        sb.AppendLine($"\t\t{workSheets[i].Name}_list = ExcelTool.ReadExcel<{workSheets[i].Name}>({i-1});");
                    }


                    sb.AppendLine("\t}\n");

                    sb.AppendLine("\tpublic void SaveAll(JsonType type = JsonType.LitJson){\n");



                    sb.AppendLine("\t}");


                    sb.AppendLine("\tpublic void Save(object data, string fileName,JsonType type = JsonType.LitJson){\n");
                    sb.AppendLine($"\t\tJsonManager.Instance.SaveData(data,fileName,type);");
                    sb.AppendLine("\t}");



                    sb.AppendLine("}");
                    AssetDatabase.Refresh();
                    try
                    {
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        if (!File.Exists(path))
                        {
                            File.Create(path).Dispose(); //避免资源占用
                        }
                        File.WriteAllText(path, sb.ToString());
                        AssetDatabase.Refresh();
                    }
                    catch (System.Exception e)
                    {

                    }
                }
            }
        }
    


    }
    private void ExcelToJson()
    {
        if (!string.IsNullOrEmpty(txt_ExcelName))
        {
            string filepath = Application.streamingAssetsPath + "/Excel/" + txt_ExcelName;
            string headPath = $"{Application.streamingAssetsPath}/{txt_JsonSavePath}";
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                using (ExcelPackage ep = new ExcelPackage(fs))
                {
                    //获得所有工作表
                    ExcelWorksheets workSheets = ep.Workbook.Worksheets;
                    List<System.Object> lst = new List<object>();

                    //遍历所有工作表
                    for (int i = 1; i <= workSheets.Count; i++)
                    {
                        //当前工作表 
                        ExcelWorksheet sheet = workSheets[i];
                        //初始化集合
                        lst.Clear();
                        int columnCount = sheet.Dimension.End.Column;
                        int rowCount = sheet.Dimension.End.Row;
                        //根据实体类创建对象集合序列化到json中
                        for (int z = 4; z <= rowCount; z++)
                        {
                            Assembly ab = Assembly.Load("Assembly-CSharp"); //要注意对面在那个程序集里面dll
                            Type type = ab.GetType($"{sheet.Name}");
                            if (type == null)
                            {
                                Debug.LogError("你还没有创建对应的实体类!");
                                return;
                            }
                            if (!Directory.Exists(headPath))
                                Directory.CreateDirectory(headPath);
                            object o = ab.CreateInstance(type.ToString());
                            for (int j = 1; j <= columnCount; j++)
                            {
                                FieldInfo fieldInfo = type.GetField(sheet.Cells[1, j].Text); //先获得字段信息，方便获得字段类型          
                                object value = Convert.ChangeType(sheet.Cells[z, j].Text, fieldInfo.FieldType);
                                type.GetField(sheet.Cells[1, j].Text).SetValue(o, value);
                            }
                            lst.Add(o);
                        }
                        //写入json文件
                        string jsonPath = $"{headPath}/{sheet.Name}.json";
                        if (!File.Exists(jsonPath))
                        {
                            File.Create(jsonPath).Dispose();
                        }
                        File.WriteAllText(jsonPath, JsonConvert.SerializeObject(lst, Formatting.Indented));
                    }
                }
            }
            AssetDatabase.Refresh();
        }
    }
    void CreateEntities()
    {
        if (!string.IsNullOrEmpty(txt_ExcelName))
        {
            string filepath = Application.streamingAssetsPath + "/Excel/" + txt_ExcelName;
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                using (ExcelPackage ep = new ExcelPackage(fs))
                {
                    //获得所有工作表
                    ExcelWorksheets workSheets = ep.Workbook.Worksheets;
                    //遍历所有工作表
                    for (int i = 1; i <= workSheets.Count; i++)
                    {
                        CreateEntity(workSheets[i]);
                    }
                    AssetDatabase.Refresh();
                }
            }
        }
    }
    void CreateEntity(ExcelWorksheet sheet)
    {
        string dir = $"{Application.dataPath}/Scripts/{txt_EntitySavePath}";
        string path = $"{dir}/{sheet.Name}.cs";
        StringBuilder sb = new StringBuilder();
        //sb.AppendLine("namespace Entity");
        //sb.AppendLine("{");
        sb.AppendLine($"public class {sheet.Name}");
        sb.AppendLine("{");
        //遍历sheet首行每个字段描述的值
        Debug.Log("column = " + sheet.Dimension.End.Column);
        Debug.Log("row = " + sheet.Dimension.End.Row);
        for (int i = 1; i <= sheet.Dimension.End.Column; i++)
        {
            sb.AppendLine("\t/// <summary>");
            sb.AppendLine($"\t///{sheet.Cells[3, i].Text}");
            sb.AppendLine("\t/// </summary>");
            sb.AppendLine($"\tpublic {sheet.Cells[2, i].Text} {sheet.Cells[1, i].Text};");
        }
        sb.AppendLine("}");
        //sb.AppendLine("}");
        try
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(path))
            {
                File.Create(path).Dispose(); //避免资源占用
            }
            File.WriteAllText(path, sb.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Excel转json时创建对应的实体类出错，实体类为：{sheet.Name},e:{e.Message}");
        }
    }
}
