using System;
using System.Collections;
using System.Collections.Generic;
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
    public BindableProperty<int> FoodCount { get; } = new BindableProperty<int>(4);
    public Dictionary<Type, GoodsInfo> PlayerResources { get; } = new Dictionary<Type, GoodsInfo>();
    
    private int maxFoodCount = 8;
    protected override void OnInit() {
        this.GetSystem<GameTimeManager>().OnDayStart += OnDayEnd;
        AddResource(new GunResource(), 1);
    }

    public void AddFood(int count) {
        FoodCount.Value = Mathf.Min(maxFoodCount, FoodCount.Value + count);
    }

    public void RemoveFood(int count) {
        FoodCount.Value = Mathf.Max(0, FoodCount.Value - count);
    }

    public void AddResource(IPlayerResource resource, int count) {
        if (PlayerResources.ContainsKey(resource.GetType())) {
            PlayerResources[resource.GetType()].Count += count;
        }
        else {
            PlayerResources.Add(resource.GetType(), new GoodsInfo(count, resource.PrefabName, resource.GetType()));
        }
        this.SendEvent<OnPlayerResourceNumberChanged>(new OnPlayerResourceNumberChanged() { GoodsInfo = PlayerResources[resource.GetType()] });
    }

    public void RemoveResource<T>(int count) {
        if (PlayerResources.ContainsKey(typeof(T))) {
            PlayerResources[typeof(T)].Count -= count;
            this.SendEvent<OnPlayerResourceNumberChanged>(new OnPlayerResourceNumberChanged() { GoodsInfo = PlayerResources[typeof(T)] });
        }
        
    }

    public bool HasEnoughResource<T>(int count) where T : IPlayerResource {
        if (PlayerResources.ContainsKey(typeof(T))) {
            return PlayerResources[typeof(T)].Count >= count;
        }
        return false;
    }

    public int GetResourceCount<T>() where T : IPlayerResource
    {
        if (PlayerResources.ContainsKey(typeof(T)))
        {
            return PlayerResources[typeof(T)].Count;
        }
        return 0;
    }

    public int GetResourceCount(Type type)
    {
        if (PlayerResources.ContainsKey(type))
        {
            return PlayerResources[type].Count;
        }
        return 0;
    }
    private void OnDayEnd(int day) {
        if (day <= 1) {
            return;
        }
        FoodCount.Value = Mathf.Max(0, FoodCount.Value - 1);
        if (FoodCount.Value <= 0) {
            this.GetModel<GameStateModel>().GameState.Value = GameState.End;
        }
    }
}
