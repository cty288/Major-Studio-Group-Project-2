using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Electricity;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public class PowerGeneratorRelatedViewController : AbstractMikroController<MainGame> {
	[SerializeField] private bool reverse = false;
	private void Awake() {
		this.RegisterEvent<OnPlayerResourceNumberChanged>(OnPlayerResourceNumberChanged);
		gameObject.SetActive(false);
	}

	private void OnPlayerResourceNumberChanged(OnPlayerResourceNumberChanged e) {
		if (e.GoodsInfo.Type == typeof(PowerGeneratorGoods)) {
			if (e.GoodsInfo.Count > 0) {
				OnHasElectricityGeneratorChanged(true);
			}else {
				OnHasElectricityGeneratorChanged(false);
			}

		}
	}

	private void OnHasElectricityGeneratorChanged(bool hasElectricityGenerator) {
		if (reverse) {
			gameObject.SetActive(!hasElectricityGenerator);
		}
		else {
			gameObject.SetActive(hasElectricityGenerator);
		}
		
	}
}
