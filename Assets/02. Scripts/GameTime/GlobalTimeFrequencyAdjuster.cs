using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using UnityEngine;

public class GlobalTimeFrequencyAdjuster : AbstractMikroController<MainGame> {
    [SerializeField] protected AnimationCurve timeCurve;

    private void Awake() {
        this.GetModel<GameTimeModel>().GlobalTimeFreqCurve = timeCurve;
    }
}
