using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.UI;

namespace _02._Scripts.Notebook {
	public class NotebookDroppedNewsPhoto: DroppedUIObjectViewController, IHaveBodyInfo {
		protected RawImage photoImage;
		//protected PlayerControlModel playerControlModel;
		protected BodyInfo bodyInfo;
		protected GameObject spawnedBody;
		protected override void Awake() {
			base.Awake();
			photoImage = GetComponentInChildren<RawImage>(true);
			//playerControlModel = this.GetModel<PlayerControlModel>();
		}

		public override Vector2 GetExtent() {
			Vector3[] corners = new Vector3[4];
			RectTransform rect = GetComponent<RectTransform>();
			rect.GetWorldCorners(corners);
			Vector3 pos = corners[0];
			Vector2 size = new Vector2(
				rect.lossyScale.x * rect.rect.size.x,
				rect.lossyScale.y * rect.rect.size.y);

			return new Rect(pos, size).size * 0.5f;
		}

		protected override void OnClick() {
			
		}
		
		public void SetContent(BodyInfo bodyInfo) {
			Awake();
			BodyInfos.Clear();
			BodyInfos.Add(bodyInfo);
			
			spawnedBody = AlienBody.BuildNewspaperAlienBody(bodyInfo, 0, Random.Range(-200,200), 0);
			Camera camera = spawnedBody.GetComponentInChildren<Camera>();
			RenderTexture renderTexture = camera.targetTexture;
			this.bodyInfo = bodyInfo;
            
			photoImage.texture = renderTexture;
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			if (spawnedBody) {
				Destroy(spawnedBody);
			}
		}

		public List<BodyInfo> BodyInfos { get; set; } = new List<BodyInfo>();
	}
}