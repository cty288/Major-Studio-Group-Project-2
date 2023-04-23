using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using Random = UnityEngine.Random;

namespace _02._Scripts.GameEvents.BountyHunter {
	public class BountyHunterModel: AbstractSavableModel, ICanGetModel {
		[field: ES3Serializable]
		public bool ContactedBountyHunter { get; set; } = false;
		[field: ES3Serializable]
		public string PhoneNumber { get; private set; } = "";

		[field: ES3Serializable]
		public bool QuestBodyClueAllHappened { get; set; } = false;
		[field: ES3Serializable]
		public bool QuestFinished { get; set; } = false;
		[field: ES3Serializable]
		public int FalseClueCount { get; set; } = 0;
		[field: ES3Serializable]
		public int MaxFalseClueCountForQuest = 3;
		[field: ES3Serializable]
		public int FalseClueCountForPolice = 3;
		[field: ES3Serializable]
		public DateTime NextAvailableDate { get; set; }
		[field: ES3Serializable]
		public bool IsInJail { get; set; }
		
		private GameTimeModel gameTimeModel;

		protected override void OnInit() {
			base.OnInit();
			
			if(PhoneNumber == "") {
				PhoneNumber = PhoneNumberGenor.GeneratePhoneNumber(6);
			}
			
			
		}
	}
	
}