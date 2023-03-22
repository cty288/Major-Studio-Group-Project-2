﻿using System;
using MikroFramework.BindableProperty;

namespace _02._Scripts.GameTime {
	public class GameTimeModel: AbstractSavableModel {
		[field: ES3Serializable] public int Day { get; protected set; } = -1;
		
		[ES3Serializable]
		public BindableProperty<DateTime> CurrentTime = new BindableProperty<DateTime>(new DateTime(2022, 11, 12, 22, 0, 0));

		protected DateTime DayStartTime = new DateTime(2022, 11, 13, 22, 0, 0);
		
		[field: ES3Serializable] public float GlobalTimeFreq { get; set; } = 1.2f;
		public void AddDay() {
			Day++;
			if (Day <= 0) {
				GlobalTimeFreq = 0.6f;
			}
			else {
				GlobalTimeFreq = 1.5f;
			}
		}
		
		public DateTime GetDay(int day) {
			return DayStartTime.AddDays(day);
		}
		
		
	}
}