using System;
using System.Collections.Generic;

namespace _02._Scripts.Notebook {
	public class NotebookModel : AbstractSavableModel {
		[field: ES3Serializable]
		public Dictionary<DateTime, List<DroppableInfo>> Notes { get; private set; } = new Dictionary<DateTime, List<DroppableInfo>>();
		
		[field: ES3Serializable]
		public DateTime LastOpened { get; private set; }
		
		
		
		public void UpdateLastOpened(DateTime time) {
			LastOpened = time.Date;
		}
		
		public void AddNote (DroppableInfo droppableInfo, DateTime time) {
			DateTime date = time.Date;
			
			if (!Notes.ContainsKey(date)) {
				Notes.Add(date, new List<DroppableInfo>());
			}
			Notes[date].Add(droppableInfo);
		}
		
		public List<DroppableInfo> GetNotes (DateTime date) {
			date = date.Date;
			if (Notes.ContainsKey(date)) {
				return Notes[date];
			}
			return new List<DroppableInfo>();
		}
	}
}