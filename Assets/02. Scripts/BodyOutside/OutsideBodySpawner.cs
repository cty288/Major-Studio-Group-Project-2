using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using UnityEngine;
using Random = UnityEngine.Random;

public class OutsideBodySpawner : AbstractMikroController<MainGame>, ICanSendEvent {
    private AlienBody bodyViewController = null;
    private BodyGenerationModel bodyGenerationModel;
    private BodyGenerationSystem bodyGenerationSystem;
    private PlayerResourceSystem playerResourceSystem;
    private BodyManagmentSystem bodyManagmentSystem;
  

    //private bool speakEnd = false;
    private bool todaysAlienIsVeryLarge = false;
    private void Awake() {
        bodyGenerationModel = this.GetModel<BodyGenerationModel>();
        bodyGenerationSystem = this.GetSystem<BodyGenerationSystem>();
        playerResourceSystem = this.GetSystem<PlayerResourceSystem>();
        bodyManagmentSystem = this.GetSystem<BodyManagmentSystem>();
        bodyGenerationModel.CurrentOutsideBody.RegisterOnValueChaned(OnOutsideBodyChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        this.GetSystem<GameTimeManager>().OnDayStart += OnDayStart;
    }

    private void OnDayStart(int obj) {
        todaysAlienIsVeryLarge = Random.value < 0.1f;
    }

    private void OnOutsideBodyChanged(BodyInfo oldBody, BodyInfo body) {
        if (body == null) {
            if (bodyViewController) {
                bodyViewController.Hide();
                bodyViewController.onClickAlienBody -= OnOutsideBodyClicked;
                this.Delay(0.5f, () => {
                    Destroy(bodyViewController.gameObject);
                    bodyViewController = null;
                });
            }
            else {
                bodyViewController = null;
            }
        }
        else {
            
            if(String.IsNullOrEmpty(body.BuiltBodyOverriddenPrefabName)){
                bodyViewController = AlienBody.BuildShadowBody(body, true).GetComponent<AlienBody>();
            }
            else {
                bodyViewController = AlienBody.BuildShadowBody(body, true, body.BuiltBodyOverriddenPrefabName).GetComponent<AlienBody>();
            }
            
            bodyViewController.transform.position = transform.position;
            
            if (body.IsAlien) {
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
        
        BackButton.Singleton.Hide();
        
        this.SendEvent<OnOutsideBodyClicked>(new OnOutsideBodyClicked() {BodyInfo = bodyViewController.BodyInfo});
        
    }

 
}


public struct OnShowFood {

}

public struct OnOutsideBodyClicked {
    public BodyInfo BodyInfo;
}
