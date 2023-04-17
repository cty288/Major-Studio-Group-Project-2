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

public class MainGamePhotoPanel : OpenableUIPanel
{
   	private List<Image> images;
	private List<TMP_Text> texts;
	private List<RawImage> rawImages;
	private Dictionary<Image, float> imageAlpha = new Dictionary<Image, float>();
	private Transform photoPanel;
	
	protected override void Awake() {
		base.Awake();
		photoPanel = transform.Find("Panel");
		images = GetComponentsInChildren<Image>(true).ToList();
		texts = GetComponentsInChildren<TMP_Text>(true).ToList();
		rawImages = GetComponentsInChildren<RawImage>(true).ToList();
		foreach (var image in images) {
			imageAlpha.Add(image, image.color.a);
		}
		
		Hide(0.5f);
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
			if (this) { {
					photoPanel.gameObject.SetActive(false);
				}
			}
			
		});
	}

	public override void OnDayEnd() {
		Hide(0.5f);
	}
}
