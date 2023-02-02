using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotebookPagePanel : OpenableUIPanel {
   
    protected TMP_Text pageText;
    private GameObject panel;

   

    public override void OnDayEnd()
    {
        Hide(0.5f);
    }
    
    private void OnBackButtonClicked() {
        Hide(0.5f);
    }    

    protected override void Awake() {
        base.Awake();
        panel = transform.Find("Panel").gameObject;

        pageText = transform.Find("Panel/NotebookWritePage/Text").GetComponent<TMP_Text>();
        this.RegisterEvent<OnNotePanelOpened>(OnNotePanelOpened).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnNotePanelOpened(OnNotePanelOpened e) {
        SetContent(e.Content);
        Show(0.5f);
    }


    private string content;
    public void SetContent(string content) {
        this.content = content;
    }
    
    public override void OnShow(float time) {
        panel.gameObject.SetActive(true);
        pageText.text = content;
    }

    public override void OnHide(float time) {
        panel.gameObject.GetComponent<Animator>().CrossFade("Stop", time);
        this.Delay(time, () => {
            panel.gameObject.SetActive(false);
        });
    }
}
