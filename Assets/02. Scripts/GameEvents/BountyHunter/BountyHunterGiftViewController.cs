using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class BountyHunterGiftViewController : DraggableItems {
    private PlayerResourceModel playerResourceModel;
    public int FoodCount = 1;
    protected override void Awake() {
        base.Awake();
        // = this.GetSystem<PlayerResourceSystem>();
        playerResourceModel = this.GetModel<PlayerResourceModel>();
    }

    

    public override void SetLayer(int layer) {
        
    }

    protected override void OnClick() {
        playerResourceModel.AddFood(FoodCount);
        GameObject.Destroy(gameObject);
    }

    public override void OnThrownToRubbishBin() {
        Destroy(gameObject);
    }
}
