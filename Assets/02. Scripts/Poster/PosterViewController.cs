using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.Poster;
using MikroFramework;
using MikroFramework.Architecture;
using UnityEngine;

public class PosterViewController : DraggableItems {
	private List<SpriteRenderer> renderers = new List<SpriteRenderer>();

	[ES3Serializable] private string id = "";
	protected GameObject hintCanvas;
	protected PosterModel posterModel;

	protected override void Awake() {
		base.Awake();
		posterModel = this.GetModel<PosterModel>();
		renderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();
		hintCanvas = transform.Find("CanvasParent/Canvas").gameObject;
	}

	public override void SetLayer(int layer) {
		foreach (var renderer in renderers) {
			renderer.sortingOrder = layer;
		}
	}
	
	public void SetContent(string id) {
		this.id = id;
		hintCanvas.gameObject.SetActive(true);
	}

	protected override void OnClick() {
		this.Delay(0.1f, () => {
			if (this) {
				this.SendCommand<OpenPosterUIPanelCommand>(new OpenPosterUIPanelCommand(
					id));
				hintCanvas.SetActive(false);
			}
		});
	}

	public override void OnThrownToRubbishBin() {
		posterModel.RemovePoster(id);
		Destroy(gameObject);
		
	}
}


public class OpenPosterUIPanelCommand : AbstractCommand<OpenPosterUIPanelCommand> {
	private string id;
	public OpenPosterUIPanelCommand(){}
	public OpenPosterUIPanelCommand(string id) {
		this.id = id;
	}
	protected override void OnExecute() {
		this.SendEvent<OnPosterUIPanelOpened>(
			new OnPosterUIPanelOpened() {id = id});
	}
}

public struct OnPosterUIPanelOpened {
	public string id;
}
