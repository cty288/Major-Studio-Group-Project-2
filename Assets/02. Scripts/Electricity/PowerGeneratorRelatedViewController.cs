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
		this.GetModel<ElectricityModel>().HasElectricityGenerator.RegisterWithInitValue(OnHasElectricityGeneratorChanged)
			.UnRegisterWhenGameObjectDestroyed(gameObject);
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
