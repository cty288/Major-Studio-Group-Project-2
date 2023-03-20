using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class MerchantNoteOnTableViewController : DraggableItems {

    [ES3Serializable] protected string content;
    
    
    private SpriteRenderer spriteRenderer;

    protected override void Awake() {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void SetLayer(int layer) {
        spriteRenderer.sortingOrder = layer;
    }

    
    protected override void OnClick() {
        this.SendCommand<OpenMerchantNoteUIPanelCommand>();
    }

    public override void OnThrownToRubbishBin() {
        Destroy(gameObject);
    }
}

public class OpenMerchantNoteUIPanelCommand : AbstractCommand<OpenMerchantNoteUIPanelCommand> {
    public OpenMerchantNoteUIPanelCommand() { }
  
    protected override void OnExecute() {
        this.SendEvent<OnMerchantNoteUIPanelOpened>(
            new OnMerchantNoteUIPanelOpened());
    }
}

public struct OnMerchantNoteUIPanelOpened {
    
}

