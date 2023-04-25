using System;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using NHibernate.Mapping;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02._Scripts.CultEnding {
	public struct OnCultistLetterSpawned {
		public List<string> contents;
	}
	public class CultistLetterSpawnEvent: GameEvent {
		[field: ES3Serializable] public override GameEventType GameEventType { get; } = GameEventType.General;
		protected CultEndingModel cultEndingModel;
		public override float TriggerChance { get; } = 1;

		public CultistLetterSpawnEvent() : base() {
			cultEndingModel = this.GetModel<CultEndingModel>();
		}

		public CultistLetterSpawnEvent(TimeRange timeRange) : base(timeRange) {
			cultEndingModel = this.GetModel<CultEndingModel>();
			Debug.Log("Cult letter will spawn at " + timeRange.StartTime);
		}
		
		public override void OnStart() {
			
		}

		public override EventState OnUpdate() {
			List<string> letterContents = this.GetModel<CultEndingModel>().PopContents(2);
			this.SendEvent<OnCultistLetterSpawned>(new OnCultistLetterSpawned() {
				contents = letterContents
			});
			return EventState.End;
		}

		public override void OnEnd() {
			if (cultEndingModel.NotAddedCultLetterContents.Count <= 0) {
				return;
			}
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime nextTime = gameTimeModel.GetDay(gameTimeModel.Day + Random.Range(2, 4));
			gameEventSystem.AddEvent(new CultistLetterSpawnEvent(new TimeRange(nextTime)));
		}

		public override void OnMissed() {
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime nextTime = gameTimeModel.GetDay(gameTimeModel.Day + Random.Range(2, 4));
			gameEventSystem.AddEvent(new CultistLetterSpawnEvent(new TimeRange(nextTime)));
		}
	}
}