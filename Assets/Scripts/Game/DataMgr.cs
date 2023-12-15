using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataMgr : MonoBehaviour
{
	private static DataMgr _instance;
	public static DataMgr Instance
	{
		get
		{
			 if (_instance == null)
			{
				DataMgr ins = FindObjectOfType<DataMgr>();
				if (ins == null)
				{
					Debug.LogError("场景中没有DataMgr组件，请添加");
				}
				else
				{
					 _instance = ins;
				}
			}
			return _instance;
		}
	}
	public List<Sheet1> Sheet1_list;
	public void Init(){	

		//Sheet1_list = ExcelTool.ReadExcel<Sheet1>(0);
	}

	public void SaveAll(JsonType type = JsonType.LitJson){

	}
	public void Save(object data, string fileName,JsonType type = JsonType.LitJson){

		//JsonManager.Instance.SaveData(data,fileName,type);
	}
}
