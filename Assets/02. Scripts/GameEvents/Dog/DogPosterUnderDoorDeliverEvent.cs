using MikroFramework.Architecture;

namespace _02._Scripts.GameEvents.Dog {
	public struct OnDogPosterUnderDoorDelivered {
		
	}
	public class DogPosterUnderDoorDeliverEvent: GameEvent {
		[field: ES3Serializable]
		public override GameEventType GameEventType { get; } = GameEventType.General;
		[field: ES3Serializable]
		public override float TriggerChance { get; } = 1;

		
		private DogModel dogModel;
		public DogPosterUnderDoorDeliverEvent() : base() {
			dogModel = this.GetModel<DogModel>();
		}

		public DogPosterUnderDoorDeliverEvent(TimeRange timeRange) : base(timeRange) {
			dogModel = this.GetModel<DogModel>();
		}
		
		public override void OnStart() {
			
		}

		public override EventState OnUpdate() {
			if(!dogModel.SentDogBack && dogModel.isDogAlive) {
				this.SendEvent<OnDogPosterUnderDoorDelivered>();
			}

			return EventState.End;
		}

		public override void OnEnd() {
			
		}

		public override void OnMissed() {
			
		}
	}
}