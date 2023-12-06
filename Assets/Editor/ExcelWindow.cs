/* һ��xlsx�����ж��ű����Ϊʵ����Ϸ��ʹ�õ���json�ļ����������ɵ�ʱ����ֻ��һ��xlsx
 * һ��sheet����һ�����ݼ��� 
 * ����sheet���ֶ����ɶ�Ӧ�����ݼ���json
 * ����json�ļ�ͬʱ����ʵ����
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
    private string txt_ExcelName = "data.xlsx"; //�������
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
        GUILayout.Label("�������: ");
        txt_ExcelName = GUILayout.TextField(txt_ExcelName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Json����·��: ");
        txt_JsonSavePath = GUILayout.TextField(txt_JsonSavePath);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("����ʵ����"))
        {
            CreateEntities();
        }
        if (GUILayout.Button("����Json"))
        {
            ExcelToJson();
        }
        if (GUILayout.Button("����Json���ݶ�ȡ������"))
        {
            CreateJsonDataMgr();
        }
        if (GUILayout.Button("����Excel���ݶ�ȡ������"))
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
            sb.AppendLine("\t\t\t\t\tDebug.LogError(\"������û��DataMgr����������\");");
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
                //Debug.LogError("ָ�����ļ��в�����: " + folderPath);
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
                //Debug.LogError("ָ�����ļ��в�����: " + folderPath);
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
                //Debug.LogError("ָ�����ļ��в�����: " + folderPath);
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
                    File.Create(path).Dispose(); //������Դռ��
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
                    //������й�����
                    ExcelWorksheets workSheets = ep.Workbook.Worksheets;
                    //�������й�����
                    

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
                    sb.AppendLine("\t\t\t\t\tDebug.LogError(\"������û��DataMgr����������\");");
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
                            File.Create(path).Dispose(); //������Դռ��
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
                    //������й�����
                    ExcelWorksheets workSheets = ep.Workbook.Worksheets;
                    List<System.Object> lst = new List<object>();

                    //�������й�����
                    for (int i = 1; i <= workSheets.Count; i++)
                    {
                        //��ǰ������ 
                        ExcelWorksheet sheet = workSheets[i];
                        //��ʼ������
                        lst.Clear();
                        int columnCount = sheet.Dimension.End.Column;
                        int rowCount = sheet.Dimension.End.Row;
                        //����ʵ���ഴ�����󼯺����л���json��
                        for (int z = 4; z <= rowCount; z++)
                        {
                            Assembly ab = Assembly.Load("Assembly-CSharp"); //Ҫע��������Ǹ���������dll
                            Type type = ab.GetType($"{sheet.Name}");
                            if (type == null)
                            {
                                Debug.LogError("�㻹û�д�����Ӧ��ʵ����!");
                                return;
                            }
                            if (!Directory.Exists(headPath))
                                Directory.CreateDirectory(headPath);
                            object o = ab.CreateInstance(type.ToString());
                            for (int j = 1; j <= columnCount; j++)
                            {
                                FieldInfo fieldInfo = type.GetField(sheet.Cells[1, j].Text); //�Ȼ���ֶ���Ϣ���������ֶ�����          
                                object value = Convert.ChangeType(sheet.Cells[z, j].Text, fieldInfo.FieldType);
                                type.GetField(sheet.Cells[1, j].Text).SetValue(o, value);
                            }
                            lst.Add(o);
                        }
                        //д��json�ļ�
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
                    //������й�����
                    ExcelWorksheets workSheets = ep.Workbook.Worksheets;
                    //�������й�����
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
        //����sheet����ÿ���ֶ�������ֵ
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
                File.Create(path).Dispose(); //������Դռ��
            }
            File.WriteAllText(path, sb.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Excelתjsonʱ������Ӧ��ʵ�������ʵ����Ϊ��{sheet.Name},e:{e.Message}");
        }
    }
}
