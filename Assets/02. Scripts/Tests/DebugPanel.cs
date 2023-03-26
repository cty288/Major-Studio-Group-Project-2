using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameEvents.BountyHunter;
using _02._Scripts.GameEvents.Merchant;
using _02._Scripts.Radio.RadioScheduling;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;

public class DebugPanel : AbstractMikroController<MainGame> {
	[SerializeField] private TMP_Text bountyHunterNumber;
	[SerializeField] private TMP_Text merchantNumber;
	[SerializeField] private TMP_Text radioTimeText;
	private void Awake() {
		this.RegisterEvent<OnWeeklyRadioScheduleGenerated>(OnWeeklyRadioScheduleGenerated).UnRegisterWhenGameObjectDestroyed(gameObject);
	}

	private void OnWeeklyRadioScheduleGenerated(OnWeeklyRadioScheduleGenerated e) {
		string text = "";
		RadioModel radioModel = this.GetModel<RadioModel>();
		foreach (DateTime date in e.Schedule.Keys) {
			text += date.ToString("MM/dd") + "\t";
			Dictionary<RadioChannel, List<RadioScheduleInfo>> radioScheduleInfos = e.Schedule[date];
			
			
			foreach (RadioChannel radioChannel in radioScheduleInfos.Keys) {
				List<RadioScheduleInfo> radioScheduleInfoList = radioScheduleInfos[radioChannel];
				
				text += "\t" + radioModel.GetRadioChannelInfo(radioChannel).ChannelName + ": \n";
				foreach (RadioScheduleInfo radioScheduleInfo in radioScheduleInfoList) {
					DateTime startTime = radioScheduleInfo.ActualTimeRange.StartTime;
					DateTime endTime = radioScheduleInfo.ActualTimeRange.EndTime;
					string programName = radioScheduleInfo.NameInfo.DisplayName;
					text += "\t\t" + startTime.ToString("HH:mm") + "-" + endTime.ToString("HH:mm") + "\t\t" + programName + "\n";
				}
			}
			
			text += "\n";
		}
		
		radioTimeText.text = text;
	}

	private void Start() {
		bountyHunterNumber.text = "Bounty Hunter Number: " + this.GetModel<BountyHunterModel>().PhoneNumber;
		merchantNumber.text = "Merchant Number: " + this.GetModel<MerchantModel>().PhoneNumber;
	}
}
