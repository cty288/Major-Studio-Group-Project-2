
using System;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

public class PlayerResourceModel : AbstractSavableModel {
	[field: ES3Serializable]
	public BindableProperty<int> FoodCount { get; } = new BindableProperty<int>(4);
	[field: ES3Serializable]
	public Dictionary<Type, GoodsInfo> PlayerResources { get; } = new Dictionary<Type, GoodsInfo>();
	
    [field: ES3Serializable]
    private int maxFoodCount = 8;


   
    

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
}
