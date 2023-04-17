using UnityEngine;

namespace _02._Scripts.Notebook {


	[ES3Serializable]
	public abstract class DroppableInfo {
		[field: ES3Serializable]
		public Bounds Bounds { get; set; }
		
		
		[field: ES3Serializable]
		public abstract bool IsDefaultOnTop { get; } 
		public abstract DroppedUIObjectViewController GetContentUIObject(RectTransform parent);
	}
	
	
	public interface IDroppable {
		public DroppableInfo GetDroppableInfo();
		
		public void OnDropped();
	}
}