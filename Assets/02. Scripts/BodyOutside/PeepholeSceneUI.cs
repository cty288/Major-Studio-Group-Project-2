using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PeepholeSceneUI : AbstractMikroController<MainGame> {
    private Button lightButton;
    public Action OnLightButtonPressed = () => { };
    private GameSceneModel gameSceneModel;
    private ElectricitySystem electricitySystem;
    private TMP_Text indicateText;
    private GameObject panelObj;
    private void Awake() {
        gameSceneModel = this.GetModel<GameSceneModel>();
        panelObj = transform.Find("Panel").gameObject;
        lightButton = transform.Find("Panel/Button").GetComponent<Button>();
        lightButton.onClick.AddListener(OnLightPressed);
        lightButton.GetComponent<Image>().alphaHitTestMinimumThreshold = 1;
        gameSceneModel.GameScene.RegisterWithInitValue(OnGameSceneChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        electricitySystem = this.GetSystem<ElectricitySystem>();
        indicateText = panelObj.transform.Find("IndicateText").GetComponent<TMP_Text>();
    }

    private void OnGameSceneChanged(GameScene scene) {
        this.Delay(0.5f, () => {
            if (scene == GameScene.Peephole)
            {
                panelObj.SetActive(true);
            }
            else
            {
                panelObj.SetActive(false);
            }
        });
    }

    private void Update() {
        if (electricitySystem.Electricity.Value < 0.9f) {
            indicateText.text = "Not enough electricity";
        }
        else {
            indicateText.text = "";
        }
    }

    private void OnLightPressed() {
        OnLightButtonPressed?.Invoke();
    }
}
