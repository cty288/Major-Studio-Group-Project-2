using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Random = UnityEngine.Random;

namespace _02._Scripts.Radio.RadioScheduling {

	public class RadioScheduleInfo {
		public RadioProgramNameInfo NameInfo;
		public RadioChannel Channel;
		public TimeRange PreferredTimeRange;
		public int Duration;
		
		public TimeRange ActualTimeRange;
		
		
		public RadioScheduleInfo(RadioProgramNameInfo nameInfo, RadioChannel channel, [CanBeNull]TimeRange preferredTimeRange,
			int duration) {
			NameInfo = nameInfo;
			Channel = channel;
			PreferredTimeRange = preferredTimeRange;
			Duration = duration;
		}
		
		public void SetActualTimeRange(TimeRange actualTimeRange) {
			ActualTimeRange = actualTimeRange;
		}

		public RadioScheduleInfo() {
			
		}
	}
	
	
	public class RadioSchedulingModel : AbstractSavableModel {
		[field: ES3Serializable]
		//day, channel, schedule info. Schedule info is sorted by time range start time
		protected Dictionary<DateTime, Dictionary<RadioChannel, List<RadioScheduleInfo>>> Schedule { get; set; } =
			new Dictionary<DateTime, Dictionary<RadioChannel, List<RadioScheduleInfo>>>();


		[field: ES3Serializable]
		public Dictionary<DateTime, Stack<RadioScheduleInfo>> Unscheduled { get; protected set; } =
			new Dictionary<DateTime, Stack<RadioScheduleInfo>>(); //day, schedule info.
		
		protected override void OnInit() {
			base.OnInit();
		}
		
		

		/// <summary>
		/// The last one has the highest priority
		/// </summary>
		/// <param name="scheduleInfo"></param>
		/// <param name="date"></param>
		/// <param name="nightTimeStartHour"></param>
		public void AddToUnscheduled(RadioScheduleInfo scheduleInfo, DateTime date, int nightTimeStartHour) {
			date = date.Date;
			if (scheduleInfo.PreferredTimeRange == null) {
				DateTime start = date.Date.AddHours(nightTimeStartHour);
				DateTime end = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
				scheduleInfo.PreferredTimeRange = new TimeRange(start, end);
			}
			
			if (!Unscheduled.ContainsKey(date)) {
				Unscheduled[date] = new Stack<RadioScheduleInfo>();
			}
			
			Unscheduled[date].Push(scheduleInfo);
		}

		public void LoadToScheduled(DateTime date) {
			if (!Unscheduled.ContainsKey(date)) return;
			date = date.Date;
			Stack<RadioScheduleInfo> unscheduled = Unscheduled[date];
			
			if (!Schedule.ContainsKey(date)) {
				Schedule.Add(date, new Dictionary<RadioChannel, List<RadioScheduleInfo>>());
			}
			
			
			while (unscheduled.Count > 0) {
				RadioScheduleInfo scheduleInfo = unscheduled.Pop();
				RadioChannel channel = scheduleInfo.Channel;
				Dictionary<RadioChannel, List<RadioScheduleInfo>> channelSchedule = Schedule[date];
				
				
				if (!channelSchedule.ContainsKey(scheduleInfo.Channel)) {
					channelSchedule.Add(scheduleInfo.Channel, new List<RadioScheduleInfo>());
				}

				TimeRange actualTime = CheckHasAvailableTime(channelSchedule[channel], scheduleInfo.PreferredTimeRange,
					scheduleInfo.Duration);
				if (actualTime != null) {
					scheduleInfo.SetActualTimeRange(actualTime);
					AddToSchedule(scheduleInfo, date);
				}
			}
		}

		public Dictionary<DateTime, Dictionary<RadioChannel, List<RadioScheduleInfo>>> GetSchedule(DateTime startDate,
			int days) {
			startDate = startDate.Date;
			Dictionary<DateTime, Dictionary<RadioChannel, List<RadioScheduleInfo>>> result =
				new Dictionary<DateTime, Dictionary<RadioChannel, List<RadioScheduleInfo>>>();
			
			for (int i = 0; i < days; i++) {
				DateTime date = startDate.AddDays(i);
				if (Schedule.ContainsKey(date)) {
					result.Add(date, Schedule[date]);
				}
			}

			return result;
		}


