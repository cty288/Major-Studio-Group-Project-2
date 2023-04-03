using MikroFramework.Architecture;
using UnityEngine;

namespace _02._Scripts.Dog {
	public struct OnMissingDogReward {
		public int FoodCount;
	}
	public class MissingDogRewardEvent: GameEvent {
		[field: ES3Serializable] public override GameEventType GameEventType { get; } = GameEventType.General;
		[field: ES3Serializable] public override float TriggerChance { get; } = 1;
		public override void OnStart() {
			
		}
		
		public MissingDogRewardEvent(): base(){}

		public MissingDogRewardEvent(TimeRange startTimeRange) : base(startTimeRange) {
			
		}
		public override EventState OnUpdate() {
			this.SendEvent<OnMissingDogReward>(new OnMissingDogReward() {
				FoodCount = 2
			});
			
			return EventState.End;
		}

		public override void OnEnd() {
		
		}

		public override void OnMissed() {
			
		}
	}
}