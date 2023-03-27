using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Radio.RadioScheduling;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;

public class RadioScheduleDayContentUI : AbstractMikroController<MainGame> {
    [SerializeField] private GameObject channelContentPrefab;

    private TMP_Text dayText;
    private Transform channelListContent;
    private void Awake() {
        dayText = transform.Find("DayText").GetComponent<TMP_Text>();
        channelListContent = transform.Find("ChannelList");
    }

    public void SetContent(Dictionary<RadioChannel, List<RadioScheduleInfo>> daySchedule, DateTime date) {
        Awake();
        
        dayText.text = CalenderViewController.GetMonthAbbreviation(date.Month) + " " + date.Day + ":";

        int index = 1;
        foreach (RadioChannel radioChannel in daySchedule.Keys) {
            List<RadioScheduleInfo> radioScheduleInfos = daySchedule[radioChannel];
            RadioScheduleChannelContentUI radioScheduleChannelContentUI =
                Instantiate(channelContentPrefab, channelListContent)
                    .GetComponent<RadioScheduleChannelContentUI>();
            radioScheduleChannelContentUI.SetContent(radioScheduleInfos, radioChannel, index);
            index++;
        }
    }
}
