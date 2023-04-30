using System;
using System.Collections;
using System.IO;
using MikroFramework.Singletons;
using Polyglot;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace _02._Scripts {
	public class HotUpdateLoader {
		public static IEnumerator LoadOrDownload(string docID, string sheetID, string localBackupName, Action<string> onDone) {
			
			
			TextAsset asset = Resources.Load<TextAsset>(localBackupName);
			
			//check if we have access to the internet and the url
			if (Application.internetReachability == NetworkReachability.NotReachable && asset) {
				Debug.Log("No internet connection, loading local backup");
				onDone(asset.text);
				yield break;
			}

			string result = "";
			var enumerator = GoogleDownload.DownloadSheet(docID,
				sheetID, s => {
					result = s;
				}, GoogleDriveDownloadFormat.CSV);
			while (enumerator.MoveNext()) {
				
			}
			
			if (string.IsNullOrEmpty(result) && asset) {
				Debug.Log("No internet connection, loading local backup");
				onDone(asset.text);
				yield break;
			}

			Debug.Log("Sheet loaded from Google Successfully! Sheet ID: " + sheetID);
			onDone(result);
			//ping the url to see if it exists
			if (Application.isEditor) {
				//save the file to Resources
				
				File.WriteAllText(Application.dataPath + "/Resources/" + localBackupName + ".csv", result);
				Debug.Log("Saved to Resources");
			}
			
		}
	}
}