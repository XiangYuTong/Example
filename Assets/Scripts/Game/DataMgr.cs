using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
					Debug.LogError("场景中没有DataMgr组件，已经自动生成");
					GameObject go = new GameObject(nameof(DataMgr));
					go.transform.parent = GameManager.instance.transform;
					_instance = go.AddComponent<DataMgr>();
				}
				else
				{
					 _instance = ins;
				}
			}
			return _instance;
		}
	}
	//表格：data_0
	public List<MainData> data_0_MainData_list;
	//表格：data_1
	public List<MainData> data_1_MainData_list;
	public void Init(){	

		data_0_MainData_list = ExcelTool.ReadExcel<MainData>(0,0);
		data_1_MainData_list = ExcelTool.ReadExcel<MainData>(0,1);
	}

	public void SaveAll(JsonType type = JsonType.LitJson){

	}
	public void Save(object data, string fileName,JsonType type = JsonType.LitJson){

		JsonManager.Instance.SaveData(data,fileName,type);
	}
}
