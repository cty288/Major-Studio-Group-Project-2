using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public abstract class ElectricalApplicance : AbstractMikroController<MainGame> {
    protected ElectricitySystem electricitySystem;

    protected virtual void Awake() {
        electricitySystem = this.GetSystem<ElectricitySystem>();
        this.RegisterEvent<OnNoElectricity>(OnNoElectricity).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnElectricityRecovered>(OnElectricityRecovered).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnElectricityRecovered(OnElectricityRecovered e) {
        OnElectricityRecovered();
    }

    private void OnNoElectricity(OnNoElectricity e) {
        OnNoElectricity();
    }

    protected abstract void OnNoElectricity();
    protected abstract void OnElectricityRecovered();
}
