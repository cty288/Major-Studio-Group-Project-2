using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using Random = UnityEngine.Random;

public class OutsideBodySpawner : AbstractMikroController<MainGame>, ICanSendEvent {
    private AlienBody bodyViewController = null;
    private BodyGenerationModel bodyGenerationModel;
    private BodyGenerationSystem bodyGenerationSystem;
    [SerializeField] private Speaker speaker;

    private bool speakEnd = false;
    private bool todaysAlienIsVeryLarge = false;
    private void Awake() {
        bodyGenerationModel = this.GetModel<BodyGenerationModel>();
        bodyGenerationSystem = this.GetSystem<BodyGenerationSystem>();
        bodyGenerationModel.CurrentOutsideBody.RegisterOnValueChaned(OnOutsideBodyChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        this.GetSystem<GameTimeManager>().OnDayStart += OnDayStart;
    }

    private void OnDayStart(int obj) {
        todaysAlienIsVeryLarge = Random.value < 0.1f;
    }

    private void OnOutsideBodyChanged(BodyInfo oldBody, BodyInfo body) {
        if (body == null) {
            bodyViewController.Hide();
            bodyViewController.onClickAlienBody -= OnOutsideBodyClicked;

            this.Delay(0.5f, () => {
                Destroy(bodyViewController.gameObject);
                bodyViewController = null;
            });
        }
        else {
            bodyViewController = AlienBody.BuildShadowAlienBody(body, true).GetComponent<AlienBody>();
            bodyViewController.transform.position = transform.position;
            if (body == bodyGenerationSystem.TodayAlien) {
                if (todaysAlienIsVeryLarge) {
                    bodyViewController.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    bodyViewController.gameObject.transform.DOLocalMoveY(-4, 0f);
                }
            }
            bodyViewController.Show();
            bodyViewController.onClickAlienBody += OnOutsideBodyClicked;
        }
    }

    private void OnOutsideBodyClicked() {
        if (bodyGenerationModel.CurrentOutsideBodyConversationFinishing) {
            return;
        }
        bodyGenerationModel.CurrentOutsideBodyConversationFinishing = true;

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
                this.GetSystem<PlayerResourceSystem>().AddFood(Random.Range(1, 3));
                
            }, () => {

            }, () => speakEnd);
        }

    }

    private void OnAlienSpeakEnd() {
        DieCanvas.Singleton.Show("You are killed by the creature!");
        this.GetSystem<BodyGenerationSystem>().StopCurrentBody();
    }

    private void OnSpeakEnd() {
        this.SendEvent<OnShowFood>();
        this.Delay(4.5f, () => {
            speakEnd = true;
            BackButton.Singleton.OnBackButtonClicked();
            this.GetSystem<BodyGenerationSystem>().StopCurrentBody();
        });
        
    }
}


public struct OnShowFood {

}
