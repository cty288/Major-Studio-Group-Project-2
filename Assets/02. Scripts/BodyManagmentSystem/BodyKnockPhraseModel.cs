using System.Collections.Generic;
using MikroFramework.Architecture;
using Polyglot;

namespace _02._Scripts.BodyManagmentSystem {
	public class BodyKnockPhraseModel: AbstractModel {
		protected Dictionary<string, List<string>> typeDescriptions = new Dictionary<string, List<string>>();

		protected override void OnInit() {
			LoadInfo();
		}
		
		
		
		public List<string> GetPhrases(string knockType) {
			if (typeDescriptions.ContainsKey(knockType)) {
				return typeDescriptions[knockType];
			}
			else {
				return null;
			}
		}


		public void LoadInfo() {
			string result = "";
			var enumerator = GoogleDownload.DownloadSheet("1lmnHrIwzQdimzbfLRgKDrLi43lfnCQKSiukUL9kffQo",
				"1969147239", s => {
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
                string currentTag = rows[startRow][0];
                List<string> currentPhrases = new List<string>();
                typeDescriptions.Add(currentTag, currentPhrases);
                
                

                for (int i = startRow; i < rows.Count; i++) {
					List<string> row = rows[i];
					if (row.Count > 0) {
						if (!string.IsNullOrEmpty(row[0]) && row[0] != currentTag) {
							currentTag = row[0];
							currentPhrases = new List<string>();
							typeDescriptions.Add(currentTag, currentPhrases);
						}
						if (!string.IsNullOrEmpty(row[1])) {
							currentPhrases.Add(row[1]);
						}
					}
				}

            }
            
        }
	}
}