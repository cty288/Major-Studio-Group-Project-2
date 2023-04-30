using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TelephonePanel : OpenableUIPanel {
    private GameObject panel;
    private List<MaskableGraphic> images;
    private List<TMP_Text> texts;
    private Dictionary<MaskableGraphic, float> imageAlpha = new Dictionary<MaskableGraphic, float>();
    private TelephoneNotepadUIPanel notepadUIPanel;
    private BountyHunterSystem bountyHunterSystem;
    private TelephoneSystem telephoneSystem;
    private EventTrigger phonecallEventTrigger;
    private TMP_Text pickupText;
    private PlayerControlModel playerControlModel;

    protected override void Awake() {
        base.Awake();
        panel = transform.Find("Panel").gameObject;
        images = GetComponentsInChildren<MaskableGraphic>(true).ToList();
        texts = GetComponentsInChildren<TMP_Text>(true).ToList();
        foreach (var image in images) {
            imageAlpha.Add(image, image.color.a);
        }
        telephoneSystem = this.GetSystem<TelephoneSystem>();
        bountyHunterSystem = this.GetSystem<BountyHunterSystem>();
        playerControlModel = this.GetModel<PlayerControlModel>();
        
        this.RegisterEvent<OnAddControlType>(OnAddControlType).UnRegisterWhenGameObjectDestroyed(gameObject);
        
       
        // bountyHunterSystem.IsBountyHunting.RegisterOnValueChaned(OnBountyHuntingChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
        telephoneSystem.State.RegisterOnValueChaned(OnTelephoneSystemStateChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        phonecallEventTrigger = panel.transform.Find("Phone").GetComponent<EventTrigger>();
        pickupText = phonecallEventTrigger.transform.Find("Text").GetComponent<TMP_Text>();
        phonecallEventTrigger.GetComponent<Image>().alphaHitTestMinimumThreshold = 1;
        phonecallEventTrigger.triggers[0].callback.AddListener(OnPhoneCallClicked);
        notepadUIPanel = GetComponentInChildren<TelephoneNotepadUIPanel>(true);
        notepadUIPanel.Init();
        Hide(0.5f);
    }

    private void OnAddControlType(OnAddControlType e) {
        if (e.controlType == PlayerControlType.BountyHunting) {
            Hide(0.5f);
        }
    }

    
    private void OnPhoneCallClicked(BaseEventData e) {
        if (telephoneSystem.CurrentIncomingCallContact != null) {
            telephoneSystem.ReceiveIncomingCall();
        }
    }

    protected override void Update() {
        base.Update();
        if (telephoneSystem.CurrentIncomingCallContact == null) {
            pickupText.text = "";
        }
        else {
            pickupText.text = "Pick Up";
        }
    }


    private void OnTelephoneSystemStateChanged(TelephoneState state) {
        if (state == TelephoneState.Idle || state == TelephoneState.Dealing || state == TelephoneState.IncomingCall) {
            // backButton.gameObject.SetActive(true);
        }
        else {
            {
                // backButton.gameObject.SetActive(false);
            }
        }
    }

    private void OnBackButtonClicked() {
        Hide(0.5f);
    }

    public override void OnShow(float time) {
        notepadUIPanel.OnPanelOpened();
        images = GetComponentsInChildren<MaskableGraphic>(true).ToList();
        texts = GetComponentsInChildren<TMP_Text>(true).ToList();
        panel.gameObject.SetActive(true);
        images.ForEach((image => {
            if(imageAlpha.ContainsKey(image)) {
                image.DOFade(imageAlpha[image], time);
            }
            else {
                image.DOFade(1, time);
            }
			
        }));
        texts.ForEach((text => {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            text.DOFade(1, time);
        }));
    }

    public override void OnHide(float time) {
        images = GetComponentsInChildren<MaskableGraphic>(true).ToList();
        texts = GetComponentsInChildren<TMP_Text>(true).ToList();
        
        images.ForEach((image => image.DOFade(0, time)));
        texts.ForEach((text => text.DOFade(0, time)));
		
		
        this.Delay(time, () => {
            if (this) { {
                    notepadUIPanel.OnPanelOpened();
                    panel.gameObject.SetActive(false);
                }
            }
        });
    }

    public override void OnDayEnd() {
        Hide(0.5f);
    }

    public override bool AdditionMouseExitCheck() {
        return !playerControlModel.HasControlType(PlayerControlType.Choosing);
    }
}
