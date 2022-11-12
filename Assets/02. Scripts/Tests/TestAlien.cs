using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using UnityEngine;

public class TestAlien : AbstractMikroController<MainGame> {
    private void Start() {
        Debug.Log(AlienDescriptionFactory.GetRadioDescription(null, false));
        Debug.Log(AlienDescriptionFactory.GetRadioDescription(null, true));
        this.Delay(1f, () => {
            AlienBody.BuildAlienBody(AlienBodyInfo.GetRandomAlienInfo());
        });
    }
}