		protected void AddToSchedule(RadioScheduleInfo scheduleInfo, DateTime date) {
			date = date.Date;
			if (!Schedule.ContainsKey(date)) {
				Schedule.Add(date, new Dictionary<RadioChannel, List<RadioScheduleInfo>>());
			}
				
			Dictionary<RadioChannel, List<RadioScheduleInfo>> channelSchedule = Schedule[date];
			if (!channelSchedule.ContainsKey(scheduleInfo.Channel)) {
				channelSchedule.Add(scheduleInfo.Channel, new List<RadioScheduleInfo>());
			}

			InsertToSchedule(channelSchedule[scheduleInfo.Channel], scheduleInfo);
		}
		
		
		private void InsertToSchedule(List<RadioScheduleInfo> radioScheduleInfos, RadioScheduleInfo scheduleInfo) {
			//Schedule is sorted by time range start time. Insert the schedule info to the correct position
			//If the time range is overlapping, change the start time of the schedule info so that it is not overlapping.
			//Also make sure the start time is at least 10 minutes after the previous schedule info's end time.
			//If the end time of the schedule info has different day than the start time, change the end time so that it is the end of the day.
			//if the time range of the schedule is changed, make sure it is at least 20 minutes long, otherwise do not schedule it.
			
			
			//if the schedule is empty, just add it
			if (radioScheduleInfos.Count == 0) {
				radioScheduleInfos.Add(scheduleInfo);
				return;
			}

			TimeRange timeRange = scheduleInfo.ActualTimeRange;
			//if the schedule is not empty, find the correct position to insert the schedule info
			for (int i = 0; i < radioScheduleInfos.Count; i++) {
				RadioScheduleInfo currentScheduleInfo = radioScheduleInfos[i];
				if (timeRange.StartTime < currentScheduleInfo.ActualTimeRange.StartTime) {
					//if the schedule info's start time is before the current schedule info's start time, insert it before the current schedule info
					radioScheduleInfos.Insert(i, scheduleInfo);
					return;
				}
			}
			
			//if the schedule info's start time is after all the schedule infos' start time, add it to the end of the list
			radioScheduleInfos.Add(scheduleInfo);
			
			//check if the schedule info's time range is overlapping with the previous or next schedule info
			int scheduleInfoIndex = radioScheduleInfos.IndexOf(scheduleInfo);
			if (scheduleInfoIndex > 0) {
				RadioScheduleInfo previousScheduleInfo = radioScheduleInfos[scheduleInfoIndex - 1];
				if (timeRange.StartTime < previousScheduleInfo.ActualTimeRange.EndTime) {
					//if the schedule info's start time is before the previous schedule info's end time, change the start time of the schedule info
					timeRange.StartTime = previousScheduleInfo.ActualTimeRange.EndTime.AddMinutes(10);
				}
			}
			
			if (scheduleInfoIndex < radioScheduleInfos.Count - 1) {
				RadioScheduleInfo nextScheduleInfo = radioScheduleInfos[scheduleInfoIndex + 1];
				if (timeRange.EndTime > nextScheduleInfo.ActualTimeRange.StartTime) {
					//if the schedule info's end time is after the next schedule info's start time, change the end time of the schedule info
					timeRange.EndTime = nextScheduleInfo.ActualTimeRange.StartTime.AddMinutes(-10);
				}
			}
			
			//check if the schedule info's time range is the same day
			if (timeRange.StartTime.Day != scheduleInfo.ActualTimeRange.EndTime.Day) {
				//if the schedule info's time range is not the same day, change the end time of the schedule info to the end of the day
				timeRange.EndTime = scheduleInfo.ActualTimeRange.StartTime.Date.AddDays(1).AddSeconds(-1);
			}
			
			//check if the schedule info's time range is at least 20 minutes long
			if (timeRange.EndTime.Subtract(scheduleInfo.ActualTimeRange.StartTime).TotalMinutes < 20) {
				//if the schedule info's time range is less than 20 minutes long, remove it from the schedule
				radioScheduleInfos.Remove(scheduleInfo);
			}
		}


