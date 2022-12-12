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
using UnityEngine.UI;

public class MerchantNoteUIPanel : OpenableUIPanel {
    private List<Image> images;
    private List<TMP_Text> texts;
    private GameObject panel;
    private TMP_Text phoneNumberText;
    private MerchantSystem merchantSystem;
    private Button backButton;
    protected override void Awake() {
        base.Awake();
        this.RegisterEvent<OnMerchantNoteUIPanelOpened>(OnMerchantNoteUIPanelOpened).UnRegisterWhenGameObjectDestroyed(gameObject);
        images = GetComponentsInChildren<Image>(true).ToList();
        texts = GetComponentsInChildren<TMP_Text>(true).ToList();
        merchantSystem = this.GetSystem<MerchantSystem>();
        phoneNumberText = transform.Find("Panel/Paper/PhoneNumberText").GetComponent<TMP_Text>();
        panel = transform.Find("Panel").gameObject;
        backButton = panel.transform.Find("BackButton").GetComponent<Button>();
        backButton.onClick.AddListener(Hide);
        Hide();
    }

    private void OnMerchantNoteUIPanelOpened(OnMerchantNoteUIPanelOpened e) {
        Show();
    }

    public override void Show() {
        phoneNumberText.text = merchantSystem.PhoneNumber;
        panel.SetActive(true);
        images.ForEach((image => image.DOFade(1, 0.5f)));
        texts.ForEach((text => text.DOFade(1, 0.5f)));
    }

    public override void Hide() {
        images.ForEach((image => image.DOFade(0, 0.5f)));
        texts.ForEach((text => text.DOFade(0, 0.5f)));
        this.Delay(0.5f, () => {
            if (this) {
                {
                    panel.SetActive(false);
                }
            }
        });
    }

    public override void OnDayEnd() {
        Hide();
    }

   
}
