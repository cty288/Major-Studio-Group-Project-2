using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerGeneratorViewController : ElectricalApplicance, IPointerClickHandler {
    [SerializeField] private List<Sprite> electricitySprites;
    private SpriteRenderer spriteRenderer;
    protected override void Awake() {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        electricityModel.Electricity.RegisterWithInitValue(OnElectricityChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
       
    }

    private void OnElectricityChanged(float arg1, float electricity) {
        int index = 0;
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
