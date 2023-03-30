using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Dog;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImportantNewspaperMissingDogContentUI : ImportantNewspaperPageContentUIPanel {
	private TMP_Text phoneNumberText;
	private GameObject spawnedBody;
	private void Awake() {
		phoneNumberText = transform.Find("PhoneNumber").GetComponent<TMP_Text>();
	}

	public override void OnSetContent(IImportantNewspaperPageContent content, int weekCount) {
		Awake();
		MissingDogImportantNewsContent missingDogContent = content as MissingDogImportantNewsContent;
		phoneNumberText.text = missingDogContent.PhoneNumber;

		BuildDogPhoto(missingDogContent.MissingDogBodyInfo);
	}

	private void BuildDogPhoto(BodyInfo missingDogBodyInfo) {
		BodyInfo newspaperInfo = BodyInfo.GetBodyInfoForDisplay(missingDogBodyInfo, BodyPartDisplayType.Newspaper, true);
		Transform imageTr = transform.Find("PhotoImg");
		spawnedBody = AlienBody.BuildNewspaperAlienBody(newspaperInfo, 0, 2000, 0, 0.5f);
		Camera camera = spawnedBody.GetComponentInChildren<Camera>();
		RenderTexture renderTexture = camera.targetTexture;
		
		imageTr.GetComponent<RawImage>().texture = renderTexture;
	}

	private void OnDestroy() {
		if (spawnedBody) {
			Destroy(spawnedBody);
		}
	}
}
