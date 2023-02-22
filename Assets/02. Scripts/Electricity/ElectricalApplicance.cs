using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Electricity;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public abstract class ElectricalApplicance : AbstractMikroController<MainGame> {
    protected ElectricityModel electricityModel;
    protected Transform noElectricityCanvas;
    protected virtual void Awake() {
        electricityModel = this.GetModel<ElectricityModel>();
        this.RegisterEvent<OnNoElectricity>(OnNoElectricity).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnElectricityRecovered>(OnElectricityRecovered).UnRegisterWhenGameObjectDestroyed(gameObject);
        
        noElectricityCanvas = transform.Find("NoElectricityCanvas");
    }

    private void OnElectricityRecovered(OnElectricityRecovered e) {
        OnElectricityRecovered();
        if (noElectricityCanvas) {
            noElectricityCanvas.gameObject.SetActive(false);
        }
    }

    private void OnNoElectricity(OnNoElectricity e) {
        OnNoElectricity();
        if (noElectricityCanvas) {
            noElectricityCanvas.gameObject.SetActive(true);
        }
    }

    protected abstract void OnNoElectricity();
    protected abstract void OnElectricityRecovered();
}
