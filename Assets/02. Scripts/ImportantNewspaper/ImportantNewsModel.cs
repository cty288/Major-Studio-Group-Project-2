
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using MikroFramework.Architecture;
using Polyglot;
using UnityEngine;

public class ImportantNewsHotUpdateInfo {
	public string Key;
	public List<string> Titles;
	public List<string> Contents;

	public ImportantNewsHotUpdateInfo(string key) {
		Key = key;
		Titles = new List<string>();
		Contents = new List<string>();
	}
}

public class ImportantNewsInfo {
	public string Key;
	public string Title;
	public string Content;
	
	public ImportantNewsInfo(string key, string title, string content) {
		Key = key;
		Title = title;
		Content = content;
	}
}

public class ImportantNewsModel: AbstractModel {
	private Dictionary<string, ImportantNewsHotUpdateInfo> hotUpdateInfo =
		new Dictionary<string, ImportantNewsHotUpdateInfo>();

	protected override void OnInit() {
		LoadInfo();
	}

	public ImportantNewsInfo GetInfo(string key, int titleIndex, int contentIndex) {
		if (hotUpdateInfo.ContainsKey(key)) {
			var info = hotUpdateInfo[key];
			return new ImportantNewsInfo(key, info.Titles[titleIndex], info.Contents[contentIndex]);
		}
		return null;
	}
	
	public ImportantNewsInfo GetInfo(string key) {
		if (hotUpdateInfo.ContainsKey(key)) {
			var info = hotUpdateInfo[key];
			int titleIndex = Random.Range(0, info.Titles.Count);
			int contentIndex = Random.Range(0, info.Contents.Count);
			return new ImportantNewsInfo(key, info.Titles[titleIndex], info.Contents[contentIndex]);
		}
		return null;
	}
	
	

	public void LoadInfo() {
		string result = "";
		var enumerator = GoogleDownload.DownloadSheet("1lmnHrIwzQdimzbfLRgKDrLi43lfnCQKSiukUL9kffQo",
			"24778915", s => {
				result = s;
			}, GoogleDriveDownloadFormat.CSV);
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

              
			int startRow = 1;
			string currentTag = rows[startRow][0].ToLower();
			ImportantNewsHotUpdateInfo currentTagDescription = new ImportantNewsHotUpdateInfo(currentTag);
			
			hotUpdateInfo.Add(currentTag, currentTagDescription);
                
			for (int i = startRow; i < rows.Count; i++) {
				List<string> row = rows[i];
				if (row.Count > 0) {
					if (!string.IsNullOrEmpty(row[0].ToLower()) && row[0].ToLower() != currentTag) {
						currentTag = row[0].ToLower();
						currentTagDescription = new ImportantNewsHotUpdateInfo(currentTag);
						hotUpdateInfo.Add(currentTag, currentTagDescription);
					}
					if (!string.IsNullOrEmpty(row[1])) {
						currentTagDescription.Titles.Add(row[1]);
					}
					if (!string.IsNullOrEmpty(row[2])) {
						currentTagDescription.Contents.Add(row[2]);
					}
				}
			}

		}
            
	}
}
