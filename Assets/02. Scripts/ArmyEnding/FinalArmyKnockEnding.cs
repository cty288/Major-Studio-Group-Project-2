using System;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using UnityEngine;

namespace _02._Scripts.ArmyEnding {
	public class FinalArmyKnockEnding : BodyGenerationEvent {
		[field: ES3Serializable] protected int refuseToOpenTime = 0;
		
		public FinalArmyKnockEnding(TimeRange startTimeRange, BodyInfo bodyInfo, int refuseToOpenTime): base(startTimeRange, bodyInfo, 1) {
			this.refuseToOpenTime = refuseToOpenTime;
		}

		public FinalArmyKnockEnding(): base() {
			
		}
		
		
		public override void OnMissed() {
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime finalKnockTime = gameTimeModel.GetDay(gameTimeModel.Day + 1);
			Debug.Log("finalKnockTime: " + finalKnockTime);
			gameEventSystem.AddEvent(new FinalArmyKnockEnding(
				new TimeRange(finalKnockTime, finalKnockTime.AddMinutes(119)),
				ArmyGunConfiscatedEvent.GenerateArmyBodyInfo(), refuseToOpenTime));
		}

		protected override Func<bool> OnOpen() {
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			string message =
				"Hello, I'm here to take you to the shelter. Your entry has been approved and it's time for you to come with me. " +
				"The shelter will provide you with a safe haven from the dangers outside. " +
				"Please gather any essential items you wish to bring with you, and we'll depart shortly. " +
				"<color=yellow>Congratulations on being granted entry, and stay safe</color>.";
			AudioSource audioSource = AudioSystem.Singleton.Play2DSound("door_open");
			
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			
			speaker.Speak(message,
				bodyInfo.VoiceTag.VoiceGroup,
				"Military Officer", 1, OnFinish,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
			
			
			
			
			
			return () => onClickPeepholeSpeakEnd;
		}

		private void OnFinish(Speaker obj) {
			onClickPeepholeSpeakEnd = true;
			
			LoadCanvas.Singleton.Load(() => {
				this.GetModel<GameStateModel>().GameState.Value = GameState.End;
				DieCanvas.Singleton.Show("Game Ends", "You are saved!", 4, "Shelter_End", false, false);
			});
		}

		public override void OnEventEnd() {
			
		}

		protected override void OnNotOpen() {
			base.OnNotOpen();
			if (refuseToOpenTime <= 3) {
				refuseToOpenTime++;
				OnMissed();
			}
		}
	}
}