		public bool CheckIsProgramPlaying(DateTime time, RadioChannel channel, RadioProgramType programType) {
			DateTime date = time.Date;
			if (Schedule.ContainsKey(date)) {
				Dictionary<RadioChannel, List<RadioScheduleInfo>> channelSchedule = Schedule[date];
				if (channelSchedule.ContainsKey(channel)) {
					List<RadioScheduleInfo> radioScheduleInfos = channelSchedule[channel];
					foreach (RadioScheduleInfo scheduleInfo in radioScheduleInfos) {
						if (scheduleInfo.NameInfo.Type == programType) {
							if (scheduleInfo.ActualTimeRange.StartTime <= time &&
							    scheduleInfo.ActualTimeRange.EndTime >= time) {
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		
		
		public DateTime FindNextAvailableTime(DateTime currentTime, RadioChannel channel, RadioProgramType programType) {
			DateTime date = currentTime.Date;
			foreach (DateTime scheduleDate in Schedule.Keys) {
				if(scheduleDate >= date) {
					Dictionary<RadioChannel, List<RadioScheduleInfo>> channelSchedule = Schedule[scheduleDate];
					if (channelSchedule.ContainsKey(channel)) {
						List<RadioScheduleInfo> radioScheduleInfos = channelSchedule[channel];
						foreach (RadioScheduleInfo scheduleInfo in radioScheduleInfos) {
							if (scheduleInfo.NameInfo.Type == programType) {
								if (scheduleInfo.ActualTimeRange.StartTime > currentTime) {
									return scheduleInfo.ActualTimeRange.StartTime;
								}
							}
						}
					}
				}
			}

			return DateTime.MinValue;
		}
		
		protected TimeRange CheckHasAvailableTime(List<RadioScheduleInfo> radioScheduleInfos,
			TimeRange preferredTimeRange, int duration) {
			
			//in preferred time range, check if there is at least duration minutes available
			//if there is, return a random time range in the available time range
			//if there is not, return null
			
			//if the schedule is empty, directly return a random time range in the preferred time range
			if (radioScheduleInfos.Count == 0) {
				return GetRandomTimeRangeInTimeRange(preferredTimeRange, duration);
			}
			
			
			//if the schedule is not empty, check if there is at least duration minutes available
			//if there is, return a random time range in the available time range
			//if there is not, return null
			
			//check if the preferred time range is before the first schedule info's start time
			if (preferredTimeRange.StartTime < radioScheduleInfos[0].ActualTimeRange.StartTime) {
				//if the preferred time range is before the first schedule info's start time, check if there is at least duration minutes available
				
				TimeRange availableTimeRange = new TimeRange(preferredTimeRange.StartTime,
					preferredTimeRange.EndTime < radioScheduleInfos[0].ActualTimeRange.StartTime
						? preferredTimeRange.EndTime
						: radioScheduleInfos[0].ActualTimeRange.StartTime);
				
				if (availableTimeRange.Duration >= duration) {
					//if there is at least duration minutes available, return a random time range in the available time range
					return GetRandomTimeRangeInTimeRange(availableTimeRange, duration);
				}
			}
			
			//check if the preferred time range is after the last schedule info's end time
			if (preferredTimeRange.EndTime > radioScheduleInfos[radioScheduleInfos.Count - 1].ActualTimeRange.EndTime) {
				//if the preferred time range is after the last schedule info's end time, check if there is at least duration minutes available
				TimeRange availableTimeRange = new TimeRange(radioScheduleInfos[radioScheduleInfos.Count - 1].ActualTimeRange.EndTime < preferredTimeRange.StartTime
						? preferredTimeRange.StartTime
						: radioScheduleInfos[radioScheduleInfos.Count - 1].ActualTimeRange.EndTime,
					preferredTimeRange.EndTime);
				if (availableTimeRange.Duration >= duration) {
					//if there is at least duration minutes available, return a random time range in the available time range
					return GetRandomTimeRangeInTimeRange(availableTimeRange, duration);
				}
			}
			
			//check if there is at least duration minutes available between the schedule infos. Also should match the preferred time range
			for (int i = 0; i < radioScheduleInfos.Count - 1; i++) {
				RadioScheduleInfo currentScheduleInfo = radioScheduleInfos[i];
				RadioScheduleInfo nextScheduleInfo = radioScheduleInfos[i + 1];
				TimeRange availableTimeRange = new TimeRange(currentScheduleInfo.ActualTimeRange.EndTime,
					nextScheduleInfo.ActualTimeRange.StartTime);
				if (availableTimeRange.Duration >= duration) {
					if(preferredTimeRange.EndTime < availableTimeRange.StartTime || preferredTimeRange.StartTime > availableTimeRange.EndTime) {
						//if the preferred time range is not in the available time range, continue to check the next schedule info
						continue;
					}
					
					//if preferred time range is totally in the available time range, return a random time range in the available time range
					if (preferredTimeRange.StartTime >= availableTimeRange.StartTime && preferredTimeRange.EndTime <= availableTimeRange.EndTime) {
						return GetRandomTimeRangeInTimeRange(preferredTimeRange, duration);
					}
					
					//if preferred time range is partially in the available time range, return a random time range in the preferred time range
					if (preferredTimeRange.StartTime < availableTimeRange.StartTime && preferredTimeRange.EndTime > availableTimeRange.EndTime) {
						return GetRandomTimeRangeInTimeRange(availableTimeRange, duration);
					}
					
					//if preferred time range is partially in the available time range, return a random time range in the preferred time range
					if (preferredTimeRange.StartTime < availableTimeRange.StartTime) {
						return GetRandomTimeRangeInTimeRange(new TimeRange(availableTimeRange.StartTime, preferredTimeRange.EndTime), duration);
					}
					
					//if preferred time range is partially in the available time range, return a random time range in the preferred time range
					if (preferredTimeRange.EndTime > availableTimeRange.EndTime) {
						return GetRandomTimeRangeInTimeRange(new TimeRange(preferredTimeRange.StartTime, availableTimeRange.EndTime), duration);
					}
					
					
					
				}
			}

			return null;

		}

		public static TimeRange GetRandomTimeRangeInTimeRange(TimeRange preferredTimeRange, int duration) {
			if (preferredTimeRange.EndTime.Subtract(preferredTimeRange.StartTime).TotalMinutes < duration) {
				return null;
			}
			
			DateTime startTime = preferredTimeRange.StartTime.AddMinutes(Random.Range(0, (int) preferredTimeRange.EndTime.Subtract(preferredTimeRange.StartTime).TotalMinutes - duration));
			return new TimeRange(startTime, startTime.AddMinutes(duration));
		}
	}
}