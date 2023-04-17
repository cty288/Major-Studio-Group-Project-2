using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _02._Scripts.Notebook {
	[ES3Serializable]
	[Serializable]
	public class DroppableNewspaperBodyInfo: DroppableInfo {
		[field: ES3Serializable]
		public override bool IsDefaultOnTop { get; } = true;
		
		[field: ES3Serializable]
		private BodyInfo BodyInfo { get; set; }

		[ES3Serializable]
		private GameObject contentUIPrefab;
		
		public DroppableNewspaperBodyInfo():base(){}
		
		public DroppableNewspaperBodyInfo(BodyInfo bodyInfo, GameObject contentUIPrefab):base(){
			BodyInfo = bodyInfo;
			this.contentUIPrefab = contentUIPrefab;
		}
		public override DroppedUIObjectViewController GetContentUIObject(RectTransform parent) {
			NotebookDroppedNewsPhoto droppedPhoto =
				GameObject.Instantiate(contentUIPrefab).GetComponent<NotebookDroppedNewsPhoto>() as
					NotebookDroppedNewsPhoto;
			droppedPhoto.SetContent(BodyInfo);
			droppedPhoto.transform.SetParent(parent);
			//droppedText.GetComponent<RectTransform>().localPosition = Vector3.zero;
		
			return droppedPhoto;
		}
	}
}