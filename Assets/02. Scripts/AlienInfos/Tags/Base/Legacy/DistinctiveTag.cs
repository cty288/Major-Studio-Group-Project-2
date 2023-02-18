using System.Collections.Generic;

namespace _02._Scripts.AlienInfos.Tags.Base {
	public class DistinctiveTag: IAlienTag {
		public string GetRandomRadioDescription(out bool isReal) {
			isReal = true;
			return "";
		}

		public string GetRandomRadioDescription(bool isReal) {
			return "";
		}

		public List<string> GetShortDescriptions() {
			return null;
		}
	}
}