namespace _02._Scripts.GameEvents.Merchant {
	public class MerchantModel: AbstractSavableModel {
		[field: ES3Serializable]
		public string PhoneNumber { get; set; }
		[ES3Serializable]
		public int PhoneNumberGenerationDate { get; set; }

		
		protected override void OnInit() {
			base.OnInit();
		}
	}
}