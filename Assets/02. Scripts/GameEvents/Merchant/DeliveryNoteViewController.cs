using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class DeliveryNoteViewController : DraggableItems {
    [ES3Serializable] protected string content;

    protected SpriteRenderer spriteRenderer;
    protected override void Awake() {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void SetLayer(int layer) {
        spriteRenderer.sortingOrder = layer;
    }

    public void SetContent(string content) {
        this.content = content;
    }
    protected override void OnClick() {
        this.SendCommand<OpenGeneralNoteUIPanelCommand>(new OpenGeneralNoteUIPanelCommand(content));
    }

    public override void OnThrownToRubbishBin() {
       Destroy(this.gameObject);
    }
}

public class OpenGeneralNoteUIPanelCommand : AbstractCommand<OpenGeneralNoteUIPanelCommand> {
    protected string content;

    public OpenGeneralNoteUIPanelCommand(){}
    public OpenGeneralNoteUIPanelCommand(string content) {
        this.content = content;
    }
  
    protected override void OnExecute() {
        this.SendEvent<OpenGeneralNoteUIPanelEvent>(
            new OpenGeneralNoteUIPanelEvent() {
                Content = content
            });
    }
}

public struct OpenGeneralNoteUIPanelEvent {
    public string Content;
}
