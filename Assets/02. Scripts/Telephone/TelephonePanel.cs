using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TelephonePanel : OpenableUIPanel {
    private GameObject panel;
    private Button backButton;
    private BountyHunterSystem bountyHunterSystem;
    private TelephoneSystem telephoneSystem;
    private EventTrigger phonecallEventTrigger;
    private TMP_Text pickupText;
    protected override void Awake() {
        base.Awake();
        panel = transform.Find("Panel").gameObject;
        backButton = panel.transform.Find("BackButton").GetComponent<Button>();
        backButton.onClick.AddListener(OnBackButtonClicked);
        telephoneSystem = this.GetSystem<TelephoneSystem>();
        bountyHunterSystem = this.GetSystem<BountyHunterSystem>();
        bountyHunterSystem.IsBountyHunting.RegisterOnValueChaned(OnBountyHuntingChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
        telephoneSystem.State.RegisterOnValueChaned(OnTelephoneSystemStateChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
        phonecallEventTrigger = panel.transform.Find("Phone").GetComponent<EventTrigger>();
        pickupText = phonecallEventTrigger.transform.Find("Text").GetComponent<TMP_Text>();
        phonecallEventTrigger.GetComponent<Image>().alphaHitTestMinimumThreshold = 1;
        phonecallEventTrigger.triggers[0].callback.AddListener(OnPhoneCallClicked);
    }

    private void OnPhoneCallClicked(BaseEventData e) {
        if (telephoneSystem.CurrentIncomingCallContact != null) {
            telephoneSystem.ReceiveIncomingCall();
        }
    }

    private void Update() {
        if (telephoneSystem.CurrentIncomingCallContact == null) {
            pickupText.text = "";
        }
        else {
            pickupText.text = "Pick Up";
        }
    }

    private void OnBountyHuntingChanged(bool isHunting) {
        if (isHunting) {
            backButton.gameObject.SetActive(true);
            Hide();            
        }
    }

    private void OnTelephoneSystemStateChanged(TelephoneState state) {
        if (state == TelephoneState.Idle || state == TelephoneState.Dealing || state == TelephoneState.IncomingCall) {
            backButton.gameObject.SetActive(true);
        }else{
        {
           // backButton.gameObject.SetActive(false);
        }}
    }

    private void OnBackButtonClicked() {
        Hide();
    }

    public override void Show() {
        panel.gameObject.SetActive(true);
    }

    public override void Hide() {
        panel.gameObject.GetComponent<Animator>().CrossFade("Stop", 0.5f);
        this.Delay(0.5f, () => {
            panel.gameObject.SetActive(false);
        });
    }

    public override void OnDayEnd() {
       Hide();
    }
}
