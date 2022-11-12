using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ResKit;
using MikroFramework.Singletons;
using UnityEngine;
using Random = UnityEngine.Random;

public class AlienBodyPartCollections : MonoPersistentMikroSingleton<AlienBodyPartCollections> {
    public  List<GameObject> HeadBodyPartPrefabs = null;
    public  List<GameObject> MainBodyPartPrefabs = null;
    public  List<GameObject> LegBodyPartPrefabs = null;


    private void Start() {
        MainGame.Interface.GetUtility<ResLoader>();
    }

    public AlienBodyPartInfo GetRandomBodyPartInfo(BodyPartType bodyPartType) {
        switch (bodyPartType) {
            case BodyPartType.Body:
                return MainBodyPartPrefabs[Random.Range(0, MainBodyPartPrefabs.Count)]
                    .GetComponent<AlienBodyPartInfo>();
            case BodyPartType.Head:
                return HeadBodyPartPrefabs[Random.Range(0, HeadBodyPartPrefabs.Count)]
                    .GetComponent<AlienBodyPartInfo>();
            case BodyPartType.Legs:
                return LegBodyPartPrefabs[Random.Range(0, LegBodyPartPrefabs.Count)]
                    .GetComponent<AlienBodyPartInfo>();
        }

        return null;
    }
}
