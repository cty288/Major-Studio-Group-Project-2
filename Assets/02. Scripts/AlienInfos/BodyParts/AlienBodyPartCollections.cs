using MikroFramework.ResKit;
using MikroFramework.Singletons;
using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.BodyManagmentSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public enum BodyPartDisplayType {
    Newspaper,
    Shadow
}


[Serializable]
public class BodyPartDisplays {
    public List<GameObject> HumanTraitPartsPrefabs;
    [HideInInspector]
    public List<GameObject> AlienOnlyPartsPrefabs;
}

[Serializable]
public class BodyPartHeightSubCollection {
    public BodyPartDisplays ShadowBodyPartPrefabs;
    public BodyPartDisplays NewspaperBodyPartDisplays;
}

[Serializable]
public class BodyPartCollection {
    public List<BodyPartHeightSubCollection> HeightSubCollections;
}

public class AlienBodyPartCollections : MonoPersistentMikroSingleton<AlienBodyPartCollections> {
    public BodyPartCollection HeadBodyPartPrefabs;
    [FormerlySerializedAs("MainBodyPartPrefabs")]
    public BodyPartCollection MainBodyPartPrefabs;
    public BodyPartCollection LegBodyPartPrefabs;

    public BodyPartCollection SpecialBodyPartPrefabs;

    private void Start() {
        MainGame.Interface.GetModel<BodyTagInfoModel>().LoadInfo();
        MainGame.Interface.GetUtility<ResLoader>();
    }

    /// <summary>
    ///E.g. High alien body part -> low alien body part
    /// </summary>
    /// <param name="displayType"></param>
    /// <param name="original"></param>
    /// <param name="originalIsAlienOnly"></param>
    /// <param name="originalHeight"></param>
    /// <returns></returns>
    public AlienBodyPartInfo TryGetHeightOppositeBodyPartInfo(BodyPartDisplayType displayType, AlienBodyPartInfo original, bool originalIsAlienOnly, HeightType height) {

        BodyPartDisplays originalDisplays = null;
        BodyPartDisplays targetDisplays = null;

        HeightType originalHeight = height;
        HeightType targetHeight = originalHeight == HeightType.Short ? HeightType.Tall : HeightType.Short;


        BodyPartHeightSubCollection targetSubCollection = TryGetBodyPartHeightSubCollection(GetBodyPartCollectionByBodyType(original.BodyPartType), targetHeight);
        targetDisplays = GetBodyPartDisplayByType(targetSubCollection, displayType);
        

        BodyPartHeightSubCollection originalSubCollection = TryGetBodyPartHeightSubCollection(GetBodyPartCollectionByBodyType(original.BodyPartType), height);
        originalDisplays = GetBodyPartDisplayByType(originalSubCollection, displayType);

        int index = originalIsAlienOnly ? originalDisplays.AlienOnlyPartsPrefabs.IndexOf(original.gameObject) : originalDisplays.HumanTraitPartsPrefabs.IndexOf(original.gameObject);

        List<GameObject> targetList = originalIsAlienOnly ? targetDisplays.AlienOnlyPartsPrefabs : targetDisplays.HumanTraitPartsPrefabs;
        if (index >= 0 && index < targetList.Count) {
            return targetList[index].GetComponent<AlienBodyPartInfo>();
        }
        else {
            return original;
        }
    }
    

    public AlienBodyPartInfo GetRandomBodyPartInfo(BodyPartDisplayType displayType, BodyPartType bodyPartType, bool isAlien, HeightType height, bool isDinstinct) {
      
        BodyPartCollection collection = GetBodyPartCollectionByBodyType(bodyPartType);
        BodyPartHeightSubCollection subCollection = TryGetBodyPartHeightSubCollection(collection, height);
        BodyPartDisplays targetDisplays = GetBodyPartDisplayByType(subCollection, displayType);
        return GetRandomBodyPartInfo(targetDisplays, isAlien, isDinstinct);
    }


    
    public AlienBodyPartInfo GetBodyPartInfoForDisplay(BodyPartDisplayType targetDisplay, BodyPartDisplayType originalDisplay, AlienBodyPartInfo originalBodyPart, HeightType height, float reality) {
        BodyPartCollection collection = GetBodyPartCollectionByBodyType(originalBodyPart.BodyPartType);
        BodyPartHeightSubCollection subCollection = TryGetBodyPartHeightSubCollection(collection, height);

        BodyPartDisplays originalDisplays = GetBodyPartDisplayByType(subCollection, originalDisplay);
        BodyPartDisplays targetDisplays = GetBodyPartDisplayByType(subCollection, targetDisplay);

        List<GameObject> originalList = originalBodyPart.IsAlienOnly ? originalDisplays.AlienOnlyPartsPrefabs : originalDisplays.HumanTraitPartsPrefabs;
        List<GameObject> targetList = originalBodyPart.IsAlienOnly ? targetDisplays.AlienOnlyPartsPrefabs : targetDisplays.HumanTraitPartsPrefabs;

        int index = originalList.IndexOf(originalBodyPart.gameObject);
        if (index >= 0 && index < targetList.Count) {
            if (reality >= Random.Range(0f, 1f)) {
                return targetList[index].GetComponent<AlienBodyPartInfo>();
            }
            else {
                return targetList[Random.Range(0, targetList.Count)].GetComponent<AlienBodyPartInfo>();
            }
           
        }

        return null;
    }

    private BodyPartHeightSubCollection TryGetBodyPartHeightSubCollection(BodyPartCollection collection, HeightType height) {

        int targetHeightListIndex = (int)height;

        var subCollections = collection.HeightSubCollections;

        if (targetHeightListIndex < 0 || targetHeightListIndex >= subCollections.Count) {
            targetHeightListIndex = 0;
        }

        return subCollections[targetHeightListIndex];
    }
    private BodyPartDisplays GetBodyPartDisplayByType(BodyPartHeightSubCollection collection, BodyPartDisplayType displayType) {
        switch (displayType) {
            case BodyPartDisplayType.Shadow:
                return collection.ShadowBodyPartPrefabs;
            case BodyPartDisplayType.Newspaper:
                return collection.NewspaperBodyPartDisplays;
        }

        return null;
    }
   
    private AlienBodyPartInfo GetRandomBodyPartInfo(BodyPartDisplays targetDisplays, bool isAlien, bool isDistinct) {
        List<GameObject> targetList = new List<GameObject>(targetDisplays.HumanTraitPartsPrefabs);
        if (isAlien) {
            targetList.AddRange(targetDisplays.AlienOnlyPartsPrefabs);
        }


        if (isDistinct) {
            targetList = targetList.FindAll((obj) => {
                return obj.GetComponent<AlienBodyPartInfo>().Tags.Exists((alienTag => alienTag is DistinctiveTag));
            });
        }
        
        AlienBodyPartInfo info = targetList[Random.Range(0, targetList.Count)].GetComponent<AlienBodyPartInfo>();
        info.IsAlienOnly = targetDisplays.AlienOnlyPartsPrefabs.Contains(info.gameObject);
        return info;
    }

    private BodyPartCollection GetBodyPartCollectionByBodyType(BodyPartType type) {
        switch (type) {
            case BodyPartType.Body:
                return MainBodyPartPrefabs;
            case BodyPartType.Head:
                return HeadBodyPartPrefabs;
            case BodyPartType.Legs:
                return LegBodyPartPrefabs;
        }

        return null;
    }
}
