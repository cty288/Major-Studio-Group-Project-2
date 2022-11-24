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
    private Button backButton;
   

    public override void OnDayEnd()
    {
        Hide();
    }
    
    private void OnBackButtonClicked() {
        Hide();
    }    

    protected override void Awake() {
        base.Awake();
        panel = transform.Find("Panel").gameObject;
        backButton = panel.transform.Find("BackButton").GetComponent<Button>();
        backButton.onClick.AddListener(OnBackButtonClicked);
        pageText = transform.Find("Panel/NotebookWritePage/Text").GetComponent<TMP_Text>();
        this.RegisterEvent<OnNotePanelOpened>(OnNotePanelOpened).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnNotePanelOpened(OnNotePanelOpened e) {
        SetContent(e.Content);
        Show();
    }


    private string content;
    public void SetContent(string content) {
        this.content = content;
    }
    
    public override void Show() {
        panel.gameObject.SetActive(true);
        pageText.text = content;
    }

    public override void Hide() {
        panel.gameObject.GetComponent<Animator>().CrossFade("Stop", 0.5f);
        this.Delay(0.5f, () => {
            panel.gameObject.SetActive(false);
        });
    }
}
