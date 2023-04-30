using System.Collections;
using System.Collections.Generic;
using _02._Scripts;
using MikroFramework.Architecture;
using Polyglot;
using UnityEngine;

public class HotUpdateData {
	public string Key;
	public string[] values;
	
	public HotUpdateData(string key, string[] values) {
		this.Key = key;
		this.values = values;
	}
}
public class HotUpdateDataModel : AbstractModel {
	protected Dictionary<string, HotUpdateData> data = new Dictionary<string, HotUpdateData>();
	protected override void OnInit() {
		LoadInfo();
	}

	public void LoadInfo() {
		string result = "";
		var enumerator = HotUpdateLoader.LoadOrDownload("1lmnHrIwzQdimzbfLRgKDrLi43lfnCQKSiukUL9kffQo",
			"1293010699", "hot_update.csv" ,s => {
				result = s;
			});
		while (enumerator.MoveNext()) {
				
		}
		//Debug.Log("result: " + result);
		OnDownloadSheet(result);
	}
		
		
	public  void OnDownloadSheet(string text) {
		if (!string.IsNullOrEmpty(text)) {
			List<List<string>> rows;
			text = text.Replace("\r\n", "\n");
			rows = CsvReader.Parse(text);

			int argNum = 5;
			int startRow = 1;
			string currentTag = rows[startRow][0];
			string[] currentValues = new string[argNum];
			for (int j = 0; j < argNum; j++) {
				currentValues[j] = rows[startRow][j+1];
			}



			HotUpdateData data = new HotUpdateData(currentTag, currentValues);
			this.data.Add(currentTag, data);
                
                

			for (int i = startRow; i < rows.Count; i++) {
				List<string> row = rows[i];
				if (row.Count > 0) {
					if (!string.IsNullOrEmpty(row[0]) && row[0] != currentTag) {
						currentTag = row[0];
						currentValues = new string[argNum];
						for (int j = 0; j < argNum; j++) {
							currentValues[j] = row[j+1];
						}
						data = new HotUpdateData(currentTag, currentValues);
						this.data.Add(currentTag, data);
					}
				}
			}

		}
	}
	
	public HotUpdateData GetData(string key) {
		if (data.ContainsKey(key)) {
			return data[key];
		}
		else {
			return null;
		}
	}
	
}
