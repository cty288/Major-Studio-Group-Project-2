﻿using System;
using System.Linq;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;

namespace _02._Scripts.ImportantNewspaper {

	public struct OnImportantNewspaperGenerated {
		public int Issue;
	}
	public class ImportantNewspaperSystem: AbstractSystem {
		
		private GameTimeModel gameTimeModel;
		
		private ImportantNewspaperModel importantNewspaperModel;
		protected override void OnInit() {
			int eventDay =
				int.Parse(this.GetModel<HotUpdateDataModel>().GetData("ImportantNewsDay").values[0]);
			gameTimeModel = this.GetModel<GameTimeModel>();
			importantNewspaperModel = this.GetModel<ImportantNewspaperModel>();
			importantNewspaperModel.NewspaperStartDay = eventDay;
			importantNewspaperModel.NewspaperStartDay = eventDay;
			
			this.RegisterEvent<OnNewDay>(OnNewDay);
		}

		private void OnNewDay(OnNewDay e) {
			if (e.Day == 0) {
				importantNewspaperModel.AddPageToNewspaper(1,
					this.GetModel<ImportantNewsTextModel>().GetInfo("ManDead"), 0);
			}
			if(importantNewspaperModel.newsDays.Contains(e.Date.DayOfWeek) && e.Day>=1) {
				this.SendEvent(new OnImportantNewspaperGenerated() {Issue = importantNewspaperModel.GetIssueForNews(e.Day, e.Date)-1});
			}
		}
	}
}