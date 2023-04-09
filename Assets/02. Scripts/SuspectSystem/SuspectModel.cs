using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspectModel : AbstractSavableModel {
    [field: ES3Serializable]
    private Dictionary<long, GoodsInfo> suspectIDToRewards { get; set; } = new Dictionary<long, GoodsInfo>();

    public void AddSuspect(BodyInfo bodyInfo, params GoodsInfo[] rewards) {
        if (suspectIDToRewards.ContainsKey(bodyInfo.ID) || bodyInfo.IsDead) {
            return;
        }

        suspectIDToRewards.Add(bodyInfo.ID, rewards[Random.Range(0, rewards.Length)]);
    }
    
    public void RemoveBody(long id) {
        if (!suspectIDToRewards.ContainsKey(id)) {
            return;
        }

        suspectIDToRewards.Remove(id);
    }
    
    public bool IsSuspect(long id) {
        return suspectIDToRewards.ContainsKey(id);
    }
    
    public GoodsInfo GetReward(long id) {
        return suspectIDToRewards[id];
    }
    public long GetRandomSuspect(out GoodsInfo reward) {
        reward = null;
        if (suspectIDToRewards.Count == 0) {
            return -1;
        }

        var randomIndex = Random.Range(0, suspectIDToRewards.Count);
        int currentIndex = 0;
        foreach (var pair in suspectIDToRewards) {
            if (currentIndex == randomIndex) {
                reward = pair.Value;
                return pair.Key;
            }

            currentIndex++;
        }
        
        return -1;
    }
}
