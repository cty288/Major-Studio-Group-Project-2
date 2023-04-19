using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.ChoiceSystem;
using _02._Scripts.GameTime;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.TimeSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02._Scripts.ArmyEnding {
	public class ArmyGunConfiscatedEvent: BodyGenerationEvent {
		public ArmyGunConfiscatedEvent(TimeRange startTimeRange, BodyInfo bodyInfo): base(startTimeRange, bodyInfo, 1) {
			Debug.Log("Army come to confiscate your gun time range: " + startTimeRange.StartTime + " - " +
			          startTimeRange.EndTime);
		}

		public ArmyGunConfiscatedEvent(): base() {
			
		}

		public override void OnMissed() {
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime nextTime = gameTimeModel.GetDay(gameTimeModel.Day + 1);
			nextTime = nextTime.AddMinutes(Random.Range(30, 60));
			gameEventSystem.AddEvent(new ArmyGunConfiscatedEvent(new TimeRange(nextTime, nextTime.AddMinutes(60)),
				GenerateArmyBodyInfo()));
		}

		public static BodyInfo GenerateArmyBodyInfo() {
			HeightType height = HeightType.Tall;

			int armyIndex = Random.Range(4, 6);
			var targetList = AlienBodyPartCollections.Singleton.SpecialBodyPartPrefabs.HeightSubCollections[armyIndex]
				.ShadowBodyPartPrefabs.HumanTraitPartsPrefabs;
			
			BodyPartPrefabInfo body = targetList[1].GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo(0);
        
			BodyPartPrefabInfo leg =  targetList[0].GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo(0);
        
			BodyPartPrefabInfo head = targetList[2].GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo(0);

			return BodyInfo.GetBodyInfo(leg, body, head, height,
				new VoiceTag(AudioMixerList.Singleton.AudioMixerGroups[13], 1.2f, Gender.MALE),
				new NormalKnockBehavior(4, 5, null, "Knock_Army"), BodyPartDisplayType.Shadow, false);

		}

		protected override Func<bool> OnOpen() {
			onClickPeepholeSpeakEnd = false;
			
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			string message =
				"Thank you for letting me in. As a military officer, I'm here to confirm that you meet the eligibility criteria for shelter registration. " +
				"We need to ensure that those seeking refuge are truly in need of protection, so I'll need to conduct a quick inspection of your home.";

			AudioSource audioSource = AudioSystem.Singleton.Play2DSound("door_open");
			
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			
			speaker.Speak(message,
				bodyInfo.VoiceTag.VoiceGroup,
				"Military Officer", 1, OnWelcomeEnd,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
			
			
			
			
			
			return () => onClickPeepholeSpeakEnd;
		}

		private void OnWelcomeEnd(Speaker obj) {
			AudioSource audioSource = AudioSystem.Singleton.Play2DSound("rummage");
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			ITimeSystem timeSystem = this.GetSystem<ITimeSystem>();
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			
			
			var addDelayTask = timeSystem.AddDelayTask(audioSource.clip.length, () => {
				
				if (playerResourceModel.HasEnoughResource<GunResource>(1)) {
					string speakContent =
						"I see you have a gun here. While you do meet the eligibility criteria for shelter registration, " +
						"<color=yellow>firearms are strictly prohibited inside the shelter</color> for safety reasons. " +
						"In order to qualify for entry, you'll need to surrender your weapon to me.";
						
						
					speaker.Speak(speakContent,
						bodyInfo.VoiceTag.VoiceGroup,
						"Military Officer", 1, DecideGiveUpGun,
						voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
				}
				else {
					string speakContent =
						"Thank you for your cooperation. Your home has been inspected and you meet the criteria for entry into the shelter. " +
						"In a few days, <color=yellow>someone else in military uniform will be knocking on your door</color>. " +
						"<color=yellow>They'll take you to the shelter, where you'll be safe</color>. Thank you for doing your part to ensure the safety of everyone in the shelter.";
					speaker.Speak(speakContent,
						bodyInfo.VoiceTag.VoiceGroup,
						"Military Officer", 1, OnConfiscateSuccess,
						voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
					
				}
				
				
				
			});
		}

		private void DecideGiveUpGun(Speaker obj) {
			this.GetSystem<ChoiceSystem.ChoiceSystem>().StartChoiceGroup(new ChoiceGroup(ChoiceType.Outside,
				new ChoiceOption("- Surrender your gun", OnYes),
				new ChoiceOption("- Keep the gun (you will lose the eligibility for entry into the shelter)", OnNo)));
		}

		private void OnNo(ChoiceOption obj) {
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			string message =
				"Unfortunately we cannot allow weapons inside the shelter. " +
				"Without surrendering your gun, you won't be able to enter. " +
				"<color=yellow>I strongly urge you to reconsider and hand it over so that you can gain entry and stay safe</color>.";
			
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			
			speaker.Speak(message,
				bodyInfo.VoiceTag.VoiceGroup,
				"Military Officer", 1, DecideGiveUpAgain,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
		}

		private void DecideGiveUpAgain(Speaker obj) {
			this.GetSystem<ChoiceSystem.ChoiceSystem>().StartChoiceGroup(new ChoiceGroup(ChoiceType.Outside,
				new ChoiceOption("- Surrender your gun", OnYes),
				new ChoiceOption("- Keep the gun (you will <color=red>really</color> lose the eligibility)", OnNoAgain)));
		}

		private void OnNoAgain(ChoiceOption obj) {
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			string message = "I'm sorry, but without surrendering your gun, you won't be able to enter the shelter." +
			                 " I have to leave now. I hope you'll understand that the safety of everyone in the shelter is our top priority.";
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			
			speaker.Speak(message,
				bodyInfo.VoiceTag.VoiceGroup,
				"Military Officer", 1, OnConfiscateFail,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
		}

		private void OnYes(ChoiceOption obj) {
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			string message =
				"Thank you, your compliance with the rules is greatly appreciated. " +
				"<color=yellow>In a few days, someone else in military uniform will be knocking on your door</color>. " +
				"<color=yellow>They'll take you to the shelter, where you'll be safe</color>.";
			
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			playerResourceModel.RemoveResource<GunResource>(1);
			speaker.Speak(message,
				bodyInfo.VoiceTag.VoiceGroup,
				"Military Officer", 1, OnConfiscateSuccess,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
		}

		private void OnConfiscateSuccess(Speaker obj) {
			onClickPeepholeSpeakEnd = true;
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime finalKnockTime = gameTimeModel.GetDay(gameTimeModel.Day + 3);
			Debug.Log("finalKnockTime: " + finalKnockTime);
			gameEventSystem.AddEvent(new FinalArmyKnockEnding(
				new TimeRange(finalKnockTime, finalKnockTime.AddMinutes(119)), GenerateArmyBodyInfo(), 0));
		}
		
		protected void OnConfiscateFail(Speaker obj) {
			onClickPeepholeSpeakEnd = true;
		}

		protected override void OnNotOpen() {
			base.OnNotOpen();
			OnMissed();
		}

		public override void OnEventEnd() {
			
		}
	}
}