using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;

public class DeliveryNoteViewController : DraggableItems {
    [ES3Serializable] protected string content;
    [ES3Serializable] protected string title = "A Note";

    protected TMP_Text titleText;
    protected SpriteRenderer spriteRenderer;
    protected override void Awake() {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        titleText = transform.Find("Canvas/Title").GetComponent<TMP_Text>();
    }

    public override void SetLayer(int layer) {
        spriteRenderer.sortingOrder = layer;
    }

    protected override void Start() {
        base.Start();
        titleText.text = title;
    }

    public void SetContent(string content, string title = "A Note") {
        this.content = content;
        this.title = title;
        titleText.text = this.title;
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
