using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

public struct OnPlayerResourceNumberChanged {
    public GoodsInfo GoodsInfo;
}

public class GoodsInfo {
    public int Count;
    public string PrefabName;
    public Type Type;

    public GoodsInfo(int count, string prefabName, Type type)
    {
        Count = count;
        PrefabName = prefabName;
        this.Type = type;
    }
}
public class PlayerResourceSystem : AbstractSystem {
   
    protected PlayerResourceModel playerResourceModel;
    
    protected override void OnInit() {
        playerResourceModel = this.GetModel<PlayerResourceModel>();
        this.GetSystem<GameTimeManager>().OnDayStart += OnDayEnd;
        if (!playerResourceModel.HasEnoughResource<GunResource>(1)) {
            playerResourceModel.AddResource(new GunResource(), 1);
        }
    }

   
    private void OnDayEnd(int day, int hour) {
        if (day <= 1) {
            return;
        }
        playerResourceModel.FoodCount.Value = Mathf.Max(0,  playerResourceModel.FoodCount.Value - 1);
        if (playerResourceModel.FoodCount.Value <= 0) {
            this.GetModel<GameStateModel>().GameState.Value = GameState.End;
        }
    }
}
