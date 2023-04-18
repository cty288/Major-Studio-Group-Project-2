using System;
using _02._Scripts.ChoiceSystem;
using _02._Scripts.GameTime;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace _02._Scripts.ArmyEnding.InitialPhoneCalls {
	public class FoodBorrowerArmyEndingInitialPhoneCall : IncomingCallEvent {
		[field: ES3Serializable] public override float TriggerChance { get; } = 1;
		
		public FoodBorrowerArmyEndingInitialPhoneCall(TimeRange startTimeRange, TelephoneContact contact, int callWaitTime) : base(startTimeRange, contact, callWaitTime) {
			Debug.Log("FoodBorrowerArmyEndingInitialPhoneCall TimeRange: " + startTimeRange.StartTime);
		}

		public FoodBorrowerArmyEndingInitialPhoneCall(): base() {
			
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

		protected override void OnHangUp() {
			OnMissedOrHangUp();
		}
		
		private void OnMissedOrHangUp() {
			DateTime nextTime = this.GetModel<GameTimeModel>().GetDay(gameTimeManager.Day + 1);
			nextTime = nextTime.AddMinutes(Random.Range(20, 60));
			gameEventSystem.AddEvent(new FoodBorrowerArmyEndingInitialPhoneCall(new TimeRange(nextTime, nextTime.AddMinutes(60)),
				NotificationContact, 5));
		}
	}
	
	public class FoodBorrowerArmyEndingInitialPhoneCallContact: TelephoneContact {
		[ES3Serializable]
		private AudioMixerGroup mixer;
		
		
		public FoodBorrowerArmyEndingInitialPhoneCallContact() : base() {
			speaker = GameObject.Find("TelephoneSpeaker").GetComponent<Speaker>();
		}
		
		public FoodBorrowerArmyEndingInitialPhoneCallContact(AudioMixerGroup mixer) : base() {
			this.mixer = mixer;
			speaker = GameObject.Find("TelephoneSpeaker").GetComponent<Speaker>();
		}

		public override bool OnDealt() {
			return true;
		}

		protected override void OnStart() {
			string welcome = "Hey, it's me. Do you remember me? You gave me some food a few days ago, remember?";
			speaker.Speak(welcome, mixer, "Food borrower", 1f, OnWelcomeEnd, 1.1f, 1f, Gender.MALE);
		}

		private void OnWelcomeEnd(Speaker obj) {
			this.GetSystem<ChoiceSystem.ChoiceSystem>().StartChoiceGroup(new ChoiceGroup(ChoiceType.Telephone,
				new ChoiceOption("\"Yeah, how are you doing?\"", OnReply),
				new ChoiceOption("\"What's good man?\"", OnReply)));
		}

		private void OnReply(ChoiceOption obj) {
			string content =
				"Not much, just living the dream in this monster-infested wasteland. Say, have you heard anything about the military building a shelter?";
			
			speaker.Speak(content, mixer, "Food borrower", 1f, OnReply2, 1.1f, 1f, Gender.MALE);
		}

		private void OnReply2(Speaker obj) {
			this.GetSystem<ChoiceSystem.ChoiceSystem>().StartChoiceGroup(new ChoiceGroup(ChoiceType.Telephone,
				new ChoiceOption("\"Nah, what do you know?\"", OnReply3),
				new ChoiceOption("\"A shelter?\"", OnReply3)));
		}

		private void OnReply3(ChoiceOption obj) {
			string content =
				"Ah, you don't know? <color=yellow>The military is building a shelter nearby, man</color>. " +
				"Well, I talked to some dudes, and they said you need to tune in to <color=yellow>FM108</color>. " +
				"They might have some juicy details about <color=yellow>the shelter tomorrow</color>.";
			
			speaker.Speak(content, mixer, "Food borrower", 1f, OnReply4, 1.1f, 1f, Gender.MALE);
		}

		private void OnReply4(Speaker obj) =>
			this.GetSystem<ChoiceSystem.ChoiceSystem>().StartChoiceGroup(new ChoiceGroup(ChoiceType.Telephone,
				new ChoiceOption("\"Thanks for the heads up!\"", OnReply5)));

		private void OnReply5(ChoiceOption obj) {
			string content = "Hey, no problem, my dude. You also helped me before. We gotta look out for each other in this crazy world.";

			speaker.Speak(content, mixer, "Food borrower", 1f, OnSpeakEnd, 1.1f, 1f, Gender.MALE);
		}

		private void OnSpeakEnd(Speaker obj) {
			EndConversation();
		}

		protected override void OnHangUp() {
			
		}

		protected override void OnEnd() {
			
		}
	}
}