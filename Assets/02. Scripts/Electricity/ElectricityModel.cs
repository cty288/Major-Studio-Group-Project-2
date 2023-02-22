using MikroFramework.BindableProperty;

namespace _02._Scripts.Electricity {
	public class ElectricityModel : AbstractSavableModel {
		[field: ES3Serializable]
		public BindableProperty<float> Electricity { get; private set; } = new BindableProperty<float>(0.6f);
		
		
		public bool HasElectricity() {
			return Electricity.Value > 0;
		}
	}
}