using System;
using _02._Scripts.BodyManagmentSystem;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02._Scripts.Poster.PosterContentPanels {
	public class SuspectPosterContentViewController: AbstractMikroController<MainGame> {
		
		private RawImage suspectPhoto;
		private BodyModel bodyModel;
		private GameObject spawnedBody;
		private TMP_Text rewardsText;
		
		[SerializeField] private Material bodyMaterial;

		private void Awake() {
			suspectPhoto = transform.Find("Content/SuspectMask/SuspectPhoto").GetComponent<RawImage>();
			bodyModel = this.GetModel<BodyModel>();
			rewardsText = transform.Find("Content/RewardText").GetComponent<TMP_Text>();
		}

		public void SetContent(long suspectId, GoodsInfo rewards) {
			Awake();
			
			BodyInfo bodyInfo = bodyModel.GetBodyInfoByID(suspectId);
			//BodyInfo newspaperInfo = BodyInfo.GetBodyInfoForDisplay(bodyInfo, BodyPartDisplayType.Newspaper, false);



			spawnedBody = AlienBody.BuildNewspaperAlienBody(bodyInfo, 0, -100, 0, 0.35f,
				Instantiate(bodyMaterial), "SuspectBody");
			spawnedBody.GetComponent<AlienBody>().ShowColor(0);
			Camera camera = spawnedBody.GetComponentInChildren<Camera>();
			RenderTexture renderTexture = camera.targetTexture;
		
			suspectPhoto.GetComponent<RawImage>().texture = renderTexture;
			rewardsText.text = $"{rewards.DisplayName} x{rewards.Count}";

			this.GetModel<TelephoneNumberRecordModel>().AddOrEditRecord("611", "Suspect Report Hotline");
		}
		
		private void OnDestroy() {
			if (spawnedBody) {
				Destroy(spawnedBody);
			}
		}
		
	}
}