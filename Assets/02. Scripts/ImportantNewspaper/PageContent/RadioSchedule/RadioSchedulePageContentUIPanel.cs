using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Radio.RadioScheduling;
using TMPro;
using UnityEngine;

public class RadioSchedulePageContentUIPanel : ImportantNewspaperPageContentUIPanel {
	private Dictionary<DateTime, Dictionary<RadioChannel, List<RadioScheduleInfo>>> schedule;
	private TMP_Text weekText;
	private Transform dayContentList;
	[SerializeField] private GameObject dayContentPrefab;
	private void Awake() {
		weekText = transform.Find("WeekText").GetComponent<TMP_Text>();
		dayContentList = transform.Find("ContentArea/DayList");
	}

	public override void OnSetContent(IImportantNewspaperPageContent content, int weekCount) {
		Awake();
		
		ImportantNewspaperRadioSchedulePage radioSchedulePage = content as ImportantNewspaperRadioSchedulePage;
		this.schedule = radioSchedulePage.Schedule;

		weekText.text = $"Issue {weekCount}";
		foreach (DateTime date in schedule.Keys) {
			Dictionary<RadioChannel, List<RadioScheduleInfo>> daySchedule = schedule[date];
			RadioScheduleDayContentUI dayContent =
				Instantiate(dayContentPrefab, dayContentList).GetComponent<RadioScheduleDayContentUI>();
			dayContent.SetContent(daySchedule, date);
		}
	}
}
