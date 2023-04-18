namespace _02._Scripts.ArmyEnding {
	public class ArmyEndingModel : AbstractSavableModel {
		[field: ES3Serializable] public bool TriggeredStartPhoneCall { get; set; } = false;
	}
}