using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public class OutsideBodySpawner : AbstractMikroController<MainGame> {
    private void Awake() {
        this.GetSystem<BodyGenerationSystem>().CurrentOutsideBody.RegisterOnValueChaned(OnOutsideBodyChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnOutsideBodyChanged(BodyInfo oldBody, BodyInfo body) {
        
    }
}
