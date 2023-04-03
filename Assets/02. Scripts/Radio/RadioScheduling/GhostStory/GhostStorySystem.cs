using System.Collections.Generic;
using System.Linq;
using _02._Scripts.GameTime;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;

namespace _02._Scripts.Radio.RadioScheduling.GhostStory {
	public class GhostStorySystem: AbstractSystem {
		protected GhostStoryModel ghostStoryModel;
		protected RadioDialogueDataModel radioDialogueDataModel;
		
		private RadioSchedulingModel radioSchedulingModel;
		//private DayOfWeek importantNewsPaperDay = 0;
		private GameTimeModel gameTimeModel;
		private ImportantNewspaperModel importantNewspaperModel;
		protected override void OnInit() {
			ghostStoryModel = this.GetModel<GhostStoryModel>();
			radioDialogueDataModel = this.GetModel<RadioDialogueDataModel>();
			gameTimeModel = this.GetModel<GameTimeModel>();
			radioSchedulingModel = this.GetModel<RadioSchedulingModel>();
			importantNewspaperModel = this.GetModel<ImportantNewspaperModel>();
			
			
			this.RegisterEvent<OnGhostStoryDataInit>(OnGhostStoryDataInit);
			
			this.RegisterEvent<OnNewDay>(OnNewDay);
			
		}

		private void OnNewDay(OnNewDay e) {
			if (e.Date.DayOfWeek == importantNewspaperModel.ImportantNewsPaperDay) {
				GhostStoryProgram(gameTimeModel.Week);
			}
		}

		private void GhostStoryProgram(int week) {
			bool hasGhostStoryThisWeek = Random.Range(0, 100) > 40;
			if (!hasGhostStoryThisWeek) {
				return;
			}
			
			RadioDialogueContent data = ghostStoryModel.GetRandomContent();
			if (data != null) {
				TimeRange timeRange1 = radioSchedulingModel.AddToScheduled(
					new RadioScheduleInfo(new RadioProgramNameInfo(RadioProgramType.GhostStory, "Ghost Story"),
						RadioChannel.FM92, null, 90),
					gameTimeModel.CurrentTime.Value.AddDays(Random.Range(1, 3)), gameTimeModel.NightTimeStart);

				int retryCount = 50;
				while (retryCount > 0 && timeRange1 == null) {
					timeRange1 = radioSchedulingModel.AddToScheduled(
						new RadioScheduleInfo(new RadioProgramNameInfo(RadioProgramType.GhostStory, "Ghost Story"),
							RadioChannel.FM92, null, 90),
						gameTimeModel.CurrentTime.Value.AddDays(Random.Range(1, 3)), gameTimeModel.NightTimeStart);
					retryCount--;
				}
				
				GameEventSystem gameEventSystem = this.GetSystem<GameEventSystem>();
				
				if (timeRange1 != null) {
					gameEventSystem.AddEvent(new GhostStoryRadioEvent(timeRange1, data));
					
					TimeRange timeRange2 = radioSchedulingModel.AddToScheduled(
						new RadioScheduleInfo(new RadioProgramNameInfo(RadioProgramType.GhostStory, "Ghost Story (Replayed)"),
							RadioChannel.FM92, null, 90),
						gameTimeModel.CurrentTime.Value.AddDays(Random.Range(4, 7)), gameTimeModel.NightTimeStart);

					if (timeRange2 != null) {
						gameEventSystem.AddEvent(new GhostStoryRadioEvent(timeRange2, data));
					}
				}
			}
		}

		

		private void OnGhostStoryDataInit(OnGhostStoryDataInit e) {
			List<string> keys = radioDialogueDataModel.HotUpdateInfo.Keys.Where((s => s.StartsWith("GhostStories".ToLower())))
				.ToList();

			foreach (string key in keys) {
				ghostStoryModel.AddContent(radioDialogueDataModel.HotUpdateInfo[key]);
			}
		}
	}
}