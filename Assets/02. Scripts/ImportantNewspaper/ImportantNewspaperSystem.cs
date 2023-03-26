using System;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;

namespace _02._Scripts.ImportantNewspaper {

	public struct OnImportantNewspaperGenerated {
		public int Week;
	}
	public class ImportantNewspaperSystem: AbstractSystem {
		
		private GameTimeModel gameTimeModel;
		
		private ImportantNewspaperModel importantNewspaperModel;
		protected override void OnInit() {
			int eventDay =
				int.Parse(this.GetModel<HotUpdateDataModel>().GetData("ImportantNewsDay").values[0]);
			gameTimeModel = this.GetModel<GameTimeModel>();
			importantNewspaperModel = this.GetModel<ImportantNewspaperModel>();
			importantNewspaperModel.ImportantNewsPaperDay = gameTimeModel.GetDay(eventDay).DayOfWeek;
			
			this.RegisterEvent<OnNewDay>(OnNewDay);
		}

		private void OnNewDay(OnNewDay e) {
			if(e.Date.DayOfWeek == importantNewspaperModel.ImportantNewsPaperDay) {
				this.SendEvent(new OnImportantNewspaperGenerated() {Week = gameTimeModel.Week});
			}
		}
	}
}