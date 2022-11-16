using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using Random = UnityEngine.Random;

public class OutsideBodySpawner : AbstractMikroController<MainGame>, ICanSendEvent {
    private AlienBody bodyViewController = null;
    private BodyGenerationSystem bodyGenerationSystem;
    [SerializeField] private Speaker speaker;

    private bool speakEnd = false;
    private void Awake() {
        bodyGenerationSystem = this.GetSystem<BodyGenerationSystem>();
        bodyGenerationSystem.CurrentOutsideBody.RegisterOnValueChaned(OnOutsideBodyChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnOutsideBodyChanged(BodyInfo oldBody, BodyInfo body) {
        if (body == null) {
            bodyViewController.Hide();
            this.Delay(0.5f, () => {
                bodyViewController.onClickAlienBody -= OnOutsideBodyClicked;
                
                Destroy(bodyViewController.gameObject);
                bodyViewController = null;
            });
        }
        else {
            bodyViewController = AlienBody.BuildShadowAlienBody(body, true).GetComponent<AlienBody>();
            bodyViewController.transform.position = transform.position;
            bodyViewController.Show();
            bodyViewController.onClickAlienBody += OnOutsideBodyClicked;
        }
    }

    private void OnOutsideBodyClicked() {
        Debug.Log("Clicked");
        if (bodyViewController.BodyInfo == bodyGenerationSystem.TodayAlien) {
            BackButton.Singleton.Hide();
            LoadCanvas.Singleton.LoadUntil(() => {
                speaker.Speak("Hahaha! I will kill you!", OnAlienSpeakEnd);
            }, () => {
                
            }, () => false);
        }
        else {
            BackButton.Singleton.Hide();
            LoadCanvas.Singleton.LoadUntil(() => {
                speaker.Speak("Hey, I brought you some foods! Take care!", OnSpeakEnd);
                this.GetSystem<PlayerResourceSystem>().AddFood(Random.Range(2, 4));
                this.GetSystem<BodyGenerationSystem>().StopCurrentBodyAndStartNew();
            }, () => {

            }, () => speakEnd);
        }

    }

    private void OnAlienSpeakEnd() {
        DieCanvas.Singleton.Show("You are killed by the creature!");
    }

    private void OnSpeakEnd() {
        this.SendEvent<OnShowFood>();
        this.Delay(4.5f, () => {
            speakEnd = true;
            BackButton.Singleton.OnBackButtonClicked();
        });
        
    }
}


public struct OnShowFood {

}
