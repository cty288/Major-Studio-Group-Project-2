using System;
using _02._Scripts.GameTime;
using _02._Scripts.Poster.PosterEvents;
using MikroFramework.Architecture;
using Random = UnityEngine.Random;

namespace _02._Scripts.Poster {
	public class PosterSystem: AbstractSystem {
		private GameTimeModel gameTimeModel;
		protected override void OnInit() {
			this.RegisterEvent<OnNewDay>(OnNewDay);
			gameTimeModel = this.GetModel<GameTimeModel>();
		}

		private void OnNewDay(OnNewDay e) {
			if (e.Day == 1) {
				DateTime currentTime = gameTimeModel.CurrentTime.Value;
				DateTime nextTime = currentTime.AddDays(2);
				nextTime = new DateTime(nextTime.Year, nextTime.Month, nextTime.Day,
					UnityEngine.Random.Range(gameTimeModel.NightTimeStart, 24), Random.Range(5, 40), 0);
				DateTime nextEndTime = nextTime.AddMinutes(20);


				this.GetSystem<GameEventSystem>().AddEvent(new AdPosterDeliverEvent(new TimeRange(nextTime, nextEndTime), 0, AdPosterDeliverEvent.GetPosterDelivererBody()));
			}
		}
	}
}