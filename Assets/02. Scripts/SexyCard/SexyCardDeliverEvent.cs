using MikroFramework.Architecture;
using UnityEngine;

namespace _02._Scripts.SexyCard {
	public struct OnSexyCardDelivered {
		
	}
	public class SexyCardDeliverEvent: GameEvent {
		[field: ES3Serializable]
		public override GameEventType GameEventType { get; } = GameEventType.General;
		[field: ES3Serializable]
		public override float TriggerChance { get; } = 1;
		
		public SexyCardDeliverEvent(TimeRange timeRange) : base(timeRange) {
			Debug.Log("Sexy card will be delivered at " + timeRange.StartTime);
		}
		
		public SexyCardDeliverEvent(): base(){}
		public override void OnStart() {
			
		}

		public override EventState OnUpdate() {
			this.SendEvent<OnSexyCardDelivered>(new OnSexyCardDelivered() {
				
			});
			return EventState.End;
		}

		public override void OnEnd() {
		
		}

		public override void OnMissed() {
			
		}
	}
}