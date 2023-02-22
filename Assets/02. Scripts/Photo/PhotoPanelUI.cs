using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotoPanelUI : OpenableUIPanel {
	
	private List<Image> images;
	private List<TMP_Text> texts;
	private List<RawImage> rawImages;
	private PhotoSaveModel photoSaveModel;
	private RawImage photoRawImage;
	private Image bgImage;
	private Transform photoPanel;
	private IHaveBodyInfo bodyInfoContainer;
	
	private Dictionary<Image, float> imageAlpha = new Dictionary<Image, float>();

	protected override void Awake() {
		base.Awake();
		images = GetComponentsInChildren<Image>(true).ToList();
		texts = GetComponentsInChildren<TMP_Text>(true).ToList();
		rawImages = GetComponentsInChildren<RawImage>(true).ToList();
		photoSaveModel = this.GetModel<PhotoSaveModel>();
		bgImage = transform.Find("Panel/Paper").GetComponent<Image>();
		photoRawImage = bgImage.transform.Find("RawImage").GetComponent<RawImage>();
		photoPanel = transform.Find("Panel");
		bodyInfoContainer = GetComponentInChildren<IHaveBodyInfo>(true);
		
		foreach (var image in images) {
			imageAlpha.Add(image, image.color.a);
		}
		
		Hide(0.5f);
		this.RegisterEvent<OnPhotoOpened>(OnPhotoOpened).UnRegisterWhenGameObjectDestroyed(gameObject);
	}

	private void OnPhotoOpened(OnPhotoOpened e) {
		bgImage.sprite = e.BGSprite;
		photoRawImage.texture = photoSaveModel.GetPhotoTexture(photoSaveModel.GetPhoto(e.Content));
		bodyInfoContainer.BodyInfos = e.BodyInfos;
		Show(0.5f);
	}

	public override void OnShow(float time) {
		photoPanel.gameObject.SetActive(true);
		images.ForEach((image => image.DOFade(imageAlpha[image], time)));
		texts.ForEach((text => text.DOFade(1, time)));
		rawImages.ForEach((rawImage => rawImage.DOFade(1, time)));
	}

	public override void OnHide(float time) {
		images.ForEach((image => image.DOFade(0, time)));
		texts.ForEach((text => text.DOFade(0, time)));
		rawImages.ForEach((rawImage => rawImage.DOFade(0, time)));
		this.Delay(time, () => {
			if (this) {
				{
					photoPanel.gameObject.SetActive(false);
				}
			}
		});
	}

	public override void OnDayEnd() {
		Hide(0.5f);
	}
}
