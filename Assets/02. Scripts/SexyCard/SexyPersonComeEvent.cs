using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.ChoiceSystem;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.TimeSystem;
using UnityEngine;
using Random = System.Random;

namespace _02._Scripts.SexyCard {
	public class SexyPersonComeEvent: BodyGenerationEvent {
		protected SexyCardModel sexyCardModel;
		public SexyPersonComeEvent(TimeRange startTimeRange, BodyInfo bodyInfo): base(startTimeRange, bodyInfo, 1) {
			sexyCardModel = this.GetModel<SexyCardModel>();
		}

		public SexyPersonComeEvent(): base() {
			sexyCardModel = this.GetModel<SexyCardModel>();
		}
		public override void OnMissed() {
			this.GetModel<SexyCardModel>().IsSexyPersonAvailable = true;
		}

		protected override Func<bool> OnOpen() {
			onClickPeepholeSpeakEnd = false;
       
			Speaker speaker = GameObject.Find("SexyCardSpeaker").GetComponent<Speaker>();
			List<string> messages = new List<string>() {
				"Thanks for having me over, are you ready to experience pleasure like never before?",
				"Hi there, it's great to see you. Are you ready to have some fun?",
			};
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			speaker.Speak(messages[UnityEngine.Random.Range(0, messages.Count)],
				bodyInfo.VoiceTag.VoiceGroup,
				"???", 1, OnGreetingEnd,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);

			
			
			return () => onClickPeepholeSpeakEnd;
		}

		private void OnGreetingEnd(Speaker obj) {
			this.GetSystem<ChoiceSystem.ChoiceSystem>().StartChoiceGroup(new ChoiceGroup(ChoiceType.Outside,
				new ChoiceOption("- \"Yes, let's do this.\"", OnYes),
				new ChoiceOption("- \"Sorry, I've changed my mind.\"", OnNo)));
		}

		private void OnNo(ChoiceOption obj) {
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			Speaker speaker = GameObject.Find("SexyCardSpeaker").GetComponent<Speaker>();
			speaker.Speak("No problem. If you ever need anything else, feel free to reach out.",
				bodyInfo.VoiceTag.VoiceGroup,
				"???", 1, (speaker) => {
					onClickPeepholeSpeakEnd = true;
				},
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
		}

		private void OnYes(ChoiceOption obj) {
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			Speaker speaker = GameObject.Find("SexyCardSpeaker").GetComponent<Speaker>();
			AudioSource audioSource = AudioSystem.Singleton.Play2DSound("door_open");
			ITimeSystem timeSystem = this.GetSystem<ITimeSystem>();
			timeSystem.AddDelayTask(audioSource.clip.length, () => {
				audioSource = AudioSystem.Singleton.Play2DSound("hit_player");
				timeSystem.AddDelayTask(audioSource.clip.length + 0.5f, () => {
					speaker.Speak("HAHAHA. I'm not actually here to give you pleasure. You're so naive to invite me over. Now give me everything you have, or else!",
						bodyInfo.VoiceTag.VoiceGroup,
						"???", 1, OnRobSpeakFinished,
						voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
				});
			});
		}

		private void OnRobSpeakFinished(Speaker obj) {
			AudioSource audioSource = AudioSystem.Singleton.Play2DSound("rummage");
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			ITimeSystem timeSystem = this.GetSystem<ITimeSystem>();
			Speaker speaker = GameObject.Find("SexyCardSpeaker").GetComponent<Speaker>();
			timeSystem.AddDelayTask(audioSource.clip.length, () => {
				string speakContent = "Thanks for being such an easy target. I appreciate it.";
				if (playerResourceModel.GetResourceCount<FoodResource>() <= 0) {
					speakContent = "No food? What a waste of my time. I guess I'll be leaving now.";
				}
				speaker.Speak(speakContent,
					bodyInfo.VoiceTag.VoiceGroup,
					"???", 1, OnRobEnd,
					voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
			});
		}

		private void OnRobEnd(Speaker obj) {
			AudioSource audioSource = AudioSystem.Singleton.Play2DSound("slam_door");
			ITimeSystem timeSystem = this.GetSystem<ITimeSystem>();
			int robMaxCount = Mathf.Min(4, playerResourceModel.GetResourceCount<FoodResource>());
			int robCount = 0;
			if (robMaxCount > 0) {
				robCount = UnityEngine.Random.Range(2, robMaxCount + 1);
			}

			playerResourceModel.RemoveFood(robMaxCount);
			timeSystem.AddDelayTask(audioSource.clip.length, () => {
				LoadCanvas.Singleton.ShowMessage("Your foods are robbed!");
				timeSystem.AddDelayTask(2f, () => {
					LoadCanvas.Singleton.HideMessage();
					onClickPeepholeSpeakEnd = true;
				});
				
			});
		}

		public override void OnEventEnd() {
			this.GetModel<SexyCardModel>().IsSexyPersonAvailable = true;
		}
	}
}