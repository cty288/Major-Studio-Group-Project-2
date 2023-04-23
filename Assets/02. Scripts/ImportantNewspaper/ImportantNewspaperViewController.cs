using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MikroFramework;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;

public class ImportantNewspaperViewController : DraggableItems {
	
	private List<SpriteRenderer> renderers = new List<SpriteRenderer>();

	[ES3Serializable] private int issue = 0;
	
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
					issue));
				hintCanvas.SetActive(false);
			}
		});
	}

	protected override void Start() {
		base.Start();
		weekText.text = $"Newspaper Issue {issue}";
	}

	public void SetContent(int Issue) {
		this.issue = Issue;
		hintCanvas.gameObject.SetActive(true);
		weekText.text = $"Newspaper Issue {Issue}";
	}

	public override void OnThrownToRubbishBin() {
		Destroy(gameObject);
	}
}
public class OpenImportantNewspaperUIPanelCommand : AbstractCommand<OpenImportantNewspaperUIPanelCommand> {
	private int _issue;
	public OpenImportantNewspaperUIPanelCommand(){}
	public OpenImportantNewspaperUIPanelCommand(int issue) {
		this._issue = issue;
	}
	protected override void OnExecute() {
		this.SendEvent<OnImportantNewspaperUIPanelOpened>(
			new OnImportantNewspaperUIPanelOpened() {Issue =  _issue});
	}
}

public struct OnImportantNewspaperUIPanelOpened {
	public int Issue;
}