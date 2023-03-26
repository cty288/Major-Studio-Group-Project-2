using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImportantNewspaperViewController : DraggableItems {
	
	private List<SpriteRenderer> renderers = new List<SpriteRenderer>();

	[ES3Serializable] private int week;
	protected override void Awake() {
		base.Awake();
		renderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();
	}

	public override void SetLayer(int layer) {
		foreach (var renderer in renderers) {
			renderer.sortingOrder = layer;
		}
	}

	protected override void OnClick() {
		
	}

	public void SetContent(int week) {
		this.week = week;
	}

	public override void OnThrownToRubbishBin() {
		Destroy(gameObject);
	}
}
