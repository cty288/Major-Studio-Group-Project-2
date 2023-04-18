using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

public struct OnPlayerResourceNumberChanged {
    public GoodsInfo GoodsInfo;
    public int OldValue;
    public int NewValue;
}

public class GoodsInfo {
    public int Count;
    public string PrefabName;
    public Type Type;
    public int MaxCount;
    public string DisplayName;

    public GoodsInfo(int count, string prefabName, Type type, int maxCount, string DisplayName)
    {
        Count = count;
        PrefabName = prefabName;
        this.Type = type;
        MaxCount = maxCount;
        this.DisplayName = DisplayName;
    }


    public static GoodsInfo GetGoodsInfo(IPlayerResource resource, int count) {
        return new GoodsInfo(count, resource.PrefabName, resource.GetType(), resource.MaxCount, resource.DisplayName);
    }
}
public class PlayerResourceSystem : AbstractSystem {
   
    protected PlayerResourceModel playerResourceModel;
    
    protected override void OnInit() {
       
        playerResourceModel = this.GetModel<PlayerResourceModel>();
        this.GetSystem<GameTimeManager>().OnDayStart += OnDayEnd;

        this.RegisterEvent<OnNewDay>(OnNewDay);
        
    }

    private void OnNewDay(OnNewDay e) {
        if (e.Day == 0) {
            HotUpdateDataModel hotUpdateDataModel = this.GetModel<HotUpdateDataModel>();
            playerResourceModel.AddResource(new GunResource(), 1);
            playerResourceModel.AddFood(int.Parse(hotUpdateDataModel.GetData("Initial_Food_Count").values[0]));
        }
    }


    private void OnDayEnd(int day, int hour) {
        if (day <= 1) {
            return;
        }
        if (!playerResourceModel.HasEnoughResource<FoodResource>(1)) {
            this.GetModel<GameStateModel>().GameState.Value = GameState.End;
        }

        playerResourceModel.RemoveFood(1);

    }
}
