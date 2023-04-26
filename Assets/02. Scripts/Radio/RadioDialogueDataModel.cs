using System;
using System.Collections.Generic;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using Polyglot;
using UnityEngine;

namespace _02._Scripts.Radio {
	public class RadioDialogueDataModel: AbstractModel { 
		
		private Dictionary<string, RadioDialogueContent> hotUpdateInfo =
		new Dictionary<string, RadioDialogueContent>();
	
	

		protected override void OnInit() {
			LoadInfo();
		}

		public RadioDialogueContent GetInfo(string key) {
			key = key.ToLower();
			if (hotUpdateInfo.ContainsKey(key)) {
				return hotUpdateInfo[key];
			}
			return null;
		}

		public Dictionary<string, RadioDialogueContent> HotUpdateInfo => hotUpdateInfo;


		public void LoadInfo() {
			string result = "";
			var enumerator = GoogleDownload.DownloadSheet("1lmnHrIwzQdimzbfLRgKDrLi43lfnCQKSiukUL9kffQo",
				"1039588817", s => {
					result = s;
				}, GoogleDriveDownloadFormat.CSV);
			while (enumerator.MoveNext()) {
					
			}
			//Debug.Log("result: " + result);
			OnDownloadSheet(result);
		}
		public  void OnDownloadSheet(string text) {
			if (!string.IsNullOrEmpty(text)) {
				List<List<string>> rows;
				text = text.Replace("\r\n", "\n");
				rows = CsvReader.Parse(text);

	              
				int startRow = 1;
				string currentTag = "";
				int propertyCount = 5;
				
				for (int i = startRow; i < rows.Count; i+=propertyCount) {
					if (string.IsNullOrEmpty(rows[i][0]) || string.IsNullOrEmpty(rows[i+1][0])) {
						continue;
					}

					string keyName = rows[i][0];
					if (keyName.ToLower() == currentTag) {
						continue;
					}

					RadioDialogueContent info = new RadioDialogueContent();
					info.Name = keyName;
					int dialogueCount = int.Parse(rows[i + 1][0]);
					int dialogueContentStartColumn = 2;
					int dialogueContentEndColumn = dialogueContentStartColumn + dialogueCount - 1;
					
					bool hasError = false;
					for (int j = dialogueContentStartColumn; j <= dialogueContentEndColumn; j++) {
						try {
							string contentText = rows[i][j];
							int genderIndex = int.Parse(rows[i + 1][j]);
							Gender gender = (Gender) genderIndex;
							float speakSpeed = float.Parse(rows[i + 2][j]);
							int mixerIndex = int.Parse(rows[i + 3][j]);
							string speakerName = rows[i + 4][j];
							info.TextContents.Add(new RadioTextContent(contentText, speakSpeed, gender,
								AudioMixerList.Singleton.AudioMixerGroups[mixerIndex],
								speakerName));
						}
						catch (Exception e) {
							hasError = true;
							Debug.LogError("Error in parsing radio dialogue: " + keyName + " " + e.StackTrace);
							break;
						}
					}
					
					if (hasError || info.TextContents.Count == 0) {
						continue;
					}

					hotUpdateInfo.Add(keyName.ToLower(), info);
				}

			}
	            
		}
	}
}