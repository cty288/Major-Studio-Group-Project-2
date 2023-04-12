using _02._Scripts.BodyManagmentSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingGround : Place {
	[field: ES3Serializable]
	public override string Name { get; } = "HuntingGround";


    public HuntingGround() : base(){}
	
	protected override void OnRegisterActivities() {
		Debug.Log("Registering activities HuntingGround");
		
		RegisterActivity(new Hunting());
				
		EnablePermanentActivity("HuntingGround_Hunting");
	}

	protected override void OnInit() {
		Debug.Log("Initing");
		
	}

	protected override void OnAvailable() {
		Debug.Log("Available");
	}

	protected override void OnUnavailable() {
		Debug.Log("Unavailable");
	}

	protected override void OnDayStarts(DateTime day) {
		Debug.Log("Day starts");
	}

	protected override void OnDayEnds(DateTime day) {
		Debug.Log("Day ends");
    }

}
