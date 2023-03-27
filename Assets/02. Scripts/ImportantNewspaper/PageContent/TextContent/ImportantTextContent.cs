using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.ImportantNewspaper;
using _02._Scripts.Radio;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImportantTextContent : AbstractMikroController<MainGame> {
	private Image imageContainer;
	private TMP_Text titleText;
	private TMP_Text subtitleText;
	private TMP_Text contentText;
	private void Awake() {
		imageContainer = transform.Find("ImageContainer")?.GetComponent<Image>();
		titleText = transform.Find("Title").GetComponent<TMP_Text>();
		subtitleText = transform.Find("Subtitle").GetComponent<TMP_Text>();
		contentText = transform.Find("Content").GetComponent<TMP_Text>();
	}

	public void SetContent(ImportantNewsTextInfo info) {
		Awake();
		titleText.text = info.Title;
		subtitleText.text = info.SubTitle;
		contentText.text = info.Content;
		if (info.ImageIndex >= 0 && imageContainer) {
			imageContainer.sprite = ImportantNewspaperPageFactory.Singleton.GetImageSprite(info.ImageIndex);
		}
	}
}
