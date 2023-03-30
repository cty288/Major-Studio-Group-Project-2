using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogModel : AbstractSavableModel
{
	[field: ES3Serializable]
	public bool isDogAlive { get; set; } = true;
	[field: ES3Serializable]
	public bool HaveDog { get; set; } = false;

	[field: ES3Serializable] 
	public string MissingDogPhoneNumber { get; set; } = "";

	[field: ES3Serializable] public bool SentDogBack { get; set; } = false;
	
	[field: ES3Serializable] public BodyInfo MissingDogBodyInfo { get; set; }
}
