using System;
using MikroFramework.BindableProperty;

namespace _02._Scripts.GameTime {
	public class GameTimeModel: AbstractSavableModel{
		[field: ES3Serializable]
		public int Day { get; protected set; }
		
		[ES3Serializable]
		public BindableProperty<DateTime> CurrentTime = new BindableProperty<DateTime>(new DateTime(2022, 11, 12, 22, 0, 0));

		public void AddDay() {
			
			Day++;
		}
	}
}