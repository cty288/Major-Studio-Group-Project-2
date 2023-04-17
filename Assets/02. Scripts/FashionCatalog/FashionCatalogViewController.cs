using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.FashionCatalog;
using MikroFramework;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;

public class FashionCatalogViewController : DraggableItems {
	private GameObject indicateCanvas;
	private GameObject dateCanvas;
	private List<SpriteRenderer> renderers = new List<SpriteRenderer>();
	private SpriteRenderer selfRenderer;
	protected FashionCatalogModel fashionCatalogModel;
	//protected SpriteRenderer spriteRenderer;
    
	[ES3Serializable]
	private DateTime date;

	[ES3Serializable] private int week;

	[ES3Serializable] private int spriteIndex = 0;
	protected override void Awake() {
		base.Awake();
		renderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();
		selfRenderer = GetComponent<SpriteRenderer>();
		indicateCanvas = transform.Find("CanvasParent/Canvas").gameObject;
		dateCanvas = transform.Find("CanvasParent/DateCanvas").gameObject;
		indicateCanvas.SetActive(false);
		dateCanvas.SetActive(false);

		fashionCatalogModel = this.GetModel<FashionCatalogModel>();
		
		//SetLayer(1000);
		
	}

	protected override void Start() {
		base.Start();
		dateCanvas.transform.GetChild(0).GetComponent<TMP_Text>().text =
			$"Week {week} Fashion Book";
		selfRenderer.sprite = FashionCatalogAssets.Singleton.GetSprite(spriteIndex);
	}


	public override void SetLayer(int layer) {
		foreach (var renderer in renderers) {
			renderer.sortingOrder = layer;
		}
		//indicateCanvas.GetComponent<Canvas>().sortingOrder = layer;
		dateCanvas.GetComponent<Canvas>().sortingOrder = 1000;
	}

	public void SetContent(DateTime time, int week, int spriteIndex) {
		this.date = time;
		this.week = week;
		indicateCanvas.gameObject.SetActive(true);
		dateCanvas.transform.GetChild(0).GetComponent<TMP_Text>().text =
			$"Week {week} Fashion Book";
		this.spriteIndex = spriteIndex;
		selfRenderer.sprite = FashionCatalogAssets.Singleton.GetSprite(spriteIndex);
	}
	protected override void OnClick() {
		indicateCanvas.gameObject.SetActive(false);
		this.Delay(0.1f, () => {
			if (this) {
				this.SendCommand<OpenFashionCatalogUIPanelCommand>(new OpenFashionCatalogUIPanelCommand(
					date, true, week));
			}
		});
	}

	public override void OnThrownToRubbishBin() {
		fashionCatalogModel.RemoveBodyPartIndicesUpdateInfo(date);
		Destroy(this.gameObject);
	}
}
public class OpenFashionCatalogUIPanelCommand : AbstractCommand<OpenFashionCatalogUIPanelCommand> {
	private DateTime date;
	private bool isOpen;
	private int week;
	public OpenFashionCatalogUIPanelCommand(){}
	public OpenFashionCatalogUIPanelCommand(DateTime date, bool isOpen, int week) {
		this.date = date;
		this.isOpen = isOpen;
		this.week = week;
	}
	protected override void OnExecute() {
		this.SendEvent<OnFashionCatalogUIPanelOpened>(
			new OnFashionCatalogUIPanelOpened() {Date = date, IsOpen = isOpen, Week = week});
	}
}

public struct OnFashionCatalogUIPanelOpened {
	public DateTime Date;
	public bool IsOpen;
	public int Week;
}
