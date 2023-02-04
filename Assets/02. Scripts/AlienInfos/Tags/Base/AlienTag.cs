using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using NHibernate.Mapping;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IAlienTag {
    
    string GetRandomRadioDescription(out bool isReal);
    string GetRandomRadioDescription(bool isReal);
    
    List<string> GetShortDescriptions();
}

[Serializable]
public abstract class AbstractAlienTag : IAlienTag {
    
    public abstract string TagName { get; }
    
    private BodyTagInfoModel bodyTagInfoModel;
    
    protected void CheckInit() {
        if (bodyTagInfoModel == null) {
            bodyTagInfoModel = MainGame.Interface.GetModel<BodyTagInfoModel>();
        }
    }
    public virtual string GetRandomRadioDescription(out bool isReal) {
        CheckInit();
         isReal = Random.Range(0, 2) == 0;
        List<string> targetList = isReal ? bodyTagInfoModel.GetRealRadioDescription(TagName) : 
            bodyTagInfoModel.GetFakeRadioDescription(TagName);
        
        return targetList[Random.Range(0, targetList.Count)];
        
    }

    public virtual string GetRandomRadioDescription(bool isReal) {
        CheckInit();
        List<string> targetList = isReal ? bodyTagInfoModel.GetRealRadioDescription(TagName) : 
            bodyTagInfoModel.GetFakeRadioDescription(TagName);
        return targetList[Random.Range(0, targetList.Count)];
    }

    public virtual List<string> GetShortDescriptions() {
        CheckInit();
        return bodyTagInfoModel.GetShortDescriptions(TagName);
    }
}
