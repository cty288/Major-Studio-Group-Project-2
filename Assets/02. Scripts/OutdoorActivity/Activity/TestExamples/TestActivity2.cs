using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ES3Serializable]
public class TestActivity2 : Activity {
	[field: ES3Serializable]
	public override string Name { get; protected set; } = "TestArea_TestActivity2";
	[field: ES3Serializable]
	public override string SceneAssetName { get; } = "TestArea_TestActivity2";
	
	public TestActivity2(): base(){}
	protected override void OnInit() {
		Debug.Log("TestActivity OnInit");
	}

	protected override void OnDayStarts(DateTime date) {
		Debug.Log("TestActivity OnDayStarts");
	}

	protected override void OnDayEnds(DateTime date) {
		Debug.Log("OnDayEnds");
	}

	protected override void OnEnterPlayer() {
		Debug.Log("OnEnterPlayer");
	}

	protected override void OnLeavePlayer() {
		Debug.Log("OnLeavePlayer");
	}

	protected override void OnAvailable() {
		Debug.Log("TestActivity is available");
	}

	protected override void OnUnavailable() {
		Debug.Log("TestActivity is unavailable");
	}
}
