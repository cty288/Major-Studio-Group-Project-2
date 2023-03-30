using System;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace _02._Scripts.Dog {

	public struct OnDogSendBack {
		
	}
	public class MissingDogContact: TelephoneContact {
		private AudioMixerGroup mixer;
		protected DogModel dogModel;
		private bool dogSent = false;
		public MissingDogContact() {
			speaker = GameObject.Find("MissingDogSpeaker").GetComponent<Speaker>();
			this.mixer = speaker.GetComponent<AudioSource>().outputAudioMixerGroup;
			dogModel = this.GetModel<DogModel>();
		}
		public override bool OnDealt() {
			return true;
		}

		protected override void OnStart() {
			dogSent = false;
			List<string> speakContent = new List<string>();
			if (dogModel.SentDogBack) {
				speakContent.Add("Thank you for sending my Little Jonny back to me! I hope you like the reward!");
				speakContent.Add("I really appreciate your help! I will never forget this!");
			}
			else {
				if (dogModel.HaveDog && dogModel.isDogAlive) {
					speakContent.Add(
						"Hello! You said you found my dog! Can you please send him back to me? I am so worried about my Little Jonny!" +
						"Thank you sooo much! I will deliver you a reward later on!");
					
					dogSent = true;
					GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
					DateTime time = gameTimeModel.GetDay(gameTimeModel.Day + Random.Range(1, 3));

					this.GetSystem<GameEventSystem>().AddEvent(new MissingDogRewardEvent(new TimeRange(time)));
				}
				else {
					speakContent.Add(
						"Hello! I am looking for my Little Jonny! I've posted his appearance on the newspaper. " +
						"If you find him, please send him back to me! And I will reward you! Thank you so much!");
					speakContent.Add(
						"Hi! If you find my Little Jonny, please send him back to me! He is so important to my family! " +
						"I've posted his appearance on the newspaper. I will reward you for your help! Thank you!");
				}
			}
			
			speaker.Speak(speakContent[Random.Range(0, speakContent.Count)], mixer, "Dog Owner", 1f, OnSpeakEnd,
				1.2f,1f, Gender.FEMALE);
		}

		private void OnSpeakEnd(Speaker obj) {
			if (dogSent) {
				dogModel.SentDogBack = true;
				this.SendEvent<OnDogSendBack>();
			}
			
			EndConversation();
		}

		protected override void OnHangUp() {
			dogSent = false;
		}

		protected override void OnEnd() {
			dogSent = false;
		}
	}
}