using System;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace _02._Scripts.GameTime {
	public class GameTimeModel: AbstractSavableModel {
		[field: ES3Serializable] public int Day { get; protected set; } = -1;
		
		[ES3Serializable]
		public BindableProperty<DateTime> CurrentTime = new BindableProperty<DateTime>(new DateTime(2022, 11, 12, 22, 0, 0));

		protected DateTime DayStartTime = new DateTime(2022, 11, 13, 22, 0, 0);
		public int NightTimeStart { get; private set; } = 22;
		public int DayTimeStart { get; private set; } = 9;
    
		public int DayTimeEnd { get; private set; } = 17;
		
		public AnimationCurve GlobalTimeFreqCurve { get; set; }
		
		[ES3Serializable] private int week = 0;

		public int Week => week;
		public void AddDay(out bool isNewWeek) {
			Day++;
			isNewWeek = false;
			if(week==0 || Day%7==0) {
				week++;
				isNewWeek = true;
			}
			/*
			if (Day <= 0) {
				GlobalTimeFreq = 0.6f;
			}
			else {
				GlobalTimeFreq = 1f;
			}*/
		}


		public int GetWeek(int day) {
			return day / 7 + 1;
		}
		public DateTime GetDay(int day, bool startFromNight = true) {
			DateTime date = DayStartTime.AddDays(day);
			date = new DateTime(date.Year, date.Month, date.Day, startFromNight? NightTimeStart: DayTimeStart, 0, 0);
			return date;
		}
		
		public int GetDay(DateTime dateTime) {
			return (int) (dateTime - DayStartTime).TotalDays;
		}
	}
}