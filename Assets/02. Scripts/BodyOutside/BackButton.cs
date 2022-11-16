using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoMikroSingleton<BackButton> {
    private Button button;
    private Image image;

    [SerializeField] private List<Camera> sceneCameras;
    [SerializeField] private Camera mainCamera;

    private TMP_Text hintText;
    private void Awake() {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        hintText = GetComponentInChildren<TMP_Text>();
        button.onClick.AddListener(OnBackButtonClicked);
        button.interactable = false;
        image.enabled = false;
        image.color = new Color(1, 1, 1, 0);
        hintText.color = new Color(1, 1, 1, 0);
    }

    public void Show() {
        button.interactable = true;
        image.enabled = true;
        image.DOFade(1, 0.5f);
        hintText.DOFade(1, 0.5f);
    }

    public void Hide() {
        button.interactable = false;
        //image.enabled = false;
        image.DOFade(0, 0.5f).OnComplete(() =>
        {
            image.enabled = false;
        });
        hintText.DOFade(0, 0.5f);
    }
    public void OnBackButtonClicked() {
        LoadCanvas.Singleton.Load(0.2f, () => {
            foreach (Camera sceneCamera in sceneCameras) {
                sceneCamera.gameObject.SetActive(false);
            }
            mainCamera.gameObject.SetActive(true);
            Hide();
        }, null);
    }

    
}
