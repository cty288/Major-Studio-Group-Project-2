using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.GameEvents.Merchant;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CultistLetterUIPanel : OpenableUIPanel {
	private List<Image> images;
	private List<TMP_Text> texts;
	private GameObject panel;
	private Dictionary<Image, float> imageAlpha = new Dictionary<Image, float>();
	private TMP_Text content;
	
	protected override void Awake() {
		base.Awake();
		images = GetComponentsInChildren<Image>(true).ToList();
		texts = GetComponentsInChildren<TMP_Text>(true).ToList();
		panel = transform.Find("Panel").gameObject;
		foreach (Image image in images) {
			imageAlpha.Add(image, image.color.a);
		}

		content = panel.transform.Find("Paper/Content").GetComponent<TMP_Text>();
		this.RegisterEvent<OnCultistLetterUIPanelOpened>(OnCultistLetterUIPanelOpened)
			.UnRegisterWhenGameObjectDestroyed(gameObject);
		Hide(0.5f);
	}

	private void OnCultistLetterUIPanelOpened(OnCultistLetterUIPanelOpened e) {
		Show(0.5f);
		string text = "";
		for (int i = 1; i <= e.Contents.Count; i++) {
			text += i + ". " + e.Contents[i - 1];
			if (i != e.Contents.Count) {
				text += "\n\n";
			}
		}
		content.text = text;
	}


	public override void OnShow(float time) {
		panel.SetActive(true);
		images.ForEach((image => image.DOFade( imageAlpha.ContainsKey(image)? imageAlpha[image]: 1 , time)));
		texts.ForEach((text => text.DOFade(1, time)));
	}

	public override void OnHide(float time) {
		images.ForEach((image => image.DOFade(0, time)));
		texts.ForEach((text => text.DOFade(0, time)));
		this.Delay(time, () => {
			if (this) {
				{
					panel.SetActive(false);
				}
			}
		});
	}

	public override void OnDayEnd() {
		Hide(0.5f);
	}
}
