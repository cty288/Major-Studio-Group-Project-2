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
