using MikroFramework.Architecture;

namespace _02._Scripts.KFC {
	public class KFCSystem: AbstractSystem {
		protected override void OnInit() {
			this.RegisterEvent<OnNewDay>(OnNewDay);
		}

		private void OnNewDay(OnNewDay e) {
			if (e.Day == 1) {
				this.GetSystem<TelephoneSystem>().AddContact("233333", new KFCPhone());
			}
		}
	}
}