using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Singletons;
using UnityEngine;
using UnityEngine.UI;

public class LoadCanvas : MonoMikroSingleton<LoadCanvas>, IController {
    private GameTimeManager gameTimeManager;
    private Image bg;
    private void Awake() {
        gameTimeManager = this.GetSystem<GameTimeManager>();
        bg = transform.Find("BG").GetComponent<Image>();
    }

    public void Load(float loadTime, Action onScreenBlack, Action onScreenRecover) {
        gameTimeManager.LockTime.Retain();
        bg.DOFade(1, 0.5f).OnComplete(() => {
            onScreenBlack?.Invoke();
            bg.DOFade(0, 0.5f).OnComplete(() => {
                onScreenRecover?.Invoke();
                gameTimeManager.LockTime.Release();
            }).SetDelay(loadTime);
        });
    }

    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
