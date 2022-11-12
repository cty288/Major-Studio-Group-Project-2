using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ResKit;
using MikroFramework.Singletons;
using UnityEngine;
using Random = UnityEngine.Random;


[Serializable]
public struct BodyPartCollection {
    public List<GameObject> HumanTraitPartsPrefabs;
    public List<GameObject> AlienOnlyPartsPrefabs;
}

public class AlienBodyPartCollections : MonoPersistentMikroSingleton<AlienBodyPartCollections> {
    public BodyPartCollection HeadBodyPartPrefabs;
    public BodyPartCollection MainBodyPartPrefabs;
    public BodyPartCollection LegBodyPartPrefabs;


    private void Start() {
        MainGame.Interface.GetUtility<ResLoader>();
    }

    public AlienBodyPartInfo GetRandomBodyPartInfo(BodyPartType bodyPartType, bool isAlien) {
        switch (bodyPartType) {
            case BodyPartType.Body:
                return GetRandomBodyPartInfo(MainBodyPartPrefabs, isAlien);
            case BodyPartType.Head:
                return GetRandomBodyPartInfo(HeadBodyPartPrefabs, isAlien);
            case BodyPartType.Legs:
                return GetRandomBodyPartInfo(LegBodyPartPrefabs, isAlien);
        }

        return null;
    }

    private AlienBodyPartInfo GetRandomBodyPartInfo(BodyPartCollection targetCollection, bool isAlien) {
        List<GameObject> targetList = new List<GameObject>(targetCollection.HumanTraitPartsPrefabs);
        if (isAlien) {
            targetList.AddRange(targetCollection.AlienOnlyPartsPrefabs);
        }

        return targetList[Random.Range(0, targetList.Count)].GetComponent<AlienBodyPartInfo>();
    }
}
