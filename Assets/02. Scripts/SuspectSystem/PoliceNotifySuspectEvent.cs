using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using MikroFramework.Architecture;
using UnityEngine;

namespace _02._Scripts.SuspectSystem {
	public class PoliceNotifySuspectEvent: BodyGenerationEvent {
		private SuspectModel suspectModel;
		protected long suspectID = -1;
		protected GoodsInfo rewardInfo = null;
		public PoliceNotifySuspectEvent(TimeRange startTimeRange, BodyInfo bodyInfo): base(startTimeRange, bodyInfo, 1) {
			suspectModel = this.GetModel<SuspectModel>();
		}

		public PoliceNotifySuspectEvent(): base() {
			suspectModel = this.GetModel<SuspectModel>();
		}

		public override EventState OnUpdate() {
			if (suspectID == -1) {
				suspectID = suspectModel.GetRandomSuspect(out rewardInfo);
				if(suspectID == -1) {
					return EventState.Missed;
				}
			}
			return base.OnUpdate();
		}

		public override void OnMissed() {
			throw new NotImplementedException();
		}

		protected override Func<bool> OnOpen() {
			onClickPeepholeSpeakEnd = false;
       
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			List<string> messages = new List<string>() {
				"Thanks for having me over, are you ready to experience pleasure like never before?",
				"Hi there, it's great to see you. Are you ready to have some fun?",
			};
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			speaker.Speak(messages[UnityEngine.Random.Range(0, messages.Count)],
				bodyInfo.VoiceTag.VoiceGroup,
				"???", 1, OnSpeakEnd,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);

			
			
			return () => onClickPeepholeSpeakEnd;
		}

		private void OnSpeakEnd(Speaker obj) {
			
		}

		public override void OnEventEnd() {
			throw new NotImplementedException();
		}
	}
}