using System;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace _02._Scripts.Telephone {
	public class TelephoneFixedEvent: GameEvent {
		[field: ES3Serializable]
		public override GameEventType GameEventType { get; } = GameEventType.General;
		[field: ES3Serializable]
		public override float TriggerChance { get; } = 1;

		
		
		public TelephoneFixedEvent() {
			
		}
		
		
		public TelephoneFixedEvent(TimeRange timeRange) : base(timeRange) {
			
		}
		
		
		public override void OnStart() {
			
		}

		public override EventState OnUpdate() {
			this.GetSystem<TelephoneSystem>().IsBroken = false;
			return EventState.End;
		}

		public override void OnEnd() {
			DateTime time = gameTimeManager.CurrentTime.Value.AddMinutes(Random.Range(10, 20));
			gameEventSystem.AddEvent(new TelephoneFixedRadioEvent(new TimeRange(time),
				AudioMixerList.Singleton.AudioMixerGroups[1]));
		}

		public override void OnMissed() {
			
		}
	}
}