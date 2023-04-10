using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerGeneratorViewController : ElectricalApplicance, IPointerClickHandler {
    [SerializeField] private List<Sprite> electricitySprites;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private bool isUI = false; 
    protected override void Awake() {
        base.Awake();
        gameObject.SetActive(false);
        spriteRenderer = GetComponent<SpriteRenderer>();
        electricityModel.Electricity.RegisterWithInitValue(OnElectricityChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnPlayerResourceNumberChanged>(OnPlayerResourceNumberChanged);
        
        if (this.GetModel<PlayerResourceModel>().HasEnoughResource<PowerGeneratorGoods>(1)) {
            gameObject.SetActive(true);
        }
    }

    private void Start() {
        
    }

    private void OnPlayerResourceNumberChanged(OnPlayerResourceNumberChanged e) {
        if (e.GoodsInfo.Type == typeof(PowerGeneratorGoods)) {
            if (e.GoodsInfo.Count > 0) {
                OnHasElectricityGeneratorChanged(true);
            }else {
                OnHasElectricityGeneratorChanged(false);
            }

        }
    }

    private void OnHasElectricityGeneratorChanged(bool hasElectricityGenerator) {
        gameObject.SetActive(hasElectricityGenerator);
    }
        
    

    private void OnElectricityChanged(float arg1, float electricity) {
        int index = 0;
        if (!isUI) {
            if (electricity <= 0.01f) {
                index = 0;
            }
            else if (electricity <= 0.45f) {
                index = 1;
            }
            else if (electricity <= 0.95f) {
                index = 2;
            }
            else {
                index = 3;
            }

        }else {
            if (electricity <= 0.01f) {
                index = 0;
            }
            else if (electricity <= 0.25f) {
                index = 1;
            }
            else if (electricity <= 0.5f) {
                index = 2;
            }
            else if(electricity<=0.75) {
                index = 3;
            }else if (electricity <= 0.95f) {
                index = 4;
            }
            else {
                index = 5;
            }

        }
       
        spriteRenderer.sprite = electricitySprites[index];
    }

    public void OnPointerClick(PointerEventData eventData) {
        this.GetModel<GameSceneModel>().GameScene.Value = GameScene.PowerGenerator;
    }

    protected override void OnNoElectricity() {
        
    }

    protected override void OnElectricityRecovered() {
       
    }
}
