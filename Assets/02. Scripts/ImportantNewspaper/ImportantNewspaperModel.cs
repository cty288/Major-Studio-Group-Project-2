	

using System;
using System.Collections.Generic;

public interface IImportantNewspaperPageContent {
	public List<IImportantNewspaperPageContent> GetPages();
	public string ID { get; }
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
	
	public void AddRange(IEnumerable<IImportantNewspaperPageContent> content) {
		News.AddRange(content);
	}
	
	public void Add(IImportantNewspaperPageContent content, int index) {
		if (index < 0 || index > News.Count) {
			News.Add(content);
		}
		else {
			News.Insert(index, content);
		}
		
	}
	
	public void AddRange(IEnumerable<IImportantNewspaperPageContent> content, int index) {
		if (index < 0 || index > News.Count) {
			News.AddRange(content);
		}
		else {
			News.InsertRange(index, content);
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

	public bool HasPageOfID(int week, string ID) {
		if(!importantNewspaperInfo.ContainsKey(week)) return false;
		
		foreach (IImportantNewspaperPageContent importantNewspaperPageContent in importantNewspaperInfo[week].News) {
			if (importantNewspaperPageContent.ID == ID) {
				return true;
			}
		}
		return false;
	}
	
	public void RemoveAllPagesOfID(int week, string ID) {
		if(!importantNewspaperInfo.ContainsKey(week)) return;
		
		for (int i = importantNewspaperInfo[week].News.Count - 1; i >= 0; i--) {
			if (importantNewspaperInfo[week].News[i].ID == ID) {
				importantNewspaperInfo[week].News.RemoveAt(i);
			}
		}
	}
	
	public void AddPageToNewspaper(int week, IImportantNewspaperPageContent page, int index) {
		if (!importantNewspaperInfo.ContainsKey(week)) {
			importantNewspaperInfo.Add(week, new ImportantNewspaperInfo(week));
		}
		
		if(index >= importantNewspaperInfo[week].News.Count) {
			importantNewspaperInfo[week].AddRange(page.GetPages());
		}
		else if (index <= 0) {
			importantNewspaperInfo[week].AddRange(page.GetPages(), 0);
		} else {
			//when we insert a page, we need to make sure that the page before it is not a page that has multiple pages
			//if it is, we need to insert the page after the last page of the page that has the same ID
			IImportantNewspaperPageContent originalPage = importantNewspaperInfo[week].News[index];
			IImportantNewspaperPageContent originalPageBefore = importantNewspaperInfo[week].News[index - 1];
			if (originalPage.ID == originalPageBefore.ID) {
				//we need to find the last page of the original page
				int lastIndexOfOriginalPage = index;
				for (int i = index; i < importantNewspaperInfo[week].News.Count; i++) {
					if (importantNewspaperInfo[week].News[i].ID != originalPage.ID) {
						break;
					}
					lastIndexOfOriginalPage = i;
				}
				importantNewspaperInfo[week].AddRange(page.GetPages(), lastIndexOfOriginalPage + 1);
			}
			else {
				importantNewspaperInfo[week].AddRange(page.GetPages(), index);
			}


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
