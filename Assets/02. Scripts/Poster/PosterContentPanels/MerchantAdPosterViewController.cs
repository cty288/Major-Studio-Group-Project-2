using System;
using _02._Scripts.GameEvents.Merchant;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;

namespace _02._Scripts.Poster.PosterContentPanels {
	public class MerchantAdPosterViewController: AbstractMikroController<MainGame> {
		private TMP_Text phoneNumText;
		private MerchantModel merchantModel;
		private TelephoneNumberRecordModel telephoneNumberRecordModel;
		private void Awake() {
			phoneNumText = transform.Find("PhoneNumber").GetComponent<TMP_Text>();
			merchantModel = this.GetModel<MerchantModel>();
			telephoneNumberRecordModel = this.GetModel<TelephoneNumberRecordModel>();
		}

		private void Start() {
			phoneNumText.text = merchantModel.PhoneNumber;
			telephoneNumberRecordModel.AddOrEditRecord(merchantModel.PhoneNumber + " ", "Power Generator Seller");
		}
	}
}