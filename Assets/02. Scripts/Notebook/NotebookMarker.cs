using System;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace _02._Scripts.Notebook {
	
	public class NotebookMarkerDroppableInfo : DroppableInfo {
		[field: ES3Serializable]
		public override bool IsDefaultOnTop { get; } = false;
		[field: ES3Serializable]

		public List<Vector3> markerPositions = new List<Vector3>();
		
		[ES3Serializable]
		private Bounds markerBounds;

		public Bounds MarkerBounds => markerBounds;

		[ES3Serializable]
		private GameObject contentUIPrefab;
		
		[ES3Serializable]
		private bool canDraw = true;

		public bool CanDraw => canDraw;


		public NotebookMarkerDroppableInfo(GameObject contentUIPrefab, Bounds bounds) {
			this.contentUIPrefab = contentUIPrefab;
			this.markerBounds = bounds;
		}
		
		public void StopDrawing() {
			canDraw = false;
		}
		
		public NotebookMarkerDroppableInfo(){}
		public override DroppedUIObjectViewController GetContentUIObject(RectTransform parent) {
			NotebookMarker droppedText =
				GameObject.Instantiate(contentUIPrefab).GetComponent<NotebookMarker>() as
					NotebookMarker;

			droppedText.SetInitialContent(markerPositions);
			droppedText.transform.SetParent(parent);
			
			
		
			return droppedText;
		}
	}
	public class NotebookMarker : DroppedUIObjectViewController {
		private ScreenSpaceCameraMarker _screenSpaceCameraMarker;
		private PlayerControlModel playerControlModel;
		protected LineRenderer currentLineRenderer = null;
		
		protected Vector2 lastMarkerPosition = Vector2.zero;
		
		
		protected override void Awake() {
			base.Awake();
			playerControlModel = this.GetModel<PlayerControlModel>();
			_screenSpaceCameraMarker = GetComponent<ScreenSpaceCameraMarker>();
		}

		public override Vector2 GetExtent() {
			return Vector2.zero;
		}

		private void Update() {
			if (DroppableInfo == null) {
				return;
			}
			
			NotebookMarkerDroppableInfo info = (NotebookMarkerDroppableInfo)DroppableInfo;
			if (info.CanDraw) {
				if (playerControlModel.ControlType.Value != PlayerControlType.Normal) {
					return;
				}
				
				
				if (Input.GetMouseButton(0)) {
					Vector2 mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                
					//Debug.Log("MousePosWorld: " + mousePosWorld + " MarkerArea: " + markerArea.bounds + "MousePos: " + Input.mousePosition);
					if(Vector2.Distance(lastMarkerPosition, Input.mousePosition) > 0.01) {
						//check if the mouse is in the marker area. The canvas is screen space camera
						Vector3 mousePosWorldFixed = new Vector3(mousePosWorld.x, mousePosWorld.y,
							info.MarkerBounds.center.z);

						if (info.MarkerBounds.Contains(mousePosWorldFixed)) {
							Vector3 mousePos = new Vector3(mousePosWorld.x, mousePosWorld.y, -6);
							_screenSpaceCameraMarker.AddMarkerPosition(currentLineRenderer, mousePos);
                        
						}
                    
						lastMarkerPosition = Input.mousePosition;
                    
					}
				}
            
				
			}
		}

		public void StopDraw() {
			if (_screenSpaceCameraMarker == null) {
				return;
			}
			NotebookMarkerDroppableInfo info = (NotebookMarkerDroppableInfo)DroppableInfo;
			List<Vector3> positions = _screenSpaceCameraMarker.GetCurrentMarkerPositions();
			if (positions!=null && positions.Count > 0) {
				info.markerPositions = positions;
			}
			info.StopDrawing();
		}

		public void SetInitialContent(List<Vector3> markerPositions) {
			if (markerPositions!=null && markerPositions.Count > 0) {
				LineRenderer renderer = _screenSpaceCameraMarker.AddMarker();
				renderer.positionCount = markerPositions.Count;
				renderer.SetPositions(markerPositions.ToArray());
			}
			else {
				currentLineRenderer = _screenSpaceCameraMarker.AddMarker();
			}
		}
		
		
		protected override void OnClick() {
			
		}
	}
}