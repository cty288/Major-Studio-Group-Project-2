using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Radio.RadioScheduling;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;

public class RadioScheduleProgramContentUI : AbstractMikroController<MainGame> {
	private TMP_Text timeText;
	private TMP_Text programNameText;

	private void Awake() {
		timeText = transform.Find("Time").GetComponent<TMP_Text>();
		programNameText = transform.Find("ProgramName").GetComponent<TMP_Text>();
	}

	public void SetContent(RadioScheduleInfo programInfo) {
		Awake();
		TimeRange timeRange = programInfo.ActualTimeRange;
		DateTime startTime = timeRange.StartTime;
		DateTime endTime = timeRange.EndTime;
		
        timeText.text = startTime.ToString("HH:mm") + "~" + endTime.ToString("HH:mm");
        programNameText.text = programInfo.NameInfo.DisplayName;
	}
}
