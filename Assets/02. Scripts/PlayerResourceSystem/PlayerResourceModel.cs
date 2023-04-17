
using System;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;


public class FoodResource : IPlayerResource {
    public string PrefabName { get; } = "Food";
    public int MaxCount { get; } = 8;
    public string DisplayName { get; } = "Cans";
}

public class PlayerResourceModel : AbstractSavableModel {
	//[field: ES3Serializable]
	//public BindableProperty<int> FoodCount { get; } = new BindableProperty<int>(5);
	[field: ES3Serializable]
	public Dictionary<Type, GoodsInfo> PlayerResources { get; } = new Dictionary<Type, GoodsInfo>();
	
    [field: ES3Serializable]
    private int maxFoodCount = 8;


   
    

    public void AddFood(int count) {
        //FoodCount.Value = Mathf.Min(maxFoodCount, FoodCount.Value + count);
        AddResource(new FoodResource(), count);
    }
    
    public void RemoveFood(int count) {
        RemoveResource<FoodResource>(count);
    }

    
    public void AddResource(IPlayerResource resource, int count) {
        int oldCount = 0;
        if (PlayerResources.ContainsKey(resource.GetType())) {
            oldCount = PlayerResources[resource.GetType()].Count;
            int maxCount = PlayerResources[resource.GetType()].MaxCount;
            PlayerResources[resource.GetType()].Count = Mathf.Min(maxCount, oldCount + count);
        }
        else {
            PlayerResources.Add(resource.GetType(), new GoodsInfo(Mathf.Min(count, resource.MaxCount), resource.PrefabName, resource.GetType(), resource.MaxCount,
                resource.DisplayName));
        }
        this.SendEvent<OnPlayerResourceNumberChanged>(new OnPlayerResourceNumberChanged() {
            GoodsInfo = PlayerResources[resource.GetType()],
            OldValue = oldCount,
            NewValue = PlayerResources[resource.GetType()].Count
        });
    }
    public void AddResources(GoodsInfo goodsInfo) {
        int oldCount = 0;
        if (PlayerResources.ContainsKey(goodsInfo.Type)) {
            oldCount = PlayerResources[goodsInfo.Type].Count;
            int maxCount = PlayerResources[goodsInfo.Type].MaxCount;
            PlayerResources[goodsInfo.Type].Count = Mathf.Min(maxCount, oldCount + goodsInfo.Count);
        }
        else {
            PlayerResources.Add(goodsInfo.Type, new GoodsInfo(goodsInfo.Count, goodsInfo.PrefabName, goodsInfo.Type, goodsInfo.MaxCount,
                goodsInfo.DisplayName));
        }
        
        this.SendEvent<OnPlayerResourceNumberChanged>(new OnPlayerResourceNumberChanged() {
            GoodsInfo = PlayerResources[goodsInfo.Type],
            OldValue = oldCount,
            NewValue = PlayerResources[goodsInfo.Type].Count
        });
    }
    public void RemoveResource<T>(int count) {
        if (PlayerResources.ContainsKey(typeof(T))) {
            if(PlayerResources[typeof(T)].Count> 0) {
                int oldCount = PlayerResources[typeof(T)].Count;
                PlayerResources[typeof(T)].Count = Mathf.Max(0, PlayerResources[typeof(T)].Count - count);
                this.SendEvent<OnPlayerResourceNumberChanged>(new OnPlayerResourceNumberChanged() {
                    GoodsInfo = PlayerResources[typeof(T)],
                    OldValue = oldCount,
                    NewValue = PlayerResources[typeof(T)].Count
                });
            }
           
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
