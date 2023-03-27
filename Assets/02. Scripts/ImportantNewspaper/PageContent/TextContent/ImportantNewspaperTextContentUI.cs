using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ImportantNewspaperTextContentUI : ImportantNewspaperPageContentUIPanel {
	private TMP_Text weekText;
	private ImportantTextContent[] importantTextContents;
	private void Awake() {
		weekText = transform.Find("WeekText").GetComponent<TMP_Text>();
		importantTextContents = GetComponentsInChildren<ImportantTextContent>(true);
	}

	public override void OnSetContent(IImportantNewspaperPageContent content, int weekCount) {
		Awake();
		ImportantNewsTextInfo info = content as ImportantNewsTextInfo;
		bool hasImage = info.ImageIndex >= 0;
		
		weekText.text = $"Week {weekCount}";
		foreach (ImportantTextContent textContent in importantTextContents) {
			textContent.gameObject.SetActive(false);
		}
		if (hasImage) {
			importantTextContents[0].SetContent(info);
			importantTextContents[0].gameObject.SetActive(true);
		}
		else {
			importantTextContents[1].SetContent(info);
			importantTextContents[1].gameObject.SetActive(true);
		}
	}
}
