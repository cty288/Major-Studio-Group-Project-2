using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Prologue;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.UI;

public class IntroSceneController : AbstractMikroController<MainGame> {
    private GameObject panel;
    private Image image;
    [SerializeField] private Sprite[] sprites;
    private void Awake() {
        panel = transform.Find("Panel").gameObject;
        image = panel.transform.Find("Image").GetComponent<Image>();
        
        this.RegisterEvent<OnIntroSceneHide>(OnIntroSceneHide).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnIntroSceneShowImage>(OnIntroSceneShowImage).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnIntroSceneShowImage(OnIntroSceneShowImage e) {
        ShowImage(e.ImageIndex);
    }

    private void OnIntroSceneHide(OnIntroSceneHide obj) {
        Hide();
    }


    private void ShowImage(int index) {
        panel.SetActive(true);
        image.DOFade(0, 0.5f).OnComplete(() => {
            image.sprite = sprites[index];
            image.DOFade(1, 0.5f);
        });
    }

    private void Hide() {
        image.DOFade(0, 0.5f).OnComplete(() => {
            panel.SetActive(false);
        });
    }
}
