using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class BountyHunterGiftViewController : DraggableItems {
    private PlayerResourceSystem playerResourceSystem;
    public int FoodCount = 1;
    protected override void Awake() {
        base.Awake();
        playerResourceSystem = this.GetSystem<PlayerResourceSystem>();
        
    }

    

    public override void SetLayer(int layer) {
        
    }

    protected override void OnClick() {
        playerResourceSystem.AddFood(FoodCount);
        GameObject.Destroy(gameObject);
    }

    public override void OnThrownToRubbishBin() {
       
    }
}
