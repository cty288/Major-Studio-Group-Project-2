using _02._Scripts.Poster;
using MikroFramework.Architecture;

namespace _02._Scripts.GameEvents.Merchant {
	public struct OnMerchantAdEventGenerated {
		public string posterID;
	}
	public class MerchantGeneratorAdEvent : GameEvent {
		public override GameEventType GameEventType { get; } = GameEventType.General;
		public override float TriggerChance { get; } = 1;
		
		public MerchantGeneratorAdEvent(TimeRange timeRange) : base(timeRange){}

		public MerchantGeneratorAdEvent() : base(){}
		public override void OnStart() {
			
		}

		public override EventState OnUpdate() {
			string id = this.GetModel<PosterModel>().AddPoster(new MerchantAdPoster(), false);
			this.SendEvent(new OnMerchantAdEventGenerated() {
				posterID = id
			});
			return EventState.End;
		}

		public override void OnEnd() {
			
		}

		public override void OnMissed() {
			
		}
	}
}