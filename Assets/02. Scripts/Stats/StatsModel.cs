using System.Collections.Generic;
using MikroFramework.Architecture;

namespace _02._Scripts.Stats {

	public class SaveData {
		public string DisplayName;
		public object Value;
		
		public SaveData(string displayName, object value) {
			DisplayName = displayName;
			Value = value;
		}
	}
	public class StatsModel : AbstractSavableModel {
		[ES3Serializable]
		protected Dictionary<string, SaveData> stats = new Dictionary<string, SaveData>();
		
		protected override void OnInit() {
			InitStats("DaySurvived", -1, "Days Survived");
			InitStats("TotalDoorKnock", 0, "Total Visitors");
			InitStats("TotalDoorOpen", 0, "Door Opened");
			InitStats("TotalMonsterKnock", 0, "Monsters Knocked on Your Door");
			InitStats("GunKilledMonster", 0, "Bullets Shot");
			//InitStats("MonstersReported", 0);
			//InitStats("GoodPeopleReported", 0);
			InitStats("TotalBulletsGet", 0, "Bullets Obtained");
			InitStats("RubbishThrown", 0, "Total Rubbish Thrown");
			InitStats("ItemsAddedToTable", 0, "Total Items Added to the Table");
			InitStats("NotebookContentAdded", 0, "Total Contents Added to the Notebook");
			InitStats("PhotoTaken", 0, "Photos Taken");
			InitStats("TotalPosterGet", 0, "Posters Collected");
			InitStats("IncomingCallReceived", 0, "Received Incoming Calls");
			InitStats("TelephoneDealt", 0, "Phone Call Dealt");
		}
		
		protected void InitStats(string name, object defaultValue, string displayName) {
			if (!stats.ContainsKey(name)) {
				stats.Add(name, new SaveData(displayName, defaultValue));
			}
		}
		
		public void UpdateStat(string name, SaveData value) {
			if (stats.ContainsKey(name)) {
				stats[name] = value;
			}
			else {
				stats.Add(name, value);
			}
		}
		
		public object GetStat(string name, object defaultValue){
			if (stats.ContainsKey(name)) {
				return stats[name].Value;
			}
			else {
				stats.Add(name, new SaveData(name, defaultValue));
				return defaultValue;
			}
		}
		
		public List<SaveData> GetStats() {
			return new List<SaveData>(stats.Values);
		}
	}
}