using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameEvents.BountyHunter;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;

public class DebugPanel : AbstractMikroController<MainGame> {
	[SerializeField] private TMP_Text bountyHunterNumber;
	[SerializeField] private TMP_Text merchantNumber;
	private void Awake() {
		
	}

	private void Start() {
		bountyHunterNumber.text = "Bounty Hunter Number: " + this.GetModel<BountyHunterModel>().PhoneNumber;
		merchantNumber.text = "Merchant Number: " + this.GetSystem<MerchantSystem>().PhoneNumber;
	}
}
