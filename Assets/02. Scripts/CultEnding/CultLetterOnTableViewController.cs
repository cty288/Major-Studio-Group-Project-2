using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using NHibernate.Mapping;
using UnityEngine;

public class CultLetterOnTableViewController : DraggableItems {
    [ES3Serializable] private List<string> contents;
    protected SpriteRenderer spriteRenderer;
    protected GameObject hintCanvas;
    

    protected override void Awake() {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hintCanvas = transform.Find("Hint").gameObject;
    }


    public void SetContent(List<string> contents) {
        this.contents = contents;
    }

    public override void SetLayer(int layer) {
        spriteRenderer.sortingOrder = layer;
    }

    protected override void OnClick() {
        this.Delay(0.1f, () => {
            if (this) {
                this.SendCommand<OpenCultistLetterUIPanelCommand>(new OpenCultistLetterUIPanelCommand(contents));
                hintCanvas.SetActive(false);
            }
        });
    }

    public override void OnThrownToRubbishBin() {
        Destroy(gameObject);
    }
}

public class OpenCultistLetterUIPanelCommand : AbstractCommand<OpenPosterUIPanelCommand> {
    protected List<string> contents;

    public OpenCultistLetterUIPanelCommand(){}
    
    public OpenCultistLetterUIPanelCommand(List<string> contents) {
        this.contents = contents;
    }
	
    protected override void OnExecute() {
        this.SendEvent<OnCultistLetterUIPanelOpened>(
            new OnCultistLetterUIPanelOpened() {
                Contents = contents
            });
    }
}

public struct OnCultistLetterUIPanelOpened {
    public List<string> Contents;
}

