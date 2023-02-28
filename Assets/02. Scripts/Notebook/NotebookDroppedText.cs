using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotebookDroppedText : DroppedUIObjectViewController {
	private TMP_Text text;
	private ContentSizeFitter fitter;
	protected override void Awake() {
		base.Awake();
		text = GetComponentInChildren<TMP_Text>(true);
		fitter = GetComponent<ContentSizeFitter>();
	}

	protected override void OnClick() {
		
	}

	public override Vector2 GetExtent() {
		//get the extent of the text
		//get world rect
		Vector3[] corners = new Vector3[4];
		RectTransform rect = GetComponent<RectTransform>();
		rect.GetWorldCorners(corners);
		Vector3 pos = corners[0];
		Vector2 size = new Vector2(
			rect.lossyScale.x * rect.rect.size.x,
			rect.lossyScale.y * rect.rect.size.y);

		return new Rect(pos, size).size * 0.5f;
	}
	
	public void SetContent(string content) {
		text.text = content;
		text.ForceMeshUpdate();
		RectTransform rect = GetComponent<RectTransform>();
		LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
		
		System.Collections.IEnumerator Routine() {
			var csf = fitter;
			csf.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
			yield return null;
			LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
			csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		}
		CoroutineRunner.Singleton.StartCoroutine(Routine());
	}
}
