using System;
using _02._Scripts.ArmyEnding.InitialPhoneCalls;
using _02._Scripts.GameTime;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;

namespace _02._Scripts.ArmyEnding {
	public class ShelterPrologueRadio : RadioEvent<RadioTextContent> {
		
		[field: ES3Serializable]
		protected RadioTextContent radioContent { get; set; }

		protected override RadioTextContent GetRadioContent() {
			return radioContent;
		}
		protected override void SetRadioContent(RadioTextContent radioContent) {
			this.radioContent = radioContent;
		}

		[field: ES3Serializable]
		public override float TriggerChance { get; } = 1f;
		
		[field: ES3Serializable]
		protected DateTime shelterConstructionFinishedTime { get; set; }
		
		
		
		public ShelterPrologueRadio(TimeRange startTimeRange, AudioMixerGroup mixer) :
			base(startTimeRange, new RadioTextContent("", 1.1f, Gender.MALE, mixer),
				RadioChannel.FM108) {
			Debug.Log("A shelter prologue radio is created at " + startTimeRange.StartTime);
			
			
		}

		public ShelterPrologueRadio() : base(){}

		public override EventState OnUpdate() {
			if (!started && (!electricityModel.HasElectricity() || !radioModel.IsOn ||
			                 radioModel.CurrentChannel != channel)) {
				return EventState.Missed;
			}
			return base.OnUpdate();
		}

		private void SetContent() {
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			shelterConstructionFinishedTime = gameTimeModel.GetDay(gameTimeModel.Day + 5);
			//get the time in "Month/Day" format like "12/25"
			string timeInMonthDayFormat = shelterConstructionFinishedTime.ToString("MM/dd");
			string content =
				"Attention Dorcha citizens, this is an important announcement from the <color=yellow>military</color>. We're want to inform you that <color=yellow>a shelter is currently under construction and expected to be completed next week</color>. " +
				$"If you're interested in seeking refuge, please tune in back to <color=yellow>FM108 on {timeInMonthDayFormat}</color>," +
				" where we'll provide further details and instructions on how to register. Thank you for your attention, and stay safe out there.";

			
			
			this.radioContent.SetContent(content);
		}


		
		
		public override void OnEnd() {
			gameEventSystem.AddEvent(new ArmyHeadCountRadio(new TimeRange(shelterConstructionFinishedTime),
				radioContent.mixer));
		}

		public override void OnMissed() {
			OnMissedOrPlayedWhenRadioOff();
		}


		protected override void OnRadioStart() {
			SetContent();
		}

		protected override void OnPlayedWhenRadioOff() {
			OnMissedOrPlayedWhenRadioOff();
		}
		
		
		protected void OnMissedOrPlayedWhenRadioOff() {
			gameEventSystem.AddEvent(
				new ShelterPrologueRadio(new TimeRange(this.StartTimeRange.StartTime.AddMinutes(5)),
					radioContent.mixer));
		}
	}
}