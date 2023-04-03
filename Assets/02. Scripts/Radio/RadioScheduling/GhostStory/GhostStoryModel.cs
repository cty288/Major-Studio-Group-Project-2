using System.Collections.Generic;
using Crosstales;
using MikroFramework.Architecture;

namespace _02._Scripts.Radio.RadioScheduling.GhostStory {

	public struct OnGhostStoryDataInit {
		
	}
	public class GhostStoryModel: AbstractSavableModel {
		[field: ES3Serializable]
		protected List<RadioDialogueContent> allContents { get; set; } = new List<RadioDialogueContent>();

		[field: ES3Serializable] 
		protected List<int> randomIndexList { get; set; } = new List<int>();

		protected override void OnInit() {
			base.OnInit();
			if (allContents.Count == 0) {
				this.SendEvent<OnGhostStoryDataInit>();
			}
		}
		
		public void AddContent(RadioDialogueContent content) {
			allContents.Add(content);
		}
		
		public void AddContent(List<RadioDialogueContent> contents) {
			allContents.AddRange(contents);
		}

		public RadioDialogueContent GetRandomContent() {
			if (allContents.Count == 0) {
				this.SendEvent<OnGhostStoryDataInit>();
			}

			if (randomIndexList.Count == 0) {
				randomIndexList = new List<int>(allContents.Count);
				for (int i = 0; i < allContents.Count; i++) {
					randomIndexList.Add(i);
				}
				randomIndexList.CTShuffle();
			}

			
			int index = randomIndexList[0];
			randomIndexList.RemoveAt(0);
			return allContents[index];
		}
	}
}