using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.Poster;
using _02._Scripts.Poster.PosterEvents;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PosterPanelViewController : OpenableUIPanel {
	private List<MaskableGraphic> images;
	private List<TMP_Text> texts;
	private Dictionary<MaskableGraphic, float> imageAlpha = new Dictionary<MaskableGraphic, float>();
	private GameObject panel;
	private Transform contentPanel;
	private PosterModel model;
	protected override void Awake() {
		base.Awake();
		images = GetComponentsInChildren<MaskableGraphic>(true).ToList();
		texts = GetComponentsInChildren<TMP_Text>(true).ToList();
		panel = transform.Find("Panel").gameObject;
		foreach (var image in images) {
			imageAlpha.Add(image, image.color.a);
		}
		model = this.GetModel<PosterModel>();
		Hide(0.5f);
		contentPanel = transform.Find("Panel/ContentPanel");
		this.RegisterEvent<OnPosterUIPanelOpened>(OnPosterUIPanelOpened)
			.UnRegisterWhenGameObjectDestroyed(gameObject);
		
	}

	private void OnPosterUIPanelOpened(OnPosterUIPanelOpened e) {
		ShowPage(model.GetPoster(e.id));
		Show(0.5f);
	}

	private void ShowPage(Poster p) {
		GameObject page = PosterAssets.Singleton.GetContentPage(p, contentPanel);
	}
	
	private void DestroyContent() {
		foreach (Transform spawnedBody in contentPanel) {
			Destroy(spawnedBody.gameObject);
		}
	}

	public override void OnShow(float time) {panel.gameObject.SetActive(true);
		images = GetComponentsInChildren<MaskableGraphic>(true).ToList();
		texts = GetComponentsInChildren<TMP_Text>(true).ToList();
		AudioSystem.Singleton.Play2DSound("pick_up_newspaper");
		images.ForEach((image => {
			if(imageAlpha.ContainsKey(image)) {
				image.DOFade(imageAlpha[image], time);
			}
			else {
				image.DOFade(1, time);
			}
			
		}));
		texts.ForEach((text => text.DOFade(1, time)));
	}

	public override void OnHide(float time) {
		images = GetComponentsInChildren<MaskableGraphic>(true).ToList();
		texts = GetComponentsInChildren<TMP_Text>(true).ToList();
		
		images.ForEach((image => image.DOFade(0, time)));
		texts.ForEach((text => text.DOFade(0, time)));
		
		
		this.Delay(time, () => {
			if (this) { {
					DestroyContent();
					panel.gameObject.SetActive(false);
				}
			}
		});
	}

	public override void OnDayEnd() {
		Hide(0.5f);
	}
}
