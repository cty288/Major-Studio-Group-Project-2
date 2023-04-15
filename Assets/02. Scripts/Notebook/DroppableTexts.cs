using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Notebook;
using DG.Tweening;
using TMPro;
using UnityEngine;


[ES3Serializable]
[Serializable]
public class DroppableTextInfo : DroppableInfo{
	[ES3Serializable]
	private string text;
	
	[ES3Serializable]
	private GameObject contentUIPrefab;
	
	
	public DroppableTextInfo(){}
	
	public DroppableTextInfo(string text, GameObject prefab){
		this.text = text;
		this.contentUIPrefab = prefab;
	}
	
	[field: ES3Serializable]
	public override bool IsDefaultOnTop { get; } = false;
	
	public override DroppedUIObjectViewController GetContentUIObject(RectTransform parent) {
		NotebookDroppedText droppedText =
			GameObject.Instantiate(contentUIPrefab).GetComponent<DroppedUIObjectViewController>() as
				NotebookDroppedText;
		droppedText.SetContent(text);
		droppedText.transform.SetParent(parent);
		//droppedText.GetComponent<RectTransform>().localPosition = Vector3.zero;
		
		return droppedText;
	}
}
public class DroppableTexts : MonoBehaviour, IDroppable {
	
	private TMP_Text text;
	[SerializeField] private GameObject contentUIPrefab;

	private void Awake() {
		text = GetComponent<TMP_Text>();
	}

	public void SetContent(string content, Color color, Vector2 targetPos) {
		text.fontSize = text.fontSize;
		text.color = color;
		text.text = content;
		text.transform.SetParent(text.transform.parent);
		text.transform.SetAsLastSibling();
		text.transform.position = Input.mousePosition;
		if (targetPos != Vector2.zero) {
			text.transform.DOMove(targetPos, 0.5f).OnComplete(OnDropped);
		}
		
	}
	

	public DroppableInfo GetDroppableInfo() {
		return new DroppableTextInfo(text.text, contentUIPrefab);
	}

	public void OnDropped() {
		Destroy(gameObject);
	}
}
