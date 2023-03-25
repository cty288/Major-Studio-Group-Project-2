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
	
	public class RadioSchedulingSystem: AbstractSystem {
		private List<PermanentRadioProgramInfo> permanentRadioProgramInfos = new List<PermanentRadioProgramInfo>();
		private RadioSchedulingModel radioSchedulingModel;
		private DayOfWeek importantNewsPaperDay = 0;
		private GameTimeModel gameTimeModel;
		protected override void OnInit() {
			RegisterPermanentRadioPrograms();
			int eventDay =
				int.Parse(this.GetModel<HotUpdateDataModel>().GetData("ImportantNewsDay").values[0]);
			gameTimeModel = this.GetModel<GameTimeModel>();
			importantNewsPaperDay = gameTimeModel.GetDay(eventDay).DayOfWeek;
			this.RegisterEvent<OnNewDay>(OnNewDay);
			radioSchedulingModel = this.GetModel<RadioSchedulingModel>();
			
		}

		private void OnNewDay(OnNewDay e) {
			if (e.Date.DayOfWeek == importantNewsPaperDay) {
				BuildThisWeekAllRadioPrograms(e.Date);
			}
		}

		
		private void RegisterPermanentRadioPrograms() {
			permanentRadioProgramInfos.Add(new PermanentRadioProgramInfo(RadioProgramType.DailyDeadBody,
				"Dead Body Report", 70, RadioChannel.FM96, new Vector2Int(7, 8), true));

			permanentRadioProgramInfos.Add(new PermanentRadioProgramInfo(RadioProgramType.Ads,
				"Ads", 30, RadioChannel.FM100, new Vector2Int(2, 5), true));

			permanentRadioProgramInfos.Add(new PermanentRadioProgramInfo(RadioProgramType.Announcement, "Announcement",
				30, RadioChannel.FM100, new Vector2Int(2, 5), false));
			
			permanentRadioProgramInfos.Reverse();

		}

		public void AddRadioProgram(RadioProgramNameInfo nameInfo, int lengthInMinutes, RadioChannel channel,
			DateTime date, [CanBeNull] TimeRange preferredTimeRange) {
			RadioScheduleInfo info = new RadioScheduleInfo(nameInfo, channel, preferredTimeRange, lengthInMinutes);
			radioSchedulingModel.AddToUnscheduled(info, date.Date, gameTimeModel.NightTimeStart);
		}
		
		private void BuildThisWeekAllRadioPrograms(DateTime date) {
			date = date.Date;
			Dictionary<DateTime, List<RadioScheduleInfo>> permanentRadioPrograms =
				GenerateWeeklyPermanentRadioProgramInfo(date);
			
			for (int i = 0; i < 7; i++) {
				DateTime currentDate = date.AddDays(i);
				List<RadioScheduleInfo> permanentRadioScheduleInfos = permanentRadioPrograms[currentDate];
				foreach (RadioScheduleInfo programInfo in permanentRadioScheduleInfos) {
					radioSchedulingModel.AddToUnscheduled(programInfo, currentDate, gameTimeModel.NightTimeStart);
				}
				
				radioSchedulingModel.LoadToScheduled(currentDate);
			}
		}

		private Dictionary<DateTime,List<RadioScheduleInfo>> GenerateWeeklyPermanentRadioProgramInfo(DateTime date) {
			Dictionary<DateTime, List<RadioScheduleInfo>> result = new Dictionary<DateTime, List<RadioScheduleInfo>>();
			for (int i = 0; i < 7; i++) {
				DateTime currentDate = date.AddDays(i);
				result.Add(currentDate, new List<RadioScheduleInfo>());
			}

			List<int> days = new List<int>() {0, 1, 2, 3, 4, 5, 6};
			days.CTShuffle();
			
			
			
			
			foreach (PermanentRadioProgramInfo info in permanentRadioProgramInfos) {
				int dayCount = Random.Range(info.DaysPerWeek.x, info.DaysPerWeek.y);
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