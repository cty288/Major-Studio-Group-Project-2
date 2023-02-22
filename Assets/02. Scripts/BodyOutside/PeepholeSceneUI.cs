using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PeepholeSceneUI : AbstractMikroController<MainGame> {
    
    //protected List<Collider2D> colliders = new List<Collider2D>();
    private Button lightButton;
    public Action OnLightButtonPressed = () => { };
    private GameSceneModel gameSceneModel;
    private ElectricitySystem electricitySystem;
    private TMP_Text indicateText;
    private GameObject panelObj;
    public OutdoorFlashLight flashlight;
    
    private List<Collider2D> selfColliders;
    
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
        selfColliders = GetComponents<Collider2D>().ToList();
        //colliders = GetComponents<Collider2D>().ToList();
    }

    private void OnGameSceneChanged(GameScene scene) {
        this.Delay(0.5f, () => {
            if (scene == GameScene.Peephole) {
                panelObj.SetActive(true);
                selfColliders.ForEach(c => c.enabled = true);
            }
            else
            {
                panelObj.SetActive(false);
                selfColliders.ForEach(c => c.enabled = false);
            }
        });
    }

    private void Update() {
        
        if (flashlight.flashed)
        {
            indicateText.text = "Flash is cooling down";
        }
        else if (electricitySystem.Electricity.Value < 0.9f) {
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
