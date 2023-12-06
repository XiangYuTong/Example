using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using Excel;
using UnityEngine;

public class ExcelTool : MonoBehaviour
{
    private void Awake()
    {

    }
    void Start()
    {

    }

    public static List<T> ReadExcel<T>(int sheetIndex)
    {
        FileStream stream;
        IExcelDataReader excelReader;
        if (File.Exists(Application.streamingAssetsPath + "/Excel/data.xlsx"))
        {
            stream = File.Open(Application.streamingAssetsPath + "/Excel/data.xlsx", FileMode.Open, FileAccess.Read, FileShare.Read);
            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        }
        else if (File.Exists(Application.streamingAssetsPath + "/Excel/data.xls"))
        {
            stream = File.Open(Application.streamingAssetsPath + "/Excel/data.xls", FileMode.Open, FileAccess.Read, FileShare.Read);
            excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
        }
        else
        {
            stream = File.Open(Application.streamingAssetsPath + "/Excel/data.xlsm", FileMode.Open, FileAccess.Read, FileShare.Read);
            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        }

        DataSet result = excelReader.AsDataSet();
        List<T> list = new List<T>();

        if (stream != null)
        {
            stream.Close();
        }

        int[] counts = GetCount(result.Tables[sheetIndex]);

        int rows = counts[0];
        int columns = counts[1];
        Debug.LogError("row:" + rows + "...col:" + columns);
        for (int i = 3; i < rows; i++)
        {
            Type contaninerType = typeof(T);
            FieldInfo[] infos = contaninerType.GetFields();
            object contaninerObj = Activator.CreateInstance(contaninerType);
            for (int j = 0; j < infos.Length; j++)
            {
                string content = result.Tables[sheetIndex].Rows[i][j].ToString();
                if (content == "")
                {
                    content = null;
                    continue;
                }
                if (infos[j].FieldType == typeof(int))
                {
                    //相当于就是把2进制数据转为int 然后赋值给了对应的字段
                    infos[j].SetValue(contaninerObj, Int16.Parse(content));

                }
                else if (infos[j].FieldType == typeof(float))
                {
                    infos[j].SetValue(contaninerObj, float.Parse(content));

                }
                else if (infos[j].FieldType == typeof(bool))
                {
                    if (content == "是")
                    {
                        infos[j].SetValue(contaninerObj, true);
                    }
                    else if (content == "否")
                    {
                        infos[j].SetValue(contaninerObj, false);
                    }


                }
                else if (infos[j].FieldType == typeof(string))
                {
                    //读取字符串字节数组的长度

                    infos[j].SetValue(contaninerObj, content);

                }
                //Debug.LogError(content);
                //Debug.LogError(result.Tables[0].Rows[i][j].ToString());
            }
            int key = Int16.Parse(result.Tables[sheetIndex].Rows[i][0].ToString());
            list.Add((T)contaninerObj);
        }


        return list;
    }

    private static int[] GetCount(DataTable dt)
    {
        int i = dt.Rows.Count;
        for (int m = 0; m < dt.Rows.Count; m++)
        {
            if (string.IsNullOrEmpty(dt.Rows[m][0].ToString()))
            {
                i = m;
                break;
            }
        }

        int j = dt.Columns.Count;
        for (int n = 0; n < dt.Columns.Count; n++)
        {
            if (string.IsNullOrEmpty(dt.Rows[0][n].ToString()))
            {
                j = n;
                break;
            }
        }
        return new int[] { i, j };
    }
}

