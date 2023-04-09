using MikroFramework.Architecture;
using UnityEngine;

namespace _02._Scripts.Dog {
	public struct OnRewardPackage {
		public GoodsInfo GoodsInfo;
		public string NoteName;
		public string NoteText;
	}
	public class GoodsRewardEvent: GameEvent {
		[field: ES3Serializable] public override GameEventType GameEventType { get; } = GameEventType.General;
		[field: ES3Serializable] public override float TriggerChance { get; } = 1;
		
		[field: ES3Serializable] private GoodsInfo goodsInfo;
		
		[field: ES3Serializable] private string noteContent;
		[field: ES3Serializable] private string noteName;
		public override void OnStart() {
			
		}
		
		public GoodsRewardEvent(): base(){}

		public GoodsRewardEvent(TimeRange startTimeRange, GoodsInfo goodsInfo, string noteContent, string noteName) : base(startTimeRange) {
			this.goodsInfo = goodsInfo;
			this.noteContent = noteContent;
			this.noteName = noteName;
		}
		public override EventState OnUpdate() {
			this.SendEvent<OnRewardPackage>(new OnRewardPackage() {
				GoodsInfo = goodsInfo,
				NoteName = noteName,
				NoteText = noteContent
			});
			
			return EventState.End;
		}

		public override void OnEnd() {
		
		}

		public override void OnMissed() {
			
		}
	}
}