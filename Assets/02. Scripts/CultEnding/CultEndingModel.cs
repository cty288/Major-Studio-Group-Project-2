using System.Collections.Generic;
using Crosstales;
using UnityEngine;

namespace _02._Scripts.CultEnding {
	public class CultEndingModel: AbstractSavableModel {
		[field: ES3Serializable]
		protected List<string> notAddedCultLetterContents = new List<string>() {
			"Xenovore has a longer lifespan and does not follow the 3-day survival pattern of others.",
			"All Xenovore sightings reported by our faithful members have noted that it appears to wear the same set of outfit.",
			"Xenovore's language is beyond human comprehension, as reported by those who have been fortunate enough to hear it speak.",
			"Xenovore is incredibly strong, so you must open your door if you hear an incredibly loud and heavy knocking sound",
			"Xenovore has been sighted frequently after 11 PM, a time when it is most active and alert."
		};

		public List<string> NotAddedCultLetterContents => notAddedCultLetterContents;
		

		protected override void OnInit() {
			base.OnInit();
		}

		public void InitModel() {
			//addedCultLetterContents.Clear();
			notAddedCultLetterContents.CTShuffle();
			notAddedCultLetterContents.Insert(0,
				"Xenovore is 3 times stronger than regular ones, and can't be killed with just one bullet like those weaklings.");
		}

		public List<string> PopContents(int count) {
			count = Mathf.Min(count, notAddedCultLetterContents.Count);
			List<string> contents = new List<string>();
			for (int i = 0; i < count; i++) {
				string content = notAddedCultLetterContents[0];
				notAddedCultLetterContents.RemoveAt(0);
				contents.Add(content);
			}

			return contents;
		}
	}
}