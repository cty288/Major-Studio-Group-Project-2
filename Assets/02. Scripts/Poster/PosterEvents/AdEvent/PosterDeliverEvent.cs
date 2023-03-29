using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02._Scripts.Poster.PosterEvents {
	public abstract class PosterDeliverEvent<T>: BodyGenerationEvent, IPosterEvent<T> where T: Poster {
		public abstract T Poster { get; set; }
		public PosterModel PosterModel { get; protected set; }


		
		public PosterDeliverEvent(T poster, TimeRange startTimeRange, BodyInfo bodyInfo, float eventTriggerChance): base(startTimeRange, bodyInfo, eventTriggerChance) {
			this.Poster = poster;
			PosterModel = this.GetModel<PosterModel>();
		}

		public PosterDeliverEvent(): base() {
			PosterModel = this.GetModel<PosterModel>();
		}
		
		
		protected override Func<bool> OnOpen() {
			onClickPeepholeSpeakEnd = false;
       
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			List<string> messages = new List<string>() {
				"Hey, here's the poster you subscribed to. It's a bit late, but I hope you like it.",
				"Here's the poster you subscribed. I hope you like it.",
			};
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			speaker.Speak(messages[Random.Range(0, messages.Count)],
				AudioMixerList.Singleton.AlienVoiceGroups[bodyInfo.VoiceTag.VoiceIndex],
				"Poster Deliver", 1, OnDelivererClickedOutside,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);

			this.AddPosterToModel(Poster);
			
			return () => onClickPeepholeSpeakEnd;
		}

		private void OnDelivererClickedOutside(Speaker obj) {
			// this.SendEvent<OnShowFood>();
			timeSystem.AddDelayTask(1f, () => {
				onClickPeepholeSpeakEnd = true;
				OnFinishOpenDoor();
			});
		}

		protected abstract void OnFinishOpenDoor();
	}
}