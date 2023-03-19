using MikroFramework.BindableProperty;

namespace _02._Scripts.Electricity {
	public class ElectricityModel : AbstractSavableModel {
		[field: ES3Serializable]
		public BindableProperty<float> Electricity { get; private set; } = new BindableProperty<float>(0.6f);

		[field: ES3Serializable]
		public BindableProperty<bool> HasElectricityGenerator { get; private set; } = new BindableProperty<bool>(false);
		
		public bool HasElectricity() {
			return Electricity.Value > 0 || !HasElectricityGenerator;
		}
	}
}