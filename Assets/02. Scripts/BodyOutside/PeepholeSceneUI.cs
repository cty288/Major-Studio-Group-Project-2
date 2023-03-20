using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.Electricity;
using DG.Tweening;
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
    public Action OnLightStopped = () => { };
    private GameSceneModel gameSceneModel;
    private ElectricityModel electricityModel;
    private TMP_Text indicateText;
    private GameObject panelObj;
    public OutdoorFlashLight flashlight;
    
    private List<Collider2D> selfColliders;
    protected ElectricitySystem electricitySystem;
    public bool flashed; 
    private void Awake() {
        gameSceneModel = this.GetModel<GameSceneModel>();
        panelObj = transform.Find("Panel").gameObject;
        lightButton = transform.Find("Panel/Button").GetComponent<Button>();
        lightButton.onClick.AddListener(OnLightPressed);
        lightButton.GetComponent<Image>().alphaHitTestMinimumThreshold = 1;
        gameSceneModel.GameScene.RegisterWithInitValue(OnGameSceneChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        electricityModel = this.GetModel<ElectricityModel>();
        indicateText = panelObj.transform.Find("IndicateText").GetComponent<TMP_Text>();
        selfColliders = GetComponents<Collider2D>().ToList();
        electricitySystem = this.GetSystem<ElectricitySystem>();
        //colliders = GetComponents<Collider2D>().ToList();
    }

    private void OnGameSceneChanged(GameScene scene) {
        this.Delay(0.5f, () => {
            if (scene == GameScene.Peephole) {
                panelObj.SetActive(true);
                selfColliders.ForEach(c => c.enabled = true);
            }
            else {
                panelObj.SetActive(false);
                selfColliders.ForEach(c => c.enabled = false);
            }
        });
    }

    private void Update() {
        
        if (flashed)
        {
            indicateText.text = "";
        }
        else if (electricityModel.Electricity.Value < 0.2f) {
            indicateText.text = "Not enough electricity";
        }
        else {
            indicateText.text = "";
        }
        
        
    }

    private void OnLightPressed() {
       
        
        if (this.electricityModel.Electricity.Value > 0.2f && flashed == false) {
            OnLightButtonPressed?.Invoke();
            StartCoroutine(FlashCoolDown());
            flashed = true;
            if (electricityModel.PowerCutoff) {
                this.electricitySystem.UseElectricity(0.2f);
            }
           
        }
    }
    
    IEnumerator FlashCoolDown()
    {
        yield return new WaitForSeconds(5);
        flashed = false;
        OnLightStopped?.Invoke();
    }
}
