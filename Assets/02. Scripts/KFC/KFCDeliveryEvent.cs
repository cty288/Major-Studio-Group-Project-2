using System;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02._Scripts.KFC {
	public class KFCDeliveryEvent: BodyGenerationEvent {
		public KFCDeliveryEvent(TimeRange startTimeRange, BodyInfo bodyInfo) :
			base(startTimeRange, bodyInfo, 1) {
			Debug.Log("KFC will be delivered at " + startTimeRange.StartTime);
		}
    
		public KFCDeliveryEvent(){}
		public override void OnMissed() {
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime nextTime = gameTimeModel.CurrentTime.Value.AddMinutes(Random.Range(20, 40));
			gameEventSystem.AddEvent(new KFCDeliveryEvent(new TimeRange(nextTime, nextTime.AddMinutes(20)), bodyInfo));
		}

		protected override Func<bool> OnOpen() {
			onClickPeepholeSpeakEnd = false;
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			string message = "Enjoy your free CFK and don't forget to give us a good review!";
			this.GetModel<PlayerResourceModel>().AddFood(Random.Range(2, 4));
			speaker.Speak(message, bodyInfo.VoiceTag.VoiceGroup, "CFK Deliverer", 1f, OnFinishSpeak, 1.15f, 1.2f);
			return () => onClickPeepholeSpeakEnd;
		}

		private void OnFinishSpeak(Speaker obj) {
			onClickPeepholeSpeakEnd = true;
		}

		public override void OnEventEnd() {
			
		}

		protected override void OnNotOpen() {
			base.OnNotOpen();
		}
	}
}