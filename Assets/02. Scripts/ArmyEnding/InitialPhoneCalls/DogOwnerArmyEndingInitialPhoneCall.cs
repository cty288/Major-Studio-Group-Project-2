using System;
using _02._Scripts.ChoiceSystem;
using _02._Scripts.GameEvents.BountyHunter;
using _02._Scripts.GameTime;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace _02._Scripts.ArmyEnding.InitialPhoneCalls {
	public class DogOwnerArmyEndingInitialPhoneCall : IncomingCallEvent {
		[field: ES3Serializable] public override float TriggerChance { get; } = 1;
		
		public DogOwnerArmyEndingInitialPhoneCall(TimeRange startTimeRange, TelephoneContact contact, int callWaitTime) : base(startTimeRange, contact, callWaitTime) {
			Debug.Log("DogOwnerArmyEndingInitialPhoneCall TimeRange: " + startTimeRange.StartTime);
		}

		public DogOwnerArmyEndingInitialPhoneCall(): base() {
			
		}
		public override void OnMissed() {
			OnMissedOrHangUp();
			
		}

		protected override void OnConversationStart() {
			
		}

		protected override void OnComplete() {
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime armyPrologueRadioTime = gameTimeModel.GetDay(gameTimeModel.Day + 1);
			gameEventSystem.AddEvent(new ShelterPrologueRadio(new TimeRange(armyPrologueRadioTime),
				AudioMixerList.Singleton.AudioMixerGroups[13]));

		}

		protected override void OnHangUp(bool hangUpByPlayer) {
			if (hangUpByPlayer) {
				return;
			}
			OnMissedOrHangUp();
		}
		
		private void OnMissedOrHangUp() {
			DateTime nextTime = this.GetModel<GameTimeModel>().GetDay(gameTimeManager.Day + 1);
			nextTime = nextTime.AddMinutes(Random.Range(20, 60));
			gameEventSystem.AddEvent(new DogOwnerArmyEndingInitialPhoneCall(new TimeRange(nextTime, nextTime.AddMinutes(60)),
				NotificationContact, 5));
		}
	}
	
	public class DogOwnerArmyEndingInitialPhoneCallContact: TelephoneContact {
		private AudioMixerGroup mixer;
		
		
		public DogOwnerArmyEndingInitialPhoneCallContact() : base() {
			speaker = GameObject.Find("TelephoneSpeaker").GetComponent<Speaker>();
			this.mixer = speaker.GetComponent<AudioSource>().outputAudioMixerGroup;
			
		}

		public override bool OnDealt() {
			return true;
		}

		protected override void OnStart() {
			string welcome = "Hey! I'm the dog owner. Have you heard anything about the military building a shelter? I've heard some chatter about the military building <color=yellow>a massive shelter</color> to protect people from those damn monsters.";
			speaker.Speak(welcome, mixer, "Dog Owner", 1f, OnWelcomeEnd, 1.1f, 1f, Gender.FEMALE);
		}

		private void OnWelcomeEnd(Speaker obj) {
			this.GetSystem<ChoiceSystem.ChoiceSystem>().StartChoiceGroup(new ChoiceGroup(ChoiceType.Telephone,
				new ChoiceOption("\"Shelter?\"", OnReply),
				new ChoiceOption("\"Oh Really?\"", OnReply)));
		}

		private void OnReply(ChoiceOption obj) {
			string content =
				"Ah, you don't know? <color=yellow>The military is building a shelter nearby, man</color>. Well, you should tune in to <color=yellow>FM108 tomorrow</color>. They might have some news about it recently. This could be the key to your survival, man, and you don't want to be caught napping when the big news drops.";
			
			speaker.Speak(content, mixer, "Dog Owner", 1f, OnSpeakEnd, 1.1f, 1f, Gender.FEMALE);
		}

		private void OnSpeakEnd(Speaker obj) {
			EndConversation();
		}

		protected override void OnHangUp(bool hangUpByPlayer) {
			
		}

		protected override void OnEnd() {
			
			
		}
	}
}