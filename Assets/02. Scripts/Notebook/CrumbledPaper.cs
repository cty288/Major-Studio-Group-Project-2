using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbledPaper : DraggableItems {
	protected SpriteRenderer spriteRenderer;
	protected Canvas hintCanvas;

	protected override void Awake() {
		base.Awake();
		spriteRenderer = GetComponent<SpriteRenderer>();
		hintCanvas = transform.Find("Info").GetComponent<Canvas>();
	}

	public override void SetLayer(int layer) {
		spriteRenderer.sortingOrder = layer;
		hintCanvas.sortingOrder = layer * 1000;
	}

	protected override void OnClick() {
	}

	public override void OnThrownToRubbishBin() {
		Destroy(gameObject);
	}
}
