	

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
		if (index < 0 || index > News.Count) {
			News.Add(content);
		}
		else {
			News.Insert(index, content);
		}
		
	}
}


public class ImportantNewspaperModel: AbstractSavableModel {
	[ES3Serializable]
	private Dictionary<int, ImportantNewspaperInfo> importantNewspaperInfo =
		new Dictionary<int, ImportantNewspaperInfo>();
	
	
	[field: ES3Serializable]
	public DayOfWeek ImportantNewsPaperDay { get; set; }
	[field: ES3Serializable]
	public int NewspaperStartDay { get; set; }
	
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

	public int GetWeekForNews(int day) {
		//suppose newspaper start day is 3. So if day is 1-3 (inclusive), week is 1; if day is 4-10 (inclusive) , it is week 2 and so on
		if (day <= NewspaperStartDay) {
			return 1;
		}
		else {
			if((day - NewspaperStartDay) % 7 == 0) {
				return (day - NewspaperStartDay) / 7 + 1;
			}
			else {
				return (day - NewspaperStartDay) / 7 + 2;
			}
		}

	}
	
	public ImportantNewspaperInfo GetNewspaperInfo(int week) {
		if (!importantNewspaperInfo.ContainsKey(week)) {
			importantNewspaperInfo.Add(week, new ImportantNewspaperInfo(week));
		}
		return importantNewspaperInfo[week];
	}
	
}
