using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.ResKit;
using UnityEngine;

public class AlienBody : AbstractMikroController<MainGame> {

    public BodyInfo BodyInfo;

    private Transform tallSpawnPosition;
    private Transform shortSpawnPosition;

    private void Awake() {
        tallSpawnPosition = transform.Find("TallSpawnPosition");
        shortSpawnPosition = transform.Find("LowSpawnPosition");
    }

    public static GameObject BuildShadowAlienBody(BodyInfo info) {
        ResLoader resLoader = MainGame.Interface.GetUtility<ResLoader>();
        GameObject body = resLoader.LoadSync<GameObject>("aliens","AlienBody");
        GameObject bodyInstance = GameObject.Instantiate(body);
        AlienBodyPartInfo lastInfo = null;
        foreach (AlienBodyPartInfo partInfo in info.AllBodyInfoPrefabs) {
            Vector2 position = Vector2.zero;
            if (lastInfo != null) {
                position = lastInfo.JointPoint.position;
            }

            if (partInfo) {
                GameObject spawnedBodyPart = Instantiate(partInfo.gameObject, position, Quaternion.identity, bodyInstance.transform);
                lastInfo = spawnedBodyPart.GetComponent<AlienBodyPartInfo>();
            }
        }

        return bodyInstance;
    }

    public static GameObject BuildNewspaperAlienBody(BodyInfo info) {
        ResLoader resLoader = MainGame.Interface.GetUtility<ResLoader>();
        GameObject body = resLoader.LoadSync<GameObject>("aliens", "NewspaperFrame");
        GameObject bodyInstance = GameObject.Instantiate(body);
        AlienBody alienBody = bodyInstance.GetComponent<AlienBody>();
        
        AlienBodyPartInfo lastInfo = null;

        int layer = 1;
        foreach (AlienBodyPartInfo partInfo in info.AllBodyInfoPrefabs) {
            Vector2 position = Vector2.zero;
            if (lastInfo != null) {
                position = lastInfo.JointPoint.position;
            }else {
                if (info.Height == HeightType.Short) {
                    position = alienBody.shortSpawnPosition.position;
                }else {
                    position = alienBody.tallSpawnPosition.position;
                }
            }

            if (partInfo)
            {
                GameObject spawnedBodyPart = Instantiate(partInfo.gameObject, position, Quaternion.identity, bodyInstance.transform);
                spawnedBodyPart.GetComponent<SpriteRenderer>().sortingOrder = layer;
                lastInfo = spawnedBodyPart.GetComponent<AlienBodyPartInfo>();
            }
            layer++;
        }

        return bodyInstance;
    }
}
