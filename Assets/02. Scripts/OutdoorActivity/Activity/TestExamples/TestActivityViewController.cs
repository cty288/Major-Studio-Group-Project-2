using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestActivityViewController : ActivityViewController<TestActivity> {
	protected override void Awake() {
		base.Awake();
		Debug.Log("Hello");
	}
}
