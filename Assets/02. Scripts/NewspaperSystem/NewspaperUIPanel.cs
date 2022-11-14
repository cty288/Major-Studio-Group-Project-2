using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewspaperUIPanel : AbstractMikroController<MainGame> {
    public Newspaper newspaper;
    private GameObject panel;
    private TMP_Text dateText;

    private Button backButton;

    private void Awake() {
        panel = transform.Find("Panel").gameObject;
        backButton = panel.transform.Find("BackButton").GetComponent<Button>();
        dateText = panel.transform.Find("NewspaperBG/DateText").GetComponent<TMP_Text>();
        backButton.onClick.AddListener(OnBackButtonClicked);
        this.RegisterEvent<OnNewspaperUIPanelOpened>(OnNewspaperUIPanelOpened)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnNewspaperUIPanelOpened(OnNewspaperUIPanelOpened e) {
        if (e.IsOpen) {
            Show(e.Newspaper);
        }
        else {
            Hide();
        }
    }

    private void OnBackButtonClicked() {
        Hide();
    }    

    public void Show(Newspaper news) {
        dateText.text = news.dateString;
        panel.gameObject.SetActive(true);
    }

    public void Hide() {
        panel.gameObject.GetComponent<Animator>().CrossFade("Stop", 0.5f);
        this.Delay(0.5f, () => {
            panel.gameObject.SetActive(false);
        });
    }
}
