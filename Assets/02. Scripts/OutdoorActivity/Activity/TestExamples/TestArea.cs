using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestArea : Place {
	[field: ES3Serializable]
	public override string Name { get; } = "TestArea";
	
	
	public TestArea() : base(){}
	
	protected override void OnRegisterActivities() {
		Debug.Log("Registering activities");
		RegisterActivity(new TestActivity());
		RegisterActivity(new TestActivity2());
		EnablePermanentActivity("TestArea_TestActivity");
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
