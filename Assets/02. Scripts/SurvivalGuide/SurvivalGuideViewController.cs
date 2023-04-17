using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using UnityEngine;

public class SurvivalGuideViewController : DraggableItems {
	protected GameObject hintCanvas;
	//[SerializeField] protected OpenableUIPanel openableUIPanel;
	protected override void Awake() {
		base.Awake();
		hintCanvas = transform.Find("CanvasParent/Canvas").gameObject;
	}

	public override void SetLayer(int layer) {
		
	}

	protected override void OnClick() {
		this.Delay(0.1f, () => {
			if (this) {
				//openableUIPanel.Show(0.5f);
				this.SendCommand<OpenSurvivalGuideUIPanelCommand>();
				hintCanvas.SetActive(false);
			}
		});
	}

	public override void OnThrownToRubbishBin() {
		Destroy(gameObject);
	}
}

public class OpenSurvivalGuideUIPanelCommand : AbstractCommand<OpenSurvivalGuideUIPanelCommand> {
	
	public OpenSurvivalGuideUIPanelCommand(){}
	
	protected override void OnExecute() {
		this.SendEvent<OpenSurvivalGuideUIPanel>(
			new OpenSurvivalGuideUIPanel() {});
	}
}

public struct OpenSurvivalGuideUIPanel {
	
}
