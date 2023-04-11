using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SuspectInfo {
    public string Crime;
    public GoodsInfo rewards;
    
    public SuspectInfo(string crime, GoodsInfo rewards) {
        this.Crime = crime;
        this.rewards = rewards;
    }
}
public class SuspectModel : AbstractSavableModel {
    [field: ES3Serializable]
    private Dictionary<long, SuspectInfo> suspectIDToInfos { get; set; } = new Dictionary<long, SuspectInfo>();

    protected HashSet<long> usedSuspectIDs = new HashSet<long>();

    [field: ES3Serializable] private bool hasMetPoliceBefore = false;

    public bool HasMetPoliceBefore {
        get => hasMetPoliceBefore;
        set => hasMetPoliceBefore = value;
    }

    public void AddSuspect(BodyInfo bodyInfo, string crime, params GoodsInfo[] rewards) {
        if (suspectIDToInfos.ContainsKey(bodyInfo.ID) || bodyInfo.IsDead) {
            return;
        }

        suspectIDToInfos.Add(bodyInfo.ID, new SuspectInfo(crime, rewards[Random.Range(0, rewards.Length)]));
    }
    
    public void RemoveBody(long id) {
        if (!suspectIDToInfos.ContainsKey(id)) {
            return;
        }

        suspectIDToInfos.Remove(id);
    }
    
    public bool IsSuspect(long id) {
        return suspectIDToInfos.ContainsKey(id);
    }
    
    public SuspectInfo GetSuspectInfo(long id) {
        return suspectIDToInfos[id];
    }
    public long GetRandomSuspect(out SuspectInfo info) {
        info = null;
        if (suspectIDToInfos.Count == 0) {
            return -1;
        }

        if(usedSuspectIDs.Count == suspectIDToInfos.Count) {
            usedSuspectIDs.Clear();
        }
        
        var randomIndex = Random.Range(0, suspectIDToInfos.Count);
        List<long> suspectIDs = suspectIDToInfos.Keys.ToList();
        long suspectID = suspectIDs[randomIndex];
        while (usedSuspectIDs.Contains(suspectID)) {
            randomIndex = Random.Range(0, suspectIDToInfos.Count);
            suspectID = suspectIDs[randomIndex];
        }
        info = suspectIDToInfos[suspectID];
        usedSuspectIDs.Add(suspectID);
        return suspectID;
    }
}
