using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public class HomeOutsideFoot : AbstractMikroController<MainGame> {
    private BodyGenerationSystem bodyGenerationSystem;
    private SpriteRenderer sprite;
    private void Awake() {
        bodyGenerationSystem = this.GetSystem<BodyGenerationSystem>();
        bodyGenerationSystem.CurrentOutsideBody.RegisterOnValueChaned(OnOutsideBodyChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    private void OnOutsideBodyChanged(BodyInfo arg1, BodyInfo body) {
        
        if (body != null) {
            sprite.DOFade(1, 0.5f);
        }
        else {
            sprite.DOFade(0, 0.5f);
        }
    }
}
