using System;
using MikroFramework.BindableProperty;

namespace _02._Scripts.GameTime {
	public class GameTimeModel: AbstractSavableModel {
		[field: ES3Serializable] public int Day { get; protected set; } = -1;
		
		[ES3Serializable]
		public BindableProperty<DateTime> CurrentTime = new BindableProperty<DateTime>(new DateTime(2022, 11, 12, 22, 0, 0));

		[field: ES3Serializable] public float GlobalTimeFreq { get; set; } = 2.5f;
		public void AddDay() {
			Day++;
			if (Day <= 0) {
				GlobalTimeFreq = 1f;
			}
			else {
				GlobalTimeFreq = 2.5f;
			}
		}
		
		
	}
}