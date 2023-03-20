using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class CameraViewController : DraggableItems {
    private PlayerResourceModel playerResourceModel;
    protected override void Awake() {
        base.Awake();
        // = this.GetSystem<PlayerResourceSystem>();
        playerResourceModel = this.GetModel<PlayerResourceModel>();
    }

    

    public override void SetLayer(int layer) {
        
    }

    protected override void OnClick()
    {
        this.GetModel<PhotoSaveModel>().HasCamera.Value = true;
        GameObject.Destroy(gameObject);
    }

    public override void OnThrownToRubbishBin() {
        Destroy(gameObject);
    }
}