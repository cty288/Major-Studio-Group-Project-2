using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OutsideFlashlightViewController : AbstractMikroController<MainGame> {
    private Light2D light;
    private void Awake() {
        light = GetComponent<Light2D>();
        this.RegisterEvent<OnFlashlightFlash>(OnFlashLightFlash).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnFlashLightFlash(OnFlashlightFlash e) {
        DOTween.To(() => light.intensity, value => light.intensity = value, 30, 0.1f);
        DOTween.To(() => light.intensity, value => light.intensity = value, 0, 0.1f).SetDelay(0.2f);
    }
}
