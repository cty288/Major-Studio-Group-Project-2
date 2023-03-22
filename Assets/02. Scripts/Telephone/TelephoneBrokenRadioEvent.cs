using System;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace _02._Scripts.Telephone {
	public class TelephoneBrokenRadioEvent: RadioEvent {
		
		public TelephoneBrokenRadioEvent(TimeRange startTimeRange, AudioMixerGroup mixer) :
			base(startTimeRange, "", 1.2f, Gender.MALE, mixer,
				RadioChannel.AllChannels) {
			this.speakContent = this.GetModel<HotUpdateDataModel>().GetData("TelepohoneBrokenRadio").values[0];
		}
         
		
		
		
		public TelephoneBrokenRadioEvent(): base(){}
		[field: ES3Serializable]
		public override float TriggerChance { get; } = 1;
		public override void OnEnd() {
        
		}

		public override void OnMissed() {
			DateTime currentTime = gameTimeManager.CurrentTime.Value;
			int nextEventInterval = Random.Range(8,15);


			gameEventSystem.AddEvent(new TelephoneBrokenRadioEvent(
				new TimeRange(currentTime + new TimeSpan(0, nextEventInterval, 0),
					currentTime + new TimeSpan(0, nextEventInterval + 10, 0)), mixer));
		}
    

		protected override void OnRadioStart() {
        
		}
	}
}