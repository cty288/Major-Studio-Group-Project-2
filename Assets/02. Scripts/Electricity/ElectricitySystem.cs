using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Electricity;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;
public class ElectricitySystemUpdater : MonoBehaviour
{
    public Action OnUpdate;
    private void Update()
    {
        OnUpdate?.Invoke();
    }
}

public struct OnNoElectricity {

}

public struct OnElectricityRecovered {

}
public class ElectricitySystem : AbstractSystem {
    private ElectricitySystemUpdater updater;

    private ElectricityModel electricityModel;
    
    private float electricityDecreaseRate = 0.002f;

    protected GameTimeManager gameTimeManager;
    protected override void OnInit() {
        electricityModel = this.GetModel<ElectricityModel>();
        updater = new GameObject("ElectricitySystemUpdater").AddComponent<ElectricitySystemUpdater>();
        electricityModel.Electricity.RegisterOnValueChaned(OnElectricityChange);
        gameTimeManager = this.GetSystem<GameTimeManager>();
        updater.OnUpdate += Update;
    }

    private void Update() {
        if (!gameTimeManager.IsNight || !electricityModel.HasElectricityGenerator) {
            return;
        }
        electricityModel.Electricity.Value = Mathf.Max(electricityModel.Electricity.Value - electricityDecreaseRate * Time.deltaTime, 0);
    }
    private void OnElectricityChange(float oldElectricity, float newElectricity) {
        if (newElectricity <= 0 && oldElectricity > 0) {
            this.SendEvent<OnNoElectricity>();
        }

        if (oldElectricity <= 0 && newElectricity > 0) {
            this.SendEvent<OnElectricityRecovered>();
        }
    }

    public void AddElectricity(float amount) {
        electricityModel.Electricity.Value = Mathf.Min(electricityModel.Electricity.Value + amount, 1);
    }

    public void UseElectricity(float amount) {
        electricityModel.Electricity.Value = Mathf.Max(electricityModel.Electricity.Value - amount, 0);
    }


}
