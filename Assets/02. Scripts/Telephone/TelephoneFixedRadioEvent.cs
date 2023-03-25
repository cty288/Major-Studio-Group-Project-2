using System;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace _02._Scripts.Telephone {
	public class TelephoneFixedRadioEvent: RadioEvent<RadioTextContent> {
		
		[field: ES3Serializable]
		protected override RadioTextContent radioContent { get; set; }
		
		public TelephoneFixedRadioEvent(TimeRange startTimeRange, AudioMixerGroup mixer) :
			base(startTimeRange, new RadioTextContent("", 1.2f, Gender.MALE, mixer),
				RadioChannel.AllChannels) {
			this.radioContent.SetContent(this.GetModel<HotUpdateDataModel>().GetData("TelepohoneFixedRadio").values[0]);
		}
         
		
		
		
		public TelephoneFixedRadioEvent(): base(){}
		[field: ES3Serializable]
		public override float TriggerChance { get; } = 1;
		public override void OnEnd() {
        
		}

		public override void OnMissed() {
			DateTime currentTime = gameTimeManager.CurrentTime.Value;
			int nextEventInterval = Random.Range(8,15);


			gameEventSystem.AddEvent(new TelephoneFixedRadioEvent(
				new TimeRange(currentTime + new TimeSpan(0, nextEventInterval, 0),
					currentTime + new TimeSpan(0, nextEventInterval + 10, 0)), radioContent.mixer));
		}
    

		protected override void OnRadioStart() {
        
		}

		protected override void OnPlayedWhenRadioOff() {
			OnMissed();
		}
	}
}