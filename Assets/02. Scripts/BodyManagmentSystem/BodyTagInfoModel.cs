using System;
using System.Collections.Generic;
using System.Linq;
using MikroFramework.Architecture;
using NHibernate.Mapping;
using Polyglot;
using UnityEngine;

namespace _02._Scripts.BodyManagmentSystem {

	public class TagDescription {
		public string TagName;
		public List<string> RealRadioDescription = new List<string>();
		public List<string> FakeRadioDescription = new List<string>();
		public List<string> ShortDescription = new List<string>();
		public Type TagType = null;
		public bool IsUpperBody = false;
	}
	
	public class BodyTagInfoModel: AbstractModel {
		protected Dictionary<string, TagDescription> tagDescriptions = new Dictionary<string, TagDescription>();
		protected DescriptionFormatter formatter = new DescriptionFormatter();
		protected override void OnInit() {
			LoadInfo();
		}
		
		public List<string> GetRealRadioDescription(string tag, string monsterName) {
			tag = tag.ToLower();
			if (tagDescriptions.ContainsKey(tag)) {
				List<string> formattedDescriptions = new List<string>();
				foreach (string description in tagDescriptions[tag].RealRadioDescription) {
					formattedDescriptions.Add(String.Format(formatter, description, "{0}", monsterName));
				}
				return formattedDescriptions;
			}
			else {
				return null;
			}
		}
		
		public List<string> GetFakeRadioDescription(string tag, Predicate<TagDescription> predicate, string monsterName) {
			tag = tag.ToLower();
			if (tagDescriptions.ContainsKey(tag)) {
				List<string> targetList = tagDescriptions[tag].FakeRadioDescription;
				if (targetList.Count > 0) {
					List<string> formattedDescriptions = new List<string>();
					
					foreach (string description in targetList) {
						formattedDescriptions.Add(String.Format(formatter, description, "{0}", monsterName));
					}
					return formattedDescriptions;
				}
			}

			
			List<TagDescription> result = new List<TagDescription>();  //tagDescriptions.Values.Where((description =>
				//description.RealRadioDescription != null && description.RealRadioDescription.Count > 0)).ToList();

			foreach (string descriptionsKey in tagDescriptions.Keys) {
				if(descriptionsKey == tag) continue;
				TagDescription description = tagDescriptions[descriptionsKey];
				if (description.RealRadioDescription != null && description.RealRadioDescription.Count > 0 && description.TagType!=null && predicate(description)) {
					result.Add(description);
				}
			}

			List<string> fakeDescriptions = new List<string>();
			if (result.Count>0) {
				foreach (var tagDescription in result) {
					fakeDescriptions.AddRange(tagDescription.RealRadioDescription);
				}
			}
			
			List<string> results = new List<string>();
					
			foreach (string description in fakeDescriptions) {
				results.Add(String.Format(formatter, description, null, monsterName));
			}
			return results;

		}
		
		public List<string> GetShortDescriptions(string tag) {
			tag = tag.ToLower();
			if (tagDescriptions.ContainsKey(tag)) {
				return tagDescriptions[tag].ShortDescription;
			}
			else {
				return null;
			}
		}


		public void LoadInfo() {
			string result = "";
			var enumerator = HotUpdateLoader.LoadOrDownload("1lmnHrIwzQdimzbfLRgKDrLi43lfnCQKSiukUL9kffQo",
				"0", "body_tag.csv", s => {
					result = s; 
				});
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
                string currentTag = rows[startRow][0].ToLower();
                TagDescription currentTagDescription = new TagDescription();
                tagDescriptions.Add(currentTag, currentTagDescription);
                
                for (int i = startRow; i < rows.Count; i++) {
					List<string> row = rows[i];
					if (row.Count > 0) {
						if (!string.IsNullOrEmpty(row[0].ToLower()) && row[0].ToLower() != currentTag) {
							currentTag = row[0].ToLower();
							currentTagDescription = new TagDescription();
							currentTagDescription.TagName = currentTag;
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
            
            
            //use reflection to create instances of all the classes that inherit from IAlienTag
            var types = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(p => typeof(IAlienTag).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);
			foreach (var type in types) {
	            IAlienTag tag = (IAlienTag) Activator.CreateInstance(type);
	            if (tagDescriptions.ContainsKey(tag.TagName.ToLower())) {
		            tagDescriptions[tag.TagName.ToLower()].TagType = type;
		            
		            if(tag is IClothTag clothTag) {
			            if (clothTag.IsUpperCloth) {
				            tagDescriptions[tag.TagName.ToLower()].IsUpperBody = true;
			            }
		            }
	            }
	            
	            
			}
            
        }
	}
}