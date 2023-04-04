using System;
using System.Collections.Generic;
using _02._Scripts.ChoiceSystem;
using _02._Scripts.GameTime;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace _02._Scripts.SexyCard {
	public class SexyCardPhoneContact: TelephoneContact {
		private AudioMixerGroup mixer;
		[ES3Serializable]
		protected Gender speakerGender;
		
		protected SexyCardModel model;
		public SexyCardPhoneContact() : base() {
			speaker = GameObject.Find("SexyCardSpeaker").GetComponent<Speaker>();
			this.mixer = this.GetModel<SexyCardModel>().SexyPerson.VoiceTag.VoiceGroup;
			speakerGender = this.GetModel<SexyCardModel>().SexyPersonGender;
			model = this.GetModel<SexyCardModel>();
		}
		public override bool OnDealt() {
			return model.IsSexyPersonAvailable;
		}

		protected override void OnStart() {
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime now = gameTimeModel.CurrentTime.Value;
			List<string> speakContent = new List<string>();
			bool tooLate = false;
			gameTimeModel.LockTime.Retain();
			if (now.Hour >= 23) {
				tooLate = true;
				speakContent.Add(
					"Sorry, it's too late for me to come over. Make sure to call earlier next time if you want to have some fun.");
				speakContent.Add(
					"Sorry, you caught me at a bad time. Next time, try calling earlier and we'll make something happen.");
			}
			else {
				speakContent.Add("Hello there, welcome to my world of pleasure. Do you want to make something happen?");
				speakContent.Add(
					"Hello, you've reached the right person for all your sexual desires. Do you want to have some pleasure?");
			}

			string content = speakContent[UnityEngine.Random.Range(0, speakContent.Count)];
			if (tooLate) {
				speaker.Speak(content, mixer, "???", 1f, OnSexUnAvailable, 1f,1f, speakerGender);
			}
			else {
				speaker.Speak(content, mixer, "???", 1f, OnSexWelcomeEnd, 1f,1f, speakerGender);
			}
		}

		private void OnSexWelcomeEnd(Speaker speaker) {
			ChoiceSystem.ChoiceSystem choiceSystem = this.GetSystem<ChoiceSystem.ChoiceSystem>();
			choiceSystem.StartChoiceGroup(new ChoiceGroup(ChoiceType.Telephone,
				new ChoiceOption("\"Sure\"", OnPlayerAgree),
				new ChoiceOption("\"No.\"", OnPlayerRefuse)));
		}

		private void OnPlayerRefuse(ChoiceOption option) {
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			
			speaker.Speak("That's perfectly fine. Remember, I'm always here if you change your mind or want to explore your sexual desires further."
				, mixer, "???", 1f, OnSexUnAvailable, 1,1f, speakerGender);
		}

		private void OnPlayerAgree(ChoiceOption option) {
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime comeTime = gameTimeModel.CurrentTime.Value;
			comeTime = comeTime.AddMinutes(Random.Range(30, 60));

			string time = comeTime.ToString("hh:mm");
			List<string> speakContent = new List<string>();
			speakContent.Add(
				$"Perfect, I'll be there at <color=yellow>{time}</color> ready to please you. Can't wait to meet you. Just a heads up, it's best if there's no one outside when I come over. We don't want to attract any unwanted attention.");

			speakContent.Add(
				$"Perfect, I'll be there at <color=yellow>{time}</color> ready to fulfill your desires. To avoid any awkward encounters, please ensure there's no one outside when I arrive. See you soon.");
			
			string content = speakContent[UnityEngine.Random.Range(0, speakContent.Count)];
			speaker.Speak(content
				, mixer, "???", 1f, OnPlayerAgree, 1f,1f, speakerGender);
			this.GetSystem<GameEventSystem>()
				.AddEvent(new SexyPersonComeEvent(new TimeRange(comeTime, comeTime.AddMinutes(20)), model.SexyPerson));
		}

		private void OnPlayerAgree(Speaker obj) {
			model.IsSexyPersonAvailable = false;
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			gameTimeModel.LockTime.Release();
			EndConversation();
		}

		private void OnSexUnAvailable(Speaker obj) {
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			gameTimeModel.LockTime.Release();
			EndConversation();
		}

		protected override void OnHangUp() {
			
		}

		protected override void OnEnd() {
			
		}
	}
}