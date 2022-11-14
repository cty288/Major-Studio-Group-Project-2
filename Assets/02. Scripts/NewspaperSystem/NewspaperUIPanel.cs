using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.UI;

public class NewspaperUIPanel : AbstractMikroController<MainGame> {
    private GameObject panel;

    private Button backButton;

    private void Awake() {
        panel = transform.Find("Panel").gameObject;
        backButton = panel.transform.Find("BackButton").GetComponent<Button>();
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
        panel.gameObject.SetActive(true);
    }

    public void Hide() {
        panel.gameObject.GetComponent<Animator>().CrossFade("Stop", 0.5f);
        this.Delay(0.5f, () => {
            panel.gameObject.SetActive(false);
        });
    }
}
