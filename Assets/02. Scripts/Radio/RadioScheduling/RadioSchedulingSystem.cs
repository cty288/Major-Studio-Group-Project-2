using System;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using Crosstales;
using JetBrains.Annotations;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02._Scripts.Radio.RadioScheduling {
	public enum RadioProgramType {
		DailyDeadBody,
		Ads,
		Announcement,
		Music,
	}

	public class RadioProgramNameInfo {
		public RadioProgramType Type;
		public string DisplayName;
		
		public RadioProgramNameInfo(RadioProgramType type, string displayName) {
			Type = type;
			DisplayName = displayName;
		}
	}

	public class PermanentRadioProgramInfo {
		public RadioProgramNameInfo NameInfo;
		public int LengthInMinutes;
		public RadioChannel Channel;
		public Vector2Int DaysPerWeek;
		public bool FixedTimeInWeek;
		
		public PermanentRadioProgramInfo(RadioProgramType type, string displayName, int lengthInMinutes, RadioChannel channel, Vector2Int daysPerWeek,
			bool fixedTimeInWeek) {
			NameInfo = new RadioProgramNameInfo(type, displayName);
			LengthInMinutes = lengthInMinutes;
			Channel = channel;
			DaysPerWeek = daysPerWeek;
			FixedTimeInWeek = fixedTimeInWeek;
		}
	}

	public struct OnWeeklyRadioScheduleGenerated {
		public Dictionary<DateTime, Dictionary<RadioChannel, List<RadioScheduleInfo>>> Schedule;
	}

	public class ImportantNewspaperRadioSchedulePage: IImportantNewspaperPageContent {
		public Dictionary<DateTime, Dictionary<RadioChannel, List<RadioScheduleInfo>>> Schedule =
			new Dictionary<DateTime, Dictionary<RadioChannel, List<RadioScheduleInfo>>>();
		
		public ImportantNewspaperRadioSchedulePage(Dictionary<DateTime, Dictionary<RadioChannel, List<RadioScheduleInfo>>> schedule) {
			Schedule = schedule;
		}

		public ImportantNewspaperRadioSchedulePage() {
			//Schedule = new Dictionary<DateTime, Dictionary<RadioChannel, List<RadioScheduleInfo>>>();
		}
		public List<IImportantNewspaperPageContent> GetPages() {
			int contentPerPage = 20;
			int currentContentCount = 0;
			List<IImportantNewspaperPageContent> pages = new List<IImportantNewspaperPageContent>();
			
			//first calculete each day's content count
			Dictionary<DateTime, int> dayContentCount = new Dictionary<DateTime, int>();
			foreach (DateTime date in Schedule.Keys) {
				float count = 0;
				Dictionary<RadioChannel, List<RadioScheduleInfo>> channelSchedule = Schedule[date];
				foreach (RadioChannel radioChannel in channelSchedule.Keys) {
					count += 1.5f;
					foreach (RadioScheduleInfo radioScheduleInfo in channelSchedule[radioChannel]) {
						count += 1f;
					}
				}

				dayContentCount.Add(date, Mathf.RoundToInt(count));
			}
			
			//then divide them into pages. Schedule of the same day will be put into the same page. So if current count + next day's count > contentPerPage, then create a new page
			ImportantNewspaperRadioSchedulePage currentPage = new ImportantNewspaperRadioSchedulePage();
			foreach (DateTime date in Schedule.Keys) {
				if (currentContentCount + dayContentCount[date] > contentPerPage && currentContentCount != 0) {
					pages.Add(currentPage);
					currentPage = new ImportantNewspaperRadioSchedulePage();
					currentContentCount = 0;
				}

				currentPage.Schedule.Add(date, Schedule[date]);
				currentContentCount += dayContentCount[date];
			}
			
			if (currentPage.Schedule.Count > 0) {
				pages.Add(currentPage);
			}
			
			//pages.Add(currentPage);

			return pages;
		}
	}
	
	public class RadioSchedulingSystem: AbstractSystem {
		private List<PermanentRadioProgramInfo> permanentRadioProgramInfos = new List<PermanentRadioProgramInfo>();
		private RadioSchedulingModel radioSchedulingModel;
		//private DayOfWeek importantNewsPaperDay = 0;
		private GameTimeModel gameTimeModel;
		private ImportantNewspaperModel importantNewspaperModel;
		protected override void OnInit() {
			RegisterPermanentRadioPrograms();
			gameTimeModel = this.GetModel<GameTimeModel>();
			this.RegisterEvent<OnNewDay>(OnNewDay);
			radioSchedulingModel = this.GetModel<RadioSchedulingModel>();
			importantNewspaperModel = this.GetModel<ImportantNewspaperModel>();
		}

		private void OnNewDay(OnNewDay e) {
			if (e.Date.DayOfWeek == importantNewspaperModel.ImportantNewsPaperDay) {
				BuildThisWeekAllRadioPrograms(e.Date, 7, true);
			}

			if (e.Day == 1) {
				DateTime date = e.Date;
				BuildThisWeekAllRadioPrograms(e.Date, importantNewspaperModel.ImportantNewsPaperDay - date.DayOfWeek, false);
			}
			
		}

		
		private void RegisterPermanentRadioPrograms() {
			permanentRadioProgramInfos.Add(new PermanentRadioProgramInfo(RadioProgramType.DailyDeadBody,
				"Dead Body Report", 70, RadioChannel.FM96, new Vector2Int(7, 8), true));

			permanentRadioProgramInfos.Add(new PermanentRadioProgramInfo(RadioProgramType.Ads,
				"Ads", 45, RadioChannel.FM100, new Vector2Int(4, 8), true));

			permanentRadioProgramInfos.Add(new PermanentRadioProgramInfo(RadioProgramType.Announcement, "Announcement",
				45, RadioChannel.FM100, new Vector2Int(4, 8), false));

			permanentRadioProgramInfos.Add(new PermanentRadioProgramInfo(RadioProgramType.Music, "Music", 119,
				RadioChannel.FM104, new Vector2Int(7, 8), true));
			
			permanentRadioProgramInfos.Reverse();

		}

		public void AddRadioProgram(RadioProgramNameInfo nameInfo, int lengthInMinutes, RadioChannel channel,
			DateTime date, [CanBeNull] TimeRange preferredTimeRange) {
			RadioScheduleInfo info = new RadioScheduleInfo(nameInfo, channel, preferredTimeRange, lengthInMinutes);
			radioSchedulingModel.AddToUnscheduled(info, date.Date, gameTimeModel.NightTimeStart);
		}
		
		private void BuildThisWeekAllRadioPrograms(DateTime date, int weekDayCount, bool addToImportantNewspaper) {
			date = date.Date;
			Dictionary<DateTime, List<RadioScheduleInfo>> permanentRadioPrograms =
				GenerateWeeklyPermanentRadioProgramInfo(date, weekDayCount);
			
			for (int i = 0; i < weekDayCount; i++) {
				DateTime currentDate = date.AddDays(i);
				List<RadioScheduleInfo> permanentRadioScheduleInfos = permanentRadioPrograms[currentDate];
				foreach (RadioScheduleInfo programInfo in permanentRadioScheduleInfos) {
					radioSchedulingModel.AddToUnscheduled(programInfo, currentDate, gameTimeModel.NightTimeStart);
				}
				
				radioSchedulingModel.LoadToScheduled(currentDate);
			}

			var schedule = radioSchedulingModel.GetSchedule(date, weekDayCount);
			this.SendEvent(new OnWeeklyRadioScheduleGenerated() {
				Schedule = schedule
			});

			if (addToImportantNewspaper) {
				importantNewspaperModel.AddPageToNewspaper(gameTimeModel.Week,
					new ImportantNewspaperRadioSchedulePage(schedule));
			}
		}

		private Dictionary<DateTime,List<RadioScheduleInfo>> GenerateWeeklyPermanentRadioProgramInfo(DateTime date, int weekDayCount) {
			Dictionary<DateTime, List<RadioScheduleInfo>> result = new Dictionary<DateTime, List<RadioScheduleInfo>>();
			List<int> days = new List<int>();
			for (int i = 0; i < weekDayCount; i++) {
				DateTime currentDate = date.AddDays(i);
				result.Add(currentDate, new List<RadioScheduleInfo>());
				days.Add(i);
			}

			
			days.CTShuffle();
			
			
			
			
			foreach (PermanentRadioProgramInfo info in permanentRadioProgramInfos) {
				int dayCount = Random.Range(info.DaysPerWeek.x, info.DaysPerWeek.y);
				if (dayCount > weekDayCount) {
					dayCount = weekDayCount;
				}
				
				TimeRange timeRange = null;
				
				for (int i = 0; i < dayCount; i++) {
					int day = days[i];
					DateTime currentDate = date.AddDays(day);
					
					DateTime startNight = currentDate.Date.AddHours(gameTimeModel.NightTimeStart);
					DateTime endNight = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 23, 59, 59);


					
					if (i==0 || !info.FixedTimeInWeek) {
						timeRange = RadioSchedulingModel.GetRandomTimeRangeInTimeRange(
							new TimeRange(startNight, endNight), info.LengthInMinutes);
					}

					if (timeRange != null) {
						DateTime realNightStart = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, timeRange.StartTime.Hour, timeRange.StartTime.Minute, 0);
						DateTime realNightEnd = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day,
							timeRange.EndTime.Hour, timeRange.EndTime.Minute, 0);
						TimeRange realTimeRange = new TimeRange(realNightStart, realNightEnd);

						RadioScheduleInfo scheduleInfo = new RadioScheduleInfo(info.NameInfo, info.Channel, realTimeRange, info.LengthInMinutes);
						result[currentDate].Add(scheduleInfo);
					}
					
				}
				days.CTShuffle();
			}
			
			return result;
		}
		
	}
}