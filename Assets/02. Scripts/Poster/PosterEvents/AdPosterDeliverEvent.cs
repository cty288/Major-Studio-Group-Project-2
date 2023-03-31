using System;
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02._Scripts.Poster.PosterEvents {
	public class AdPosterDeliverEvent : PosterDeliverEvent<RawImagePoster> {
		[ES3Serializable] private int index;
		public AdPosterDeliverEvent(TimeRange timeRange, int index, BodyInfo bodyInfo) : base(null, timeRange, bodyInfo, 1) {
			this.Poster = new RawImagePoster(index);
			this.index = index;
			Debug.Log($"Next poster event {timeRange.StartTime}");
		}
		
		
		public  AdPosterDeliverEvent(): base(){}
		
		public override void OnMissed() {
			OnNotOpen();
		}

		public override void OnEventEnd() {
			
		}

		protected override void OnNotOpen() {
			base.OnNotOpen();
			
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime currentTime = gameTimeModel.CurrentTime.Value;
			DateTime nextTime = currentTime.AddDays(1);
			nextTime = new DateTime(nextTime.Year, nextTime.Month, nextTime.Day,
				Random.Range(gameTimeModel.NightTimeStart, 24), Random.Range(5, 40), 0);

			DateTime nextEndTime = nextTime.AddMinutes(20);
			gameEventSystem.AddEvent(new AdPosterDeliverEvent(new TimeRange(nextTime, nextEndTime), index,
				GetPosterDelivererBody()));
		}
		
		

		public static BodyInfo GetPosterDelivererBody() {
			BodyInfo bodyInfo = BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, false, false,
				new PosterKnockBehavior(4,
					Random.Range(3, 7), new List<string>()),
				MainGame.Interface.GetModel<BodyModel>().AvailableBodyPartIndices);
			return bodyInfo;
		}

		[field: ES3Serializable]
		public override RawImagePoster Poster { get; set; }

		protected override void OnFinishOpenDoor() {
			index++;
			if (index < PosterAssets.Singleton.PosterSprites.Count) {
				GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
				DateTime currentTime = gameTimeModel.CurrentTime.Value;
				DateTime nextTime = currentTime.AddDays(7);
				nextTime = new DateTime(nextTime.Year, nextTime.Month, nextTime.Day,
					Random.Range(gameTimeModel.NightTimeStart, 24), Random.Range(5, 40), 0);

				DateTime nextEndTime = nextTime.AddMinutes(20);
				gameEventSystem.AddEvent(new AdPosterDeliverEvent(new TimeRange(nextTime, nextEndTime), index,
					GetPosterDelivererBody()));
			}
		}
	}
}