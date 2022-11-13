using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.ResKit;
using UnityEngine;

public class AlienBody : AbstractMikroController<MainGame> {

    public BodyInfo BodyInfo;
    public static GameObject BuildAlienBody(BodyInfo info) {
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
}
