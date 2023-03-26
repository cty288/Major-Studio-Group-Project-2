	

using System;
using System.Collections.Generic;

public interface IImportantNewspaperPageContent {
	public List<IImportantNewspaperPageContent> GetPages();
}
public class ImportantNewspaperInfo {
	public int Week;
	public List<IImportantNewspaperPageContent> News;
	
	public ImportantNewspaperInfo(int week) {
		Week = week;
		News = new List<IImportantNewspaperPageContent>();
	}

	public ImportantNewspaperInfo() {
		News = new List<IImportantNewspaperPageContent>();
	}
	
	public void Add(IImportantNewspaperPageContent content) {
		News.Add(content);
	}
	
	public void Add(IImportantNewspaperPageContent content, int index) {
		News.Insert(index, content);
	}
}


public class ImportantNewspaperModel: AbstractSavableModel {
	[ES3Serializable]
	private Dictionary<int, ImportantNewspaperInfo> importantNewspaperInfo =
		new Dictionary<int, ImportantNewspaperInfo>();
	
	
	[field: ES3Serializable]
	public DayOfWeek ImportantNewsPaperDay { get; set; }
	
	public void AddPageToNewspaper(int week, IImportantNewspaperPageContent page) {
		if (!importantNewspaperInfo.ContainsKey(week)) {
			importantNewspaperInfo.Add(week, new ImportantNewspaperInfo(week));
		}
		foreach (IImportantNewspaperPageContent importantNewspaperPageContent in page.GetPages()) {
			importantNewspaperInfo[week].Add(importantNewspaperPageContent);
		}
	}
	
	public void AddPageToNewspaper(int week, IImportantNewspaperPageContent page, int index) {
		if (!importantNewspaperInfo.ContainsKey(week)) {
			importantNewspaperInfo.Add(week, new ImportantNewspaperInfo(week));
		}
		
		int indexCounter = 0;
		foreach (IImportantNewspaperPageContent importantNewspaperPageContent in page.GetPages()) {
			importantNewspaperInfo[week].Add(importantNewspaperPageContent, index + indexCounter);
			indexCounter++;
		}
		
		
	}
	
	public ImportantNewspaperInfo GetNewspaperInfo(int week) {
		if (!importantNewspaperInfo.ContainsKey(week)) {
			importantNewspaperInfo.Add(week, new ImportantNewspaperInfo(week));
		}
		return importantNewspaperInfo[week];
	}
	
}
