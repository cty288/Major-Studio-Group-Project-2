using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.Dog;
using _02._Scripts.GameTime;
using _02._Scripts.Poster;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02._Scripts.SuspectSystem {
	public class PoliceNotifySuspectEvent: BodyGenerationEvent {
		private SuspectModel suspectModel;
		protected long suspectID = -1;
		protected SuspectInfo suspectInfo = null;
		protected GameTimeModel gameTimeModel;
		public PoliceNotifySuspectEvent(TimeRange startTimeRange, BodyInfo bodyInfo): base(startTimeRange, bodyInfo, 1) {
			suspectModel = this.GetModel<SuspectModel>();
			gameTimeModel = this.GetModel<GameTimeModel>();
			Debug.Log("Police Notify Time Range: " + startTimeRange.StartTime + " - " + startTimeRange.EndTime);
		}

		public PoliceNotifySuspectEvent(): base() {
			gameTimeModel = this.GetModel<GameTimeModel>();
			suspectModel = this.GetModel<SuspectModel>();
		}

		public override EventState OnUpdate() {
			if (suspectID == -1) {
				suspectID = suspectModel.GetRandomSuspect(out suspectInfo);
				if(suspectID == -1) {
					return EventState.Missed;
				}
			}
			return base.OnUpdate();
		}

		public override void OnMissed() {
			DateTime nextTime = gameTimeModel.GetDay(gameTimeModel.Day + 1);
			nextTime = nextTime.AddMinutes(Random.Range(20, 80));
			gameEventSystem.AddEvent(new PoliceNotifySuspectEvent(new TimeRange(nextTime, nextTime.AddMinutes(40)),
				PoliceGenerateEvent.GeneratePolice()));
		}

		protected override void OnNotOpen() {
			base.OnNotOpen();
			OnMissed();
		}

		protected override Func<bool> OnOpen() {
			onClickPeepholeSpeakEnd = false;

			
			
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			List<string> messages = new List<string>();

			if (!suspectModel.HasMetPoliceBefore) {
				messages.Add( "if you spot the guy on this poster, please <color=yellow>contact us and send us a photo</color> as soon as possible.");
			
				messages.Add("if you come across the person on the poster, <color=yellow>give us a call and send us a photo</color>");
			
				messages.Add( "if you happen to see the person on the poster, " +
				             "please don't hesitate to <color=yellow>contact us and send us a photo</color>.");
			}
			else {
				messages.Add("Hello again! " +
				             "If you spot the guy on this poster, please <color=yellow>contact us and send us a photo</color>.");
				
				messages.Add("Well hello again,  we've got a new warrant for you to look over again. " );

				messages.Add("We've got another warrant for your attention." );
			}
			
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			suspectModel.HasMetPoliceBefore = true;
			DogModel dogModel = this.GetModel<DogModel>();
			string additionalMessage = "";
			if(!dogModel.SentDogBack && dogModel.isDogAlive) {
				additionalMessage = " By the way, one of our residents has a dog that went missing.";
				this.GetModel<PosterModel>()
					.AddPoster(new MissingDogPoster(dogModel.MissingDogPhoneNumber, dogModel.MissingDogBodyInfo));
			}
				
			speaker.Speak(messages[UnityEngine.Random.Range(0, messages.Count)] + additionalMessage,
				bodyInfo.VoiceTag.VoiceGroup,
				"Police", 1, OnSpeakEnd,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);

			
			
			return () => onClickPeepholeSpeakEnd;
		}

		private void OnSpeakEnd(Speaker obj) {
			this.GetModel<PosterModel>().AddPoster(new SuspectPoster(suspectID, suspectInfo));
			onClickPeepholeSpeakEnd = true;

			DateTime nextTime = gameTimeModel.GetDay(gameTimeModel.Day + Random.Range(3, 7));
			nextTime = nextTime.AddMinutes(Random.Range(20, 80));
			gameEventSystem.AddEvent(new PoliceNotifySuspectEvent(new TimeRange(nextTime, nextTime.AddMinutes(40)),
				PoliceGenerateEvent.GeneratePolice()));
		}

		public override void OnEventEnd() {
			
		}
	}
}