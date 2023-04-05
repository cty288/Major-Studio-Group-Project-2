using MikroFramework.ResKit;
using MikroFramework.Singletons;
using System;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.BodyManagmentSystem;
using JetBrains.Annotations;
using NHibernate.Mapping;
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

public class AlienBodyPartCollections : MonoMikroSingleton<AlienBodyPartCollections> {
    public BodyPartCollection HeadBodyPartPrefabs;
    [FormerlySerializedAs("MainBodyPartPrefabs")]
    public BodyPartCollection MainBodyPartPrefabs;
    public BodyPartCollection LegBodyPartPrefabs;

    public BodyPartCollection SpecialBodyPartPrefabs;

    public List<BodyPartCollection> AccessoryCollections;

    private void Start() {
        //MainGame.Interface.GetModel<BodyTagInfoModel>().LoadInfo();
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
    public BodyPartPrefabInfo TryGetHeightOppositeBodyPartInfo(BodyPartDisplayType displayType, AlienBodyPartInfo original, bool originalIsAlienOnly, HeightType height,
        bool isSpecial = false) {

        BodyPartDisplays originalDisplays = null;
        BodyPartDisplays targetDisplays = null;

        HeightType originalHeight = height;
        HeightType targetHeight = originalHeight == HeightType.Short ? HeightType.Tall : HeightType.Short;


        BodyPartHeightSubCollection targetSubCollection = TryGetBodyPartHeightSubCollection(GetBodyPartCollectionByBodyType(original.BodyPartType, isSpecial), targetHeight);
        targetDisplays = GetBodyPartDisplayByType(targetSubCollection, displayType);
        

        BodyPartHeightSubCollection originalSubCollection = TryGetBodyPartHeightSubCollection(GetBodyPartCollectionByBodyType(original.BodyPartType, isSpecial), height);
        originalDisplays = GetBodyPartDisplayByType(originalSubCollection, displayType);

        int index = originalIsAlienOnly ? originalDisplays.AlienOnlyPartsPrefabs.IndexOf(original.gameObject) : originalDisplays.HumanTraitPartsPrefabs.IndexOf(original.gameObject);

        List<GameObject> targetList = originalIsAlienOnly ? targetDisplays.AlienOnlyPartsPrefabs : targetDisplays.HumanTraitPartsPrefabs;
        if (index >= 0 && index < targetList.Count) {
            return targetList[index].GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo();
        }
        else {
            return original.GetBodyPartPrefabInfo();
        }
    }
    

    public BodyPartPrefabInfo GetRandomBodyPartInfo(BodyPartDisplayType displayType, BodyPartType bodyPartType, bool isAlien, HeightType height, bool isDinstinct,
       [CanBeNull] HashSet<int> usedIndices, int subBodyPartChance) {
      
        BodyPartCollection collection = GetBodyPartCollectionByBodyType(bodyPartType, false);
        BodyPartHeightSubCollection subCollection = TryGetBodyPartHeightSubCollection(collection, height);
        BodyPartDisplays targetDisplays = GetBodyPartDisplayByType(subCollection, displayType);
        return GetRandomBodyPartInfo(targetDisplays, isAlien, isDinstinct, usedIndices, subBodyPartChance);
    }


    
    public BodyPartPrefabInfo GetBodyPartInfoForDisplay(BodyPartDisplayType targetDisplay, BodyPartDisplayType originalDisplay, AlienBodyPartInfo originalBodyPart, HeightType height, float reality,
        bool isSpecialType, int subBodyPartIndex) {
        if (originalBodyPart == null) {
            return null;
        }
        BodyPartCollection collection = GetBodyPartCollectionByBodyType(originalBodyPart.BodyPartType, isSpecialType);
        BodyPartHeightSubCollection subCollection = TryGetBodyPartHeightSubCollection(collection, height);

        BodyPartDisplays originalDisplays = GetBodyPartDisplayByType(subCollection, originalDisplay);
        BodyPartDisplays targetDisplays = GetBodyPartDisplayByType(subCollection, targetDisplay);

        List<GameObject> originalList = originalBodyPart.IsAlienOnly ? originalDisplays.AlienOnlyPartsPrefabs : originalDisplays.HumanTraitPartsPrefabs;
        List<GameObject> targetList = originalBodyPart.IsAlienOnly ? targetDisplays.AlienOnlyPartsPrefabs : targetDisplays.HumanTraitPartsPrefabs;

        int index = originalList.IndexOf(originalBodyPart.gameObject);
        if (index >= 0 && index < targetList.Count) {
            if (reality >= Random.Range(0f, 1f)) {
                return targetList[index].GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo(subBodyPartIndex, true);
            }
            else {
                return targetList[Random.Range(0, targetList.Count)].GetComponent<AlienBodyPartInfo>()
                    .GetBodyPartPrefabInfo(subBodyPartIndex, true);
            }
           
        }

        return null;
    }

    public BodyPartHeightSubCollection TryGetBodyPartHeightSubCollection(BodyPartCollection collection, HeightType height) {

        int targetHeightListIndex = (int)height;

        var subCollections = collection.HeightSubCollections;

        if (targetHeightListIndex < 0 || targetHeightListIndex >= subCollections.Count) {
            targetHeightListIndex = 0;
        }

        return subCollections[targetHeightListIndex];
    }
    public BodyPartDisplays GetBodyPartDisplayByType(BodyPartHeightSubCollection collection, BodyPartDisplayType displayType) {
        switch (displayType) {
            case BodyPartDisplayType.Shadow:
                return collection.ShadowBodyPartPrefabs;
            case BodyPartDisplayType.Newspaper:
                return collection.NewspaperBodyPartDisplays;
        }

        return null;
    }
   
    private BodyPartPrefabInfo GetRandomBodyPartInfo(BodyPartDisplays targetDisplays, bool isAlien, bool isDistinct,
        HashSet<int> usedIndexes, int subBodyPartChance) {
        List<GameObject> targetList = new List<GameObject>(targetDisplays.HumanTraitPartsPrefabs);
        List<GameObject> finalList;
        if (usedIndexes == null) {
            finalList = targetList;
        }
        else {
            finalList = new List<GameObject>();
            for (int i = 0; i < targetList.Count; i++) {
                if (usedIndexes.Contains(i)) {
                    finalList.Add(targetList[i]);
                }
            }
        }
        
        if (isAlien) {
            finalList.AddRange(targetDisplays.AlienOnlyPartsPrefabs);
        }

        var oldTargetList = finalList;
        if (isDistinct) {
            finalList = finalList.FindAll((obj) => {
                return obj.GetComponent<AlienBodyPartInfo>().SelfTags.Exists((alienTag => alienTag is DistinctiveTag));
            });
            if (finalList.Count == 0) {
                finalList = oldTargetList;
            }
        }
        
        if(finalList.Count == 0) {
            finalList.AddRange(targetList);
        }


        BodyPartPrefabInfo info = finalList[Random.Range(0, finalList.Count)].GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo(subBodyPartChance);
        
        //info.IsAlienOnly = targetDisplays.AlienOnlyPartsPrefabs.Contains(info.gameObject);
        return info;
    }

    public BodyPartCollection GetBodyPartCollectionByBodyType(BodyPartType type, bool isSpecial) {
        if (isSpecial) {
            return SpecialBodyPartPrefabs;
        }
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
