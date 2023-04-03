using UnityEngine;

namespace _02._Scripts.SexyCard {
	public class SexyCardModel: AbstractSavableModel {
		[field: ES3Serializable]
		public Sprite SexyCardSprite { get; set; }
		
		[field: ES3Serializable]
		public BodyInfo SexyPerson { get; set; }
		
		[field: ES3Serializable]
		public string SexyPersonPhoneNumber { get; set; }
	}
}