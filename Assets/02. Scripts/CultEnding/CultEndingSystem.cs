using System;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using Random = UnityEngine.Random;

namespace _02._Scripts.CultEnding {
	public class CultEndingSystem: AbstractSystem {
		protected ImportantNewspaperModel importantNewsModel;
		protected ImportantNewsTextModel importantNewsTextModel;
		protected override void OnInit() {
			this.RegisterEvent<OnNewDay>(OnNewDay);
			importantNewsModel = this.GetModel<ImportantNewspaperModel>();
			importantNewsTextModel = this.GetModel<ImportantNewsTextModel>();
		}

		private void OnNewDay(OnNewDay e) {
			if (e.Day == 0) {
				int motherNews1Day = Random.Range(6, 9);
				GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
				DateTime date = gameTimeModel.GetDay(motherNews1Day);


				int issue1Num = importantNewsModel.GetIssueForNews(motherNews1Day, date);
				importantNewsModel.AddPageToNewspaper(issue1Num,
					importantNewsTextModel.GetInfo("MotherNews_1"));

				importantNewsModel.AddPageToNewspaper(issue1Num + 1,
					importantNewsTextModel.GetInfo("MotherNews_2"));
			}
		}
	}
}