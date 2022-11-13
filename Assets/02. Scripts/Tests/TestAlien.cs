using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using UnityEngine;

public class TestAlien : AbstractMikroController<MainGame> {
    private void Start() {
      //  Debug.Log(AlienDescriptionFactory.GetRadioDescription(BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, false), 1));
        //Debug.Log(AlienDescriptionFactory.GetRadioDescription(BodyInfo.GetRandomBodyInfo( BodyPartDisplayType.Shadow, true), 0.5f));

        
        this.Delay(1f, () => {
            BodyInfo info1 = BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, false);
            GameObject o1 = AlienBody.BuildShadowAlienBody(info1);
            o1.transform.position = new Vector3(0, 0, 0);

            BodyInfo info2 = BodyInfo.GetBodyInfoOpposite(info1, 0.5f, true);
            GameObject o2 = AlienBody.BuildShadowAlienBody(info2);
            o2.transform.position = new Vector3(10, 0, 0);

        });
    }
}
