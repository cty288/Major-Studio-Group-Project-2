using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public class IndoorMapViewController : AbstractMikroController<MainGame> {
	private void Awake() {
		gameObject.SetActive(false);
		this.GetModel<OutdoorActivityModel>().HasMap.RegisterWithInitValue(OnHasMapChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
	}

	private void OnHasMapChanged(bool hasMap) {
		gameObject.SetActive(hasMap);
	}
}
