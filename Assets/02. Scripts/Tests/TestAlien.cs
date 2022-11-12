using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using UnityEngine;

public class TestAlien : AbstractMikroController<MainGame> {
    private void Start() {
        Debug.Log(AlienDescriptionFactory.GetRadioDescription(BodyInfo.GetRandomBodyInfo(false), 1));
        Debug.Log(AlienDescriptionFactory.GetRadioDescription(BodyInfo.GetRandomBodyInfo(true), 0.5f));
        this.Delay(1f, () => {
            AlienBody.BuildAlienBody(BodyInfo.GetRandomBodyInfo(true));
        });
    }
}
