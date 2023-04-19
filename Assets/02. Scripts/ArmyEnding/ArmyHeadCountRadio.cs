using System;
using _02._Scripts.ArmyEnding;
using _02._Scripts.GameTime;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;


public class ArmyHeadCountRadio: RadioEvent<RadioTextContent> {
	[field: ES3Serializable] public override float TriggerChance { get; } = 1;
	
	[field: ES3Serializable]
	protected RadioTextContent radioContent { get; set; }
	
	[field: ES3Serializable]
	protected DateTime headCountEventTime { get; set; }

	protected override RadioTextContent GetRadioContent() {
		return radioContent;
	}
	protected override void SetRadioContent(RadioTextContent radioContent) {
		this.radioContent = radioContent;
	}
	
	public ArmyHeadCountRadio(TimeRange startTimeRange, AudioMixerGroup mixer) :
		base(startTimeRange, new RadioTextContent("", 1.1f, Gender.MALE, mixer),
			RadioChannel.FM108) {
		Debug.Log("A army head count radio is created at " + startTimeRange.StartTime);
		
		
	}
	
	private void SetContent() {
		GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
		DateTime eventTime = gameTimeModel.GetDay(gameTimeModel.Day + 1);
		headCountEventTime = new DateTime(eventTime.Year, eventTime.Month, eventTime.Day, 22, 50, 0);
		DateTime eventTimeAnnounced = headCountEventTime.AddMinutes(10);
		//get the time in "hour:minute" format
		string time = eventTimeAnnounced.ToString("HH:mm");


		string content =
			"Attention survivors, this is an important update from the military regarding the shelter. " +
			"Unfortunately, we won't be able to accommodate all of the Dorcha population due to limited space. " +
			$"However, we will be <color=yellow>conducting a headcount tomorrow night at around 11 PM until the midnight</color>, to select a batch of citizens. " +
			"You don't need to leave your homes. If you want to register, <color=yellow>simply flicker your porch lights outside your home</color> at the designated time, " +
			"and we'll know that you're interested in applying. Thank you for your cooperation and understanding, " +
			"and we wish you all the best in these challenging times.";

			this.radioContent.SetContent(content);
	}


	public ArmyHeadCountRadio() : base(){}
	
	public override EventState OnUpdate() {
		if (!started && (!electricityModel.HasElectricity() || !radioModel.IsOn ||
		                 radioModel.CurrentChannel != channel)) {
			return EventState.Missed;
		}
		return base.OnUpdate();
	}


	public override void OnEnd() {
		gameEventSystem.AddEvent(
			new HeadCountEvent(new TimeRange(headCountEventTime, headCountEventTime.AddMinutes(20))));
	}

	public override void OnMissed() {
		OnMissedOrPlayedWhenRadioOff();
	}

	private void OnMissedOrPlayedWhenRadioOff() {
		gameEventSystem.AddEvent(
			new ArmyHeadCountRadio(new TimeRange(this.StartTimeRange.StartTime.AddMinutes(5)),
				radioContent.mixer));
	}


	protected override void OnRadioStart() {
		SetContent();
	}

	protected override void OnPlayedWhenRadioOff() {
		OnMissedOrPlayedWhenRadioOff();
	}
}
