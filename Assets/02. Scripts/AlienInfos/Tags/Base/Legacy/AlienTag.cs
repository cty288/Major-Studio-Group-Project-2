using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using NHibernate.Mapping;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IAlienTag {
    public string TagName { get; }
    string GetRandomRadioDescription(string alienName,out bool isReal);
    string GetRandomRadioDescription(string alienName,bool isReal);
    
    List<string> GetShortDescriptions();
}

[Serializable]
public abstract class AbstractAlienTag : IAlienTag {
    
    public abstract string TagName { get; }
    
    protected BodyTagInfoModel bodyTagInfoModel;
    
    protected void CheckInit() {
        if (bodyTagInfoModel == null) {
            bodyTagInfoModel = MainGame.Interface.GetModel<BodyTagInfoModel>();
        }
    }
    public virtual string GetRandomRadioDescription(string alienName, out bool isReal) {
        CheckInit();
         isReal = Random.Range(0, 2) == 0;
        List<string> targetList = isReal ? bodyTagInfoModel.GetRealRadioDescription(TagName,alienName) : 
            GetFakeRadioDescription(alienName);
        
        return targetList[Random.Range(0, targetList.Count)];
        
    }

    public virtual string GetRandomRadioDescription(string alienName, bool isReal) {
        CheckInit();
        List<string> targetList = isReal ? bodyTagInfoModel.GetRealRadioDescription(TagName, alienName) : 
            GetFakeRadioDescription(alienName);
        if(targetList == null || targetList.Count == 0) return "";
        return targetList[Random.Range(0, targetList.Count)];
    }

    public abstract List<string> GetFakeRadioDescription(string alienName);

    public virtual List<string> GetShortDescriptions() {
        CheckInit();
        return bodyTagInfoModel.GetShortDescriptions(TagName);
    }
}
