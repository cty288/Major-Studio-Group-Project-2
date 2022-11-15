using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public class OutsideBodySpawner : AbstractMikroController<MainGame> {
    private AlienBody bodyViewController = null;
    private void Awake() {
        this.GetSystem<BodyGenerationSystem>().CurrentOutsideBody.RegisterOnValueChaned(OnOutsideBodyChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnOutsideBodyChanged(BodyInfo oldBody, BodyInfo body) {
        if (body == null) {
            bodyViewController.Hide();
            this.Delay(0.5f, () => {
                Destroy(bodyViewController.gameObject);
                bodyViewController = null;
            });
        }
        else {
            bodyViewController = AlienBody.BuildShadowAlienBody(body, true).GetComponent<AlienBody>();
            bodyViewController.transform.position = transform.position;
            bodyViewController.Show();
        }
    }
}
