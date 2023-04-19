using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.ChoiceSystem;
using _02._Scripts.GameTime;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;


namespace _02._Scripts.KFC {
	public class KFCPhone: TelephoneContact {
		private AudioMixerGroup mixer;

		public KFCPhone() : base() {
			speaker = GameObject.Find("KFCSpeaker").GetComponent<Speaker>();
			this.mixer = speaker.GetComponent<AudioSource>().outputAudioMixerGroup;
		}
		
		public override bool OnDealt() {
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			if (gameTimeModel.CurrentTime.Value.DayOfWeek == DayOfWeek.Thursday) {
				return true;
			}

			return false;
		}

		protected override void OnStart() {

			string speakContent = "Hello, this is Chicken Fried Kentucky, the best fried chicken in the world." +
			                      " As you know, we have a special offer on every Thursday. " +
			                      "Would you like to have some free fried chicken?";
			speaker.Speak(speakContent, mixer, "CFK", 1f, OnSpeakEnd,
				1.2f,1f, Gender.FEMALE);
			
		}

		protected BodyInfo GetKFCDeliveryMan() {
			BodyPartPrefabInfo headPrefabInfo = AlienBodyPartCollections.Singleton.HeadBodyPartPrefabs.HeightSubCollections[0].ShadowBodyPartPrefabs
				.HumanTraitPartsPrefabs[15].GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo(0);
			BodyPartPrefabInfo bodyPrefabInfo = AlienBodyPartCollections.Singleton.MainBodyPartPrefabs
				.HeightSubCollections[0].ShadowBodyPartPrefabs
				.HumanTraitPartsPrefabs[15].GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo(5, true);
			BodyPartPrefabInfo legPrefabInfo = AlienBodyPartCollections.Singleton.LegBodyPartPrefabs.HeightSubCollections[0].ShadowBodyPartPrefabs
				.HumanTraitPartsPrefabs[15].GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo(0);
			BodyInfo bodyInfo = BodyInfo.GetBodyInfo(legPrefabInfo, bodyPrefabInfo, headPrefabInfo, HeightType.Tall,
				new VoiceTag(AudioMixerList.Singleton.AlienVoiceGroups[1], 1, Gender.MALE),
				new NormalKnockBehavior(4, 5, null, "Knock_KFC"), BodyPartDisplayType.Shadow, false);
			return bodyInfo;
		}

		private void OnSpeakEnd(Speaker obj) {
			ChoiceSystem.ChoiceSystem choiceSystem = this.GetSystem<ChoiceSystem.ChoiceSystem>();
			choiceSystem.StartChoiceGroup(new ChoiceGroup(ChoiceType.Telephone,
				new ChoiceOption("\"Sure.\"", OnAgree),
				new ChoiceOption("\"No, thanks\"", OnRefuse)));
		}

		
		private void OnAgree(ChoiceOption obj) {
			string speakContent = "Sure! We will arrange delivery for you as soon as possible. " +
			                      "Just make sure there are no prying eyes when the delivery person arrives. We don't want any monsters trying to steal your food, do we?";
			speaker.Speak(speakContent, mixer, "CFK", 1f, OnFinish,
				1.2f,1f, Gender.FEMALE);
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime eventTime = gameTimeModel.CurrentTime.Value.AddMinutes(Random.Range(30, 60));
			this.GetSystem<GameEventSystem>().AddEvent(new KFCDeliveryEvent(new TimeRange(eventTime, eventTime.AddMinutes(20)), GetKFCDeliveryMan()));
		}
		private void OnRefuse(ChoiceOption obj) {
			string speakContent = "Oh, that's a shame. We have the best fried chicken in the world. " +
			                      "But if you don't want it, that's fine. We will just give it to someone else.";
			speaker.Speak(speakContent, mixer, "CFK", 1f, OnFinish,
				1.2f,1f, Gender.FEMALE);
		}

		private void OnFinish(Speaker obj) {
			EndConversation();
		}

		protected override void OnHangUp(bool hangUpByPlayer) {
			
		}

		protected override void OnEnd() {
			
		}
	}
}