using System.Collections.Generic;
using System.Linq;
using MikroFramework.Architecture;
using NHibernate.Mapping;
using Polyglot;
using UnityEngine;

namespace _02._Scripts.BodyManagmentSystem {

	public class TagDescription {
		public List<string> RealRadioDescription = new List<string>();
		public List<string> FakeRadioDescription = new List<string>();
		public List<string> ShortDescription = new List<string>();
	}
	
	public class BodyTagInfoModel: AbstractModel {
		protected Dictionary<string, TagDescription> tagDescriptions = new Dictionary<string, TagDescription>();

		protected override void OnInit() {
			
		}
		
		public List<string> GetRealRadioDescription(string tag) {
			if (tagDescriptions.ContainsKey(tag)) {
				return tagDescriptions[tag].RealRadioDescription;
			}
			else {
				return null;
			}
		}
		
		public List<string> GetFakeRadioDescription(string tag) {
			if (tagDescriptions.ContainsKey(tag)) {
				return tagDescriptions[tag].FakeRadioDescription;
			}
			else {
				var result=  tagDescriptions.Values.Where((description =>
					description.RealRadioDescription != null && description.RealRadioDescription.Count > 0)).ToList();
				
				List<string> fakeDescriptions = new List<string>();
				foreach (var tagDescription in result) {
					fakeDescriptions.AddRange(tagDescription.FakeRadioDescription);
				}
				return fakeDescriptions;
			}
		}
		
		public List<string> GetShortDescriptions(string tag) {
			if (tagDescriptions.ContainsKey(tag)) {
				return tagDescriptions[tag].ShortDescription;
			}
			else {
				return null;
			}
		}


		public void LoadInfo() {
			string result = "";
			var enumerator = GoogleDownload.DownloadSheet("1lmnHrIwzQdimzbfLRgKDrLi43lfnCQKSiukUL9kffQo",
				"0", s => {
					result = s;
				}, GoogleDriveDownloadFormat.CSV);
			while (enumerator.MoveNext()) {
				
			}
			Debug.Log("result: " + result);
			OnDownloadSheet(result);
		}
		
		
		public  void OnDownloadSheet(string text) {
            if (!string.IsNullOrEmpty(text)) {
                List<List<string>> rows;
                text = text.Replace("\r\n", "\n");
                rows = CsvReader.Parse(text);

              
                int startRow = 1;
                string currentTag = rows[startRow][0];
                TagDescription currentTagDescription = new TagDescription();
                tagDescriptions.Add(currentTag, currentTagDescription);
                
                for (int i = startRow; i < rows.Count; i++) {
					List<string> row = rows[i];
					if (row.Count > 0) {
						if (!string.IsNullOrEmpty(row[0]) && row[0] != currentTag) {
							currentTag = row[0];
							currentTagDescription = new TagDescription();
							tagDescriptions.Add(currentTag, currentTagDescription);
						}
						if (!string.IsNullOrEmpty(row[1])) {
							currentTagDescription.RealRadioDescription.Add(row[1]);
						}
						if (!string.IsNullOrEmpty(row[2])) {
							currentTagDescription.FakeRadioDescription.Add(row[2]);
						}
						if (!string.IsNullOrEmpty(row[3])) {
							currentTagDescription.ShortDescription.Add(row[3]);
						}
					}
				}

            }
            
        }
	}
}