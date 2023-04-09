using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.GameEvents.Merchant;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MerchantNoteUIPanel : OpenableUIPanel {
    private List<Image> images;
    private List<TMP_Text> texts;
    private GameObject panel;
    private TMP_Text phoneNumberText;
    private MerchantModel merchantModel;
    
    private Dictionary<Image, float> imageAlpha = new Dictionary<Image, float>();
    private TelephoneNumberRecordModel telephoneNumberRecordModel;

    protected override void Awake() {
        base.Awake();
        this.RegisterEvent<OnMerchantNoteUIPanelOpened>(OnMerchantNoteUIPanelOpened).UnRegisterWhenGameObjectDestroyed(gameObject);
        images = GetComponentsInChildren<Image>(true).ToList();
        texts = GetComponentsInChildren<TMP_Text>(true).ToList();
        merchantModel = this.GetModel<MerchantModel>();
        phoneNumberText = transform.Find("Panel/Paper/PhoneNumberText").GetComponent<TMP_Text>();
        panel = transform.Find("Panel").gameObject;
        telephoneNumberRecordModel = this.GetModel<TelephoneNumberRecordModel>();

        foreach (Image image in images) {
            imageAlpha.Add(image, image.color.a);
        }
        Hide(0.5f);
    }

    private void OnMerchantNoteUIPanelOpened(OnMerchantNoteUIPanelOpened e) {
        if (!telephoneNumberRecordModel.ContainsRecord(merchantModel.PhoneNumber)) {
            telephoneNumberRecordModel.AddOrEditRecord(merchantModel.PhoneNumber, "Number on \"I have everything you want\"");
        }
       
        Show(0.5f);
    }

    public override void OnShow(float time) {
        phoneNumberText.text = merchantModel.PhoneNumber;
        panel.SetActive(true);
        images.ForEach((image => image.DOFade( imageAlpha.ContainsKey(image)? imageAlpha[image]: 1 , time)));
        texts.ForEach((text => text.DOFade(1, time)));
    }

    public override void OnHide(float time) {
        images.ForEach((image => image.DOFade(0, time)));
        texts.ForEach((text => text.DOFade(0, time)));
        this.Delay(time, () => {
            if (this) {
                {
                    panel.SetActive(false);
                }
            }
        });
    }

    public override void OnDayEnd() {
        Hide(0.5f);
    }

   
}
