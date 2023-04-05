using System;
using System.Collections.Generic;
using System.Linq;
using MikroFramework.Architecture;

namespace _02._Scripts.Notebook {

	public struct OnNoteDeleted {
		public bool SpawnTrash;
	}
	public class NotebookModel : AbstractSavableModel {
		[field: ES3Serializable]
		public Dictionary<DateTime, List<DroppableInfo>> Notes { get; private set; } = new Dictionary<DateTime, List<DroppableInfo>>();
		
		[field: ES3Serializable]
		public DateTime LastOpened { get; private set; }

		[field: ES3Serializable] public bool HasNotebook { get;  set; } = false;
		
		
		public void UpdateLastOpened(DateTime time, bool alsoInit = false) {
			LastOpened = time.Date;
			if (alsoInit) {
				if (!Notes.ContainsKey(LastOpened)) {
					Notes.Add(LastOpened, new List<DroppableInfo>());
				}
			}
		}
		
		public void RemoveNotes(DateTime date, bool spawnTrash) {
			if (Notes.ContainsKey(date)) {
				Notes.Remove(date);
				this.SendEvent<OnNoteDeleted>(new OnNoteDeleted() {
					SpawnTrash = spawnTrash
				});
			}
		}
		
		public void RemoveNote(DateTime date, DroppableInfo note) {
			if (Notes.ContainsKey(date)) {
				Notes[date].Remove(note);
			}
		}
		
		public bool HasNotes(DateTime date) {
			date = date.Date;
			return Notes.ContainsKey(date.Date);
		}
		
		
		public void RemoveNotes(DroppableInfo droppableInfo) {
			foreach (var notes in Notes.Values) {
				notes.RemoveAll(x => x == droppableInfo);
			}
		}

		public bool HasPreviousNotes(DateTime currentDate, out DateTime previousDate) {
			currentDate = currentDate.Date;
			previousDate = currentDate;
			//enumerate all dates before the current date
			foreach (var date in Notes.Keys.Where(date => date < currentDate).OrderByDescending(date => date)) {
				//if there are notes for this date, return them
				if (Notes[date] != null) {
					previousDate = date;
					return true;
				}
			}
			return false;
		}
		
		public bool HasNextNotes(DateTime currentDate,out DateTime nextDate) {
			currentDate = currentDate.Date;
			nextDate = currentDate;
			//enumerate all dates after the current date
			foreach (var date in Notes.Keys.Where(date => date > currentDate).OrderBy(date => date)) {
				//if there are notes for this date, return them
				if (Notes[date] != null) {
					nextDate = date;
					return true;
				}
			}
			return false;
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