
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using MikroFramework.Architecture;
using Polyglot;
using UnityEngine;

public class ImportantNewsHotUpdateInfo {
	public string Key;
	public List<string> Titles;
	public List<string> SubTitles;
	public List<string> Contents;
	public List<int> ImageIndices;

	public ImportantNewsHotUpdateInfo(string key) {
		Key = key;
		Titles = new List<string>();
		Contents = new List<string>();
		ImageIndices = new List<int>();
		SubTitles = new List<string>();
	}
}

public class ImportantNewsTextInfo: IImportantNewspaperPageContent {
	public string Key;
	public string Title;
	public string Content;
	public int ImageIndex;
	public string SubTitle;
	
	public ImportantNewsTextInfo(string key, string title, string content, int imageIndex, string subtitle) {
		Key = key;
		Title = title;
		Content = content;
		ImageIndex = imageIndex;
		SubTitle = subtitle;
	}

	public List<IImportantNewspaperPageContent> GetPages() {
		return new List<IImportantNewspaperPageContent>() {this};
	}
}

public class ImportantNewsTextModel: AbstractModel {
	private Dictionary<string, ImportantNewsHotUpdateInfo> hotUpdateInfo =
		new Dictionary<string, ImportantNewsHotUpdateInfo>();
	
	

	protected override void OnInit() {
		LoadInfo();
	}

	public ImportantNewsTextInfo GetInfo(string key, int titleIndex, int contentIndex, int imageIndex, int subtitleIndex) {
		if (hotUpdateInfo.ContainsKey(key)) {
			var info = hotUpdateInfo[key];
			int realImageIndex = imageIndex < 0 ? -1 : info.ImageIndices[imageIndex];
			return new ImportantNewsTextInfo(key, info.Titles[titleIndex], info.Contents[contentIndex],
				realImageIndex, info.SubTitles[subtitleIndex]);
		}
		return null;
	}
	
	public ImportantNewsTextInfo GetInfo(string key) {
		key = key.ToLower();
		if (hotUpdateInfo.ContainsKey(key)) {
			var info = hotUpdateInfo[key];
			int titleIndex = Random.Range(0, info.Titles.Count);
			int contentIndex = Random.Range(0, info.Contents.Count);
			int imageIndex = -1;
			if (info.ImageIndices.Count > 0) {
				imageIndex = Random.Range(0, info.ImageIndices.Count);
				imageIndex = info.ImageIndices[imageIndex];
			}
			int subtitleIndex = Random.Range(0, info.SubTitles.Count);
			return new ImportantNewsTextInfo(key, info.Titles[titleIndex], info.Contents[contentIndex],
				imageIndex, info.SubTitles[subtitleIndex]);
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
						currentTagDescription.SubTitles.Add(row[2]);
					}
					
					if (!string.IsNullOrEmpty(row[3])) {
						currentTagDescription.Contents.Add(row[3]);
					}
					
					if (!string.IsNullOrEmpty(row[4])) {
						currentTagDescription.ImageIndices.Add(int.Parse(row[4]));
					}
				}
			}

		}
            
	}
}
