using System;
using System.Collections;
using System.Collections.Generic;
using Mikrocosmos;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public class ImageEffectOpener : AbstractMikroController<MainGame>
{
    private void Awake() {
        this.GetModel<GameSceneModel>().GameScene.RegisterOnValueChaned(OnGameSceneChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnNewDay(OnNewDay e) {
        if (e.Day == 0) {
            ImageEffectController.Singleton.TurnOnScriptableRendererFeature(1);
            ImageEffectController.Singleton.TurnOnScriptableRendererFeature(2);
        }
        else {
            ImageEffectController.Singleton.TurnOffScriptableRendererFeature(1);
            ImageEffectController.Singleton.TurnOffScriptableRendererFeature(2);
        }
    }

    private void OnGameSceneChanged(GameScene scene) {
        this.Delay(0.5f, () => {
            if (scene == GameScene.Peephole)
            {
                ImageEffectController.Singleton.TurnOnScriptableRendererFeature(0);
            }
            else
            {
                ImageEffectController.Singleton.TurnOffScriptableRendererFeature(0);
            }
        });
      
    }
}
