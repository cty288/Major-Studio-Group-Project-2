using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Notebook;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class NotebookWrittenTextDroppableInfo : DroppableInfo {
	[field: ES3Serializable]
	public override bool IsDefaultLeftPage { get; } = false;

	[ES3Serializable] private string text = "";

	public string Text {
		get => text;
		set => text = value;
	}

	[ES3Serializable]
	private GameObject contentUIPrefab;
	
	public NotebookWrittenTextDroppableInfo(){}

	public NotebookWrittenTextDroppableInfo(GameObject prefab) {
		this.contentUIPrefab = prefab;
	}
	
	
	public override DroppedUIObjectViewController GetContentUIObject(RectTransform parent) {
		NotebookWrittenText droppedText =
			GameObject.Instantiate(contentUIPrefab).GetComponent<DroppedUIObjectViewController>() as
				NotebookWrittenText;
		droppedText.SetContent(text);
		droppedText.transform.SetParent(parent);
		//droppedText.GetComponent<RectTransform>().localPosition = Vector3.zero;
		
		return droppedText;
	}
}

public class NotebookWrittenText : DroppedUIObjectViewController {
	private TMP_InputField inputField;

	public Action<string> OnClickOutside;


	protected override void Awake() {
		base.Awake();
		inputField = GetComponentInChildren<TMP_InputField>(true);
		inputField.onValueChanged.AddListener(OnInputChanged);
	}


	private void OnDestroy() {
		inputField.onValueChanged.RemoveListener(OnInputChanged);
	}

	private void OnInputChanged(string input) {
		if (input == null || DroppableInfo==null) {
			return;
		}
		((NotebookWrittenTextDroppableInfo) DroppableInfo).Text = input;
		inputField.ForceLabelUpdate();
		GetComponent<ContentSizeFitter>().SetLayoutVertical();
		//CoroutineRunner.Singleton.StartCoroutine(Refresh());
		//move the cursor to the end of the text
	//	inputField.MoveTextEnd(false);
	}

	private IEnumerator Refresh() {
		ContentSizeFitter fitter = GetComponent<ContentSizeFitter>();
		fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
		LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
		yield return null;
		inputField.GraphicUpdateComplete();
		fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
		GetComponent<ContentSizeFitter>().SetLayoutVertical();
		inputField.MoveTextEnd(false);
	}

	public void SetContent(string text) {
		inputField.text = text;

		inputField.interactable = String.IsNullOrEmpty(text);
		if (String.IsNullOrEmpty(text)) {
			//select the input field
			inputField.Select();
			
		}
		else {
			GetComponentInChildren<TMP_SelectionCaret>(true).raycastTarget = false;
		}
	}
	
	public void DisableInteractable() {
		inputField.interactable = false;
	}

	public override Vector2 GetExtent() {
		Vector3[] corners = new Vector3[4];
		RectTransform rect = GetComponent<RectTransform>();
		rect.GetWorldCorners(corners);
		Vector3 pos = corners[0];
		Vector2 size = new Vector2(
			rect.lossyScale.x * rect.rect.size.x,
			rect.lossyScale.y * rect.rect.size.y);

		return new Rect(pos, size).size * 0.5f;
	}

	protected override void OnClick() {
		
	}

	private void Update() {
		if (Input.GetMouseButtonUp(0)) {
			if (!RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition)) {
				OnClickOutside?.Invoke(inputField.text);
				GetComponentInChildren<TMP_SelectionCaret>(true).raycastTarget = false;
			}
			else {
				Debug.Log("Clicked inside");
			}
		}
	}
}
