	

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

	public DayOfWeek[] newsDays = new DayOfWeek[] { DayOfWeek.Wednesday, DayOfWeek.Sunday};
	
	
	[field: ES3Serializable]
	public int NewspaperStartDay { get; set; }
	
	public void AddPageToNewspaper(int issue, IImportantNewspaperPageContent page) {
		if (!importantNewspaperInfo.ContainsKey(issue)) {
			importantNewspaperInfo.Add(issue, new ImportantNewspaperInfo(issue));
		}
		foreach (IImportantNewspaperPageContent importantNewspaperPageContent in page.GetPages()) {
			importantNewspaperInfo[issue].Add(importantNewspaperPageContent);
		}
	}

	public bool HasPageOfID(int issue, string ID) {
		if(!importantNewspaperInfo.ContainsKey(issue)) return false;
		
		foreach (IImportantNewspaperPageContent importantNewspaperPageContent in importantNewspaperInfo[issue].News) {
			if (importantNewspaperPageContent.ID == ID) {
				return true;
			}
		}
		return false;
	}
	
	public void RemoveAllPagesOfID(int issue, string ID) {
		if(!importantNewspaperInfo.ContainsKey(issue)) return;
		
		for (int i = importantNewspaperInfo[issue].News.Count - 1; i >= 0; i--) {
			if (importantNewspaperInfo[issue].News[i].ID == ID) {
				importantNewspaperInfo[issue].News.RemoveAt(i);
			}
		}
	}
	
	public void AddPageToNewspaper(int issue, IImportantNewspaperPageContent page, int index) {
		if (page == null) {
			return;
			;
		}
		if (!importantNewspaperInfo.ContainsKey(issue)) {
			importantNewspaperInfo.Add(issue, new ImportantNewspaperInfo(issue));
		}
		
		if(index >= importantNewspaperInfo[issue].News.Count) {
			importantNewspaperInfo[issue].AddRange(page.GetPages());
		}
		else if (index <= 0) {
			importantNewspaperInfo[issue].AddRange(page.GetPages(), 0);
		} else {
			//when we insert a page, we need to make sure that the page before it is not a page that has multiple pages
			//if it is, we need to insert the page after the last page of the page that has the same ID
			IImportantNewspaperPageContent originalPage = importantNewspaperInfo[issue].News[index];
			IImportantNewspaperPageContent originalPageBefore = importantNewspaperInfo[issue].News[index - 1];
			if (originalPage.ID == originalPageBefore.ID) {
				//we need to find the last page of the original page
				int lastIndexOfOriginalPage = index;
				for (int i = index; i < importantNewspaperInfo[issue].News.Count; i++) {
					if (importantNewspaperInfo[issue].News[i].ID != originalPage.ID) {
						break;
					}
					lastIndexOfOriginalPage = i;
				}
				importantNewspaperInfo[issue].AddRange(page.GetPages(), lastIndexOfOriginalPage + 1);
			}
			else {
				importantNewspaperInfo[issue].AddRange(page.GetPages(), index);
			}


		}
		
		
	}

	public int GetIssueForNews(int day, DateTime date) {
		//suppose newspaper start day is 3. So if day is 1-3 (inclusive), week is 1; if day is 4-10 (inclusive) , it is week 2 and so on
		if (day <= NewspaperStartDay) {
			return 1;
		}
		else {
			if (newsDays.Length <= 1) {
				if((day - NewspaperStartDay) % 7 == 0) {
					return (day - NewspaperStartDay) / 7 + 1;
				}
				else {
					return (day - NewspaperStartDay) / 7 + 2;
				}
			}
			else {
				//TODO: FIX
				int week = (day - NewspaperStartDay) / 7;
				int dayofWeek = (int) date.DayOfWeek;
				if(dayofWeek >= (int) newsDays[0]) {
					return week*2 + 1;
				}
				else {
					return week * 2 + 2;
				}
			}
			
		}

	}
	
	public ImportantNewspaperInfo GetNewspaperInfo(int issue) {
		if (!importantNewspaperInfo.ContainsKey(issue)) {
			importantNewspaperInfo.Add(issue, new ImportantNewspaperInfo(issue));
		}
		return importantNewspaperInfo[issue];
	}
	
}
