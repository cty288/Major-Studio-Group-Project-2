using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameEvents.Merchant;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowMerchantPosterViewController : AbstractMikroController<MainGame>, IPointerClickHandler {
    [ES3Serializable] private string posterID;
    private GameObject hintCanvas;

    private void Awake() {
        hintCanvas = transform.Find("HintCanvas").gameObject;
        this.RegisterEvent<OnMerchantAdEventGenerated>(OnMerchantAdEventGenerated)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        gameObject.SetActive(false);
    }

    private void OnMerchantAdEventGenerated(OnMerchantAdEventGenerated e) {
        posterID = e.posterID;
        gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData) {
        this.Delay(0.1f, () => {
            if (this) {
                this.SendCommand<OpenPosterUIPanelCommand>(new OpenPosterUIPanelCommand(
                    posterID));
                hintCanvas.SetActive(false);
            }
        });
    }
}
