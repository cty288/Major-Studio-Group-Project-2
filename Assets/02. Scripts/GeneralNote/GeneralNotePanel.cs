using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralNotePanel : OpenableUIPanel {
	private GameObject panel;
	private List<MaskableGraphic> images;
	private List<TMP_Text> texts;
	private Dictionary<MaskableGraphic, float> imageAlpha = new Dictionary<MaskableGraphic, float>();
	private TMP_Text contentText;
	protected override void Awake() {
		base.Awake();
		images = GetComponentsInChildren<MaskableGraphic>(true).ToList();
		texts = GetComponentsInChildren<TMP_Text>(true).ToList();
		foreach (var image in images) {
			imageAlpha.Add(image, image.color.a);
		}
		panel = transform.Find("Panel").gameObject;
		contentText = panel.transform.Find("Paper/Content").GetComponent<TMP_Text>();
		this.RegisterEvent<OpenGeneralNoteUIPanelEvent>(OnNotePanelOpened);
	}

	private void OnNotePanelOpened(OpenGeneralNoteUIPanelEvent e) {
		Show(0.5f);
		contentText.text = e.Content;
	}

	public override void OnShow(float time) {
		panel.gameObject.SetActive(true);
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
		images.ForEach((image => image.DOFade(0, time)));
		texts.ForEach((text => text.DOFade(0, time)));
		this.Delay(time, () => {
			if (this) { {
					panel.gameObject.SetActive(false);
				}
			}
		});
	}

	public override void OnDayEnd() {
		Hide(0.5f);
	}
}
