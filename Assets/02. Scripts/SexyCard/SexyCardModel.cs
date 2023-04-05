using Crosstales.RTVoice.Model.Enum;
using UnityEngine;

namespace _02._Scripts.SexyCard {
	public class SexyCardModel: AbstractSavableModel {
		[field: ES3Serializable]
		public Sprite SexyCardSprite { get; set; }
		
		[field: ES3Serializable]
		public long SexyPersonID { get; set; }
		
		[field: ES3Serializable]
		public string SexyPersonPhoneNumber { get; set; }
		
		[field: ES3Serializable]
		public Gender SexyPersonGender { get; set; }

		[field: ES3Serializable] public bool IsSexyPersonAvailable = true;
	}
}