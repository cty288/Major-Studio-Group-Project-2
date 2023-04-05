using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspectModel : AbstractSavableModel {
    [field: ES3Serializable]
    private Dictionary<long, List<GoodsInfo>> suspectIDToRewards { get; set; } = new Dictionary<long, List<GoodsInfo>>();

    public void AddSuspect(BodyInfo bodyInfo, params GoodsInfo[] rewards) {
        if (suspectIDToRewards.ContainsKey(bodyInfo.ID) || bodyInfo.IsDead) {
            return;
        }

        suspectIDToRewards.Add(bodyInfo.ID, new List<GoodsInfo>(rewards));
    }
    
    public void RemoveBody(long id) {
        if (!suspectIDToRewards.ContainsKey(id)) {
            return;
        }

        suspectIDToRewards.Remove(id);
    }
    
    public long GetRandomSuspect(out GoodsInfo reward) {
        reward = null;
        if (suspectIDToRewards.Count == 0) {
            return -1;
        }

        var randomIndex = Random.Range(0, suspectIDToRewards.Count);
        var enumerator = suspectIDToRewards.GetEnumerator();
        for (var i = 0; i < randomIndex; i++) {
            enumerator.MoveNext();
        }

        var randomSuspect = enumerator.Current;
        reward = randomSuspect.Value[Random.Range(0, randomSuspect.Value.Count)];
        return randomSuspect.Key;
    }
}
