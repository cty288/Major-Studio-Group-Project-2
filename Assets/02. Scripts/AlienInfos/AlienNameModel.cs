namespace _02._Scripts.AlienInfos {
	public class AlienNameModel: AbstractSavableModel {
		[field: ES3Serializable] public string AlienName { get; set; } = "monster";
	}
}