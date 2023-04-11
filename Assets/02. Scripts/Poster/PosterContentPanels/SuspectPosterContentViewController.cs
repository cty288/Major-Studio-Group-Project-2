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
		private TMP_Text wantedReasonText;
		
		[SerializeField] private Material bodyMaterial;

		private void Awake() {
			suspectPhoto = transform.Find("Content/SuspectMask/SuspectPhoto").GetComponent<RawImage>();
			bodyModel = this.GetModel<BodyModel>();
			rewardsText = transform.Find("Content/RewardText").GetComponent<TMP_Text>();
			wantedReasonText = transform.Find("Content/WantedReason").GetComponent<TMP_Text>();
		}

		public void SetContent(long suspectId, SuspectInfo suspectInfo) {
			Awake();
			
			BodyInfo bodyInfo = bodyModel.GetBodyInfoByID(suspectId);
			//BodyInfo newspaperInfo = BodyInfo.GetBodyInfoForDisplay(bodyInfo, BodyPartDisplayType.Newspaper, false);



			spawnedBody = AlienBody.BuildNewspaperAlienBody(bodyInfo, 0, -100, 0, 0.35f,
				Instantiate(bodyMaterial), "SuspectBody");
			spawnedBody.GetComponent<AlienBody>().ShowColor(0);
			Camera camera = spawnedBody.GetComponentInChildren<Camera>();
			RenderTexture renderTexture = camera.targetTexture;
		
			suspectPhoto.GetComponent<RawImage>().texture = renderTexture;
			rewardsText.text = $"{suspectInfo.rewards.DisplayName} x{suspectInfo.rewards.Count}";
			wantedReasonText.text = suspectInfo.Crime;
			this.GetModel<TelephoneNumberRecordModel>().AddOrEditRecord("611", "Suspect Report Hotline");
		}
		
		private void OnDestroy() {
			if (spawnedBody) {
				Destroy(spawnedBody);
			}
		}
		
	}
}