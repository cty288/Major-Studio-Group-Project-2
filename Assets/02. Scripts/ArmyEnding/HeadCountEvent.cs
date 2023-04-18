using System;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02._Scripts.ArmyEnding {
	
	public class HeadCountEvent : GameEvent {
		public override GameEventType GameEventType { get; } = GameEventType.ArmyHeadcount;
		[field: ES3Serializable]
		public override float TriggerChance { get; } = 1f;

		protected GameTimeModel gameTimeModel;

		protected bool lightTurnedOn = false;

		public HeadCountEvent(TimeRange timeRange) : base(timeRange) {
			gameTimeModel = this.GetModel<GameTimeModel>();
		}

		public HeadCountEvent() : base() {
			gameTimeModel = this.GetModel<GameTimeModel>();
		}
		public override void OnStart() {
			this.RegisterEvent<OnLightFlash>(OnLightFlash);
		}

		private void OnLightFlash(OnLightFlash obj) {
			this.lightTurnedOn = true;
		}

		public override EventState OnUpdate() {
			DateTime currentTime = gameTimeModel.CurrentTime;
			if (currentTime.Hour == 23 && currentTime.Minute >= 59) {
				return EventState.End;
			}

			return EventState.Running;
		}

		public override void OnEnd() {
			OnEventEnds();
		}

		public override void OnMissed() {
			OnEventEnds();
		}

		private void OnEventEnds() {
			Debug.Log("Headcount event: light turned on? " + lightTurnedOn);
			this.UnRegisterEvent<OnLightFlash>(OnLightFlash);
			if (lightTurnedOn) {
				DateTime armyKnockEventTime = gameTimeModel.GetDay(gameTimeModel.Day + 1);
				armyKnockEventTime = armyKnockEventTime.AddMinutes(Random.Range(30, 60));
				
			
				gameEventSystem.AddEvent(new ArmyGunConfiscatedEvent(new TimeRange(armyKnockEventTime, armyKnockEventTime.AddMinutes(60)),
					ArmyGunConfiscatedEvent.GenerateArmyBodyInfo()));
			}
			else {
				DateTime armyHeadCountRadioReplyTime =gameTimeModel.GetDay(gameTimeModel.Day + 1);
				gameEventSystem.AddEvent(
					new ArmyHeadCountRadio(new TimeRange(armyHeadCountRadioReplyTime),
						AudioMixerList.Singleton.AudioMixerGroups[13]));
			}
		}
	}
}