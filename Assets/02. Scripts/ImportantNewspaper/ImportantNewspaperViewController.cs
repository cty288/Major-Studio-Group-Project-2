using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MikroFramework;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;

public class ImportantNewspaperViewController : DraggableItems {
	
	private List<SpriteRenderer> renderers = new List<SpriteRenderer>();

	[ES3Serializable] private int week = 0;
	
	protected TMP_Text weekText;
	protected GameObject hintCanvas;
	
	
	protected override void Awake() {
		base.Awake();
		renderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();
		weekText = transform.Find("CanvasParent/DateCanvas/Date").GetComponent<TMP_Text>();
		hintCanvas = transform.Find("CanvasParent/Canvas").gameObject;
	}

	public override void SetLayer(int layer) {
		foreach (var renderer in renderers) {
			renderer.sortingOrder = layer;
		}
	}

	protected override void OnClick() {
		this.Delay(0.1f, () => {
			if (this) {
				this.SendCommand<OpenImportantNewspaperUIPanelCommand>(new OpenImportantNewspaperUIPanelCommand(
					week));
				hintCanvas.SetActive(false);
			}
		});
	}

	protected override void Start() {
		base.Start();
		weekText.text = $"Week {week} Newspaper";
	}

	public void SetContent(int week) {
		this.week = week;
		hintCanvas.gameObject.SetActive(true);
		weekText.text = $"Week {week} Newspaper";
	}

	public override void OnThrownToRubbishBin() {
		Destroy(gameObject);
	}
}
public class OpenImportantNewspaperUIPanelCommand : AbstractCommand<OpenImportantNewspaperUIPanelCommand> {
	private int week;
	public OpenImportantNewspaperUIPanelCommand(){}
	public OpenImportantNewspaperUIPanelCommand(int week) {
		this.week = week;
	}
	protected override void OnExecute() {
		this.SendEvent<OnImportantNewspaperUIPanelOpened>(
			new OnImportantNewspaperUIPanelOpened() {Week =  week});
	}
}

public struct OnImportantNewspaperUIPanelOpened {
	public int Week;
}