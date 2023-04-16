using MikroFramework.BindableProperty;

namespace _02._Scripts.SurvivalGuide {
	public class SurvivalGuideModel: AbstractSavableModel {
		[field: ES3Serializable]
		public BindableProperty<bool> ReceivedSurvivalGuideBefore { get; set; } = new BindableProperty<bool>(false);
	}
}