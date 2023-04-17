using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.BodyManagmentSystem;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProloguePhotoPanel : OpenableUIPanel {
	private List<Image> images;
	private List<TMP_Text> texts;
	private List<RawImage> rawImages;
	private Dictionary<Image, float> imageAlpha = new Dictionary<Image, float>();
	private Transform photoPanel;
	protected GameObject spawnedPhotoBody;
	protected BodyModel bodyModel;
	protected RawImage rawImage;
	[SerializeField] protected Material bodyMaterial;
	protected override void Awake() {
		base.Awake();
		photoPanel = transform.Find("Panel");
		images = GetComponentsInChildren<Image>(true).ToList();
		texts = GetComponentsInChildren<TMP_Text>(true).ToList();
		rawImages = GetComponentsInChildren<RawImage>(true).ToList();
		bodyModel = this.GetModel<BodyModel>();
		rawImage = photoPanel.Find("Content/Mask/RawImage").GetComponent<RawImage>();
		foreach (var image in images) {
			imageAlpha.Add(image, image.color.a);
		}
		
		Hide(0.5f);
	}

	public override void OnShow(float time) {
		ShowPhoto();
		photoPanel.gameObject.SetActive(true);
		images.ForEach((image => image.DOFade(imageAlpha[image], time)));
		texts.ForEach((text => text.DOFade(1, time)));
		rawImages.ForEach((rawImage => rawImage.DOFade(1, time)));
		
	}

	private void ShowPhoto() {
		BodyInfo bodyInfo = bodyModel.ProloguePlayerBodyInfo;
		//BodyInfo newspaperInfo = BodyInfo.GetBodyInfoForDisplay(bodyInfo, BodyPartDisplayType.Newspaper, false);
		
		spawnedPhotoBody = AlienBody.BuildNewspaperAlienBody(bodyInfo, 0, -100, 0, 0.35f,
			Instantiate(bodyMaterial), "SuspectBody");
		spawnedPhotoBody.GetComponent<AlienBody>().ShowColor(0);
		Camera camera = spawnedPhotoBody.GetComponentInChildren<Camera>();
		RenderTexture renderTexture = camera.targetTexture;
		
		rawImage.texture = renderTexture;
	}

	public override void OnHide(float time) {
		images.ForEach((image => image.DOFade(0, time)));
		texts.ForEach((text => text.DOFade(0, time)));
		rawImages.ForEach((rawImage => rawImage.DOFade(0, time)));
		this.Delay(time, () => {
			if (this) { {
					photoPanel.gameObject.SetActive(false);
					if (spawnedPhotoBody) {
						Destroy(spawnedPhotoBody);
					}
				}
			}
			
		});
	}

	public override void OnDayEnd() {
		Hide(0.5f);
	}
}
