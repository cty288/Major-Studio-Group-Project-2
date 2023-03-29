using MikroFramework.Architecture;

namespace _02._Scripts.Dog {
	public class DogSystem: AbstractSystem {
		protected override void OnInit() {
			this.RegisterEvent<OnNewDay>(OnNewDay);
		}

		private void OnNewDay(OnNewDay e) {
			if (e.Day == 1) {
				GameEventSystem gameEventSystem = this.GetSystem<GameEventSystem>();
				
			}
		}
	}
}