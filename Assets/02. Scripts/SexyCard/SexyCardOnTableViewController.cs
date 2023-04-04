using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.SexyCard;
using MikroFramework;
using MikroFramework.Architecture;
using UnityEngine;

public class SexyCardOnTableViewController : DraggableItems{
	private List<SpriteRenderer> renderers = new List<SpriteRenderer>();
	protected SpriteRenderer spriteRenderer;
	protected GameObject hintCanvas;
	protected SexyCardModel model;
	protected override void Awake() {
		base.Awake();
		model = this.GetModel<SexyCardModel>();
		renderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();
		spriteRenderer = GetComponent<SpriteRenderer>();
		hintCanvas = transform.Find("Hint").gameObject;
		spriteRenderer.sprite = model.SexyCardSprite;
	}

	public override void SetLayer(int layer) {
		foreach (var renderer in renderers) {
			renderer.sortingOrder = layer;
		}
	}

	protected override void OnClick() {
		this.Delay(0.1f, () => {
			if (this) {
				this.SendCommand<OpenSexyCardUIPanelCommand>(new OpenSexyCardUIPanelCommand());
				hintCanvas.SetActive(false);
			}
		});
	}

	public override void OnThrownToRubbishBin() {
		Destroy(this.gameObject);
	}
}
public class OpenSexyCardUIPanelCommand : AbstractCommand<OpenPosterUIPanelCommand> {
	
	public OpenSexyCardUIPanelCommand(){}
	
	protected override void OnExecute() {
		this.SendEvent<OnSexyCardUIPanelOpened>(
			new OnSexyCardUIPanelOpened() {});
	}
}

public struct OnSexyCardUIPanelOpened {
	
}
