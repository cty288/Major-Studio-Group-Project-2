using System;
using _02._Scripts.ChoiceSystem;
using _02._Scripts.GameTime;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace _02._Scripts.ArmyEnding.InitialPhoneCalls {
	public class PoliceArmyEndingInitialPhoneCall: IncomingCallEvent {
		[field: ES3Serializable]
		public override float TriggerChance { get; } = 1;
		
		public PoliceArmyEndingInitialPhoneCall(TimeRange startTimeRange, TelephoneContact contact, int callWaitTime) : base(startTimeRange, contact, callWaitTime) {
			Debug.Log("PoliceArmyEndingInitialPhoneCall TimeRange: " + startTimeRange.StartTime);
		}

		public PoliceArmyEndingInitialPhoneCall(): base() {
			
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
			gameEventSystem.AddEvent(new PoliceArmyEndingInitialPhoneCall(new TimeRange(nextTime, nextTime.AddMinutes(60)),
				NotificationContact, 5));
		}
	}
	
	
	public class PoliceArmyEndingInitialPhoneCallContact: TelephoneContact {
		[field: ES3Serializable]
		private AudioMixerGroup mixer;

		[ES3Serializable] private string policeName;
		
		public PoliceArmyEndingInitialPhoneCallContact() : base() {
			speaker = GameObject.Find("TelephoneSpeaker").GetComponent<Speaker>();
		}
		
		public PoliceArmyEndingInitialPhoneCallContact(AudioMixerGroup mixer, string policeName) : base() {
			this.mixer = mixer;
			this.policeName = policeName;
			speaker = GameObject.Find("TelephoneSpeaker").GetComponent<Speaker>();
		}

		public override bool OnDealt() {
			return true;
		}

		protected override void OnStart() {
			string welcome = $" Hi, it's {policeName} from the Police Department. We just wanted to thank you for reporting that criminal the other day. Your help was invaluable in bringing them to justice.";
			speaker.Speak(welcome, mixer, $"{policeName}", 1f, OnWelcomeEnd, 1.1f, 1f, Gender.MALE);
		}

		private void OnWelcomeEnd(Speaker obj) {
			this.GetSystem<ChoiceSystem.ChoiceSystem>().StartChoiceGroup(new ChoiceGroup(ChoiceType.Telephone,
				new ChoiceOption("\"No problem!\"", OnReply)));
		}

		private void OnReply(ChoiceOption obj) {
			string content =
				"Listen, we also wanted to let you know that <color=yellow>there might be some important news about the military's shelter plans on FM108 tomorrow</color>. You should tune in and stay informed.";

			speaker.Speak(content, mixer, policeName, 1f, OnRely2, 1.1f, 1f);
		}

		private void OnRely2(Speaker obj) {
			this.GetSystem<ChoiceSystem.ChoiceSystem>().StartChoiceGroup(new ChoiceGroup(ChoiceType.Telephone,
				new ChoiceOption("\"Shelter?\"", OnReply3),
				new ChoiceOption("\"What do you mean? A shelter?\"", OnReply3)));
		}

		private void OnReply3(ChoiceOption obj) {
			string content =
				"Ah, you don't know? <color=yellow>The military is building a shelter nearby, man</color>. " +
				"We don't have all the details yet, but <color=yellow>FM108</color> might have the inside scoop tomorrow. " +
				"You should definitely tune in if you're looking to stay alive, my dude.";

			speaker.Speak(content, mixer, policeName, 1f, OnSpeakEnd, 1.1f, 1f);
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