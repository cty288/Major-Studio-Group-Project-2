using MikroFramework.Architecture;

namespace _02._Scripts.Cat {

	public struct OnCatSpawn {
		
	}
	public class CatSpawnEvent : GameEvent {
		[field: ES3Serializable]
		public override GameEventType GameEventType { get; } = GameEventType.General;
		[field: ES3Serializable]
		public override float TriggerChance { get; } = 1;
		
		public CatSpawnEvent(): base(){}
		public CatSpawnEvent(TimeRange timeRange) : base(timeRange){}
		public override void OnStart() {
			
		}

		public override EventState OnUpdate() {
			this.SendEvent<OnCatSpawn>();
			return EventState.End;
		}

		public override void OnEnd() {
			
		}

		public override void OnMissed() {
			
		}
	}
}