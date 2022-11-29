using System;
using System.Collections;
using System.Collections.Generic;
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
    public BindableProperty<float> Electricity { get; private set; } = new BindableProperty<float>(0.6f);

    private float electricityDecreaseRate = 0.003f;
    protected override void OnInit() {
        updater = new GameObject("ElectricitySystemUpdater").AddComponent<ElectricitySystemUpdater>();
        Electricity.RegisterOnValueChaned(OnElectricityChange);
        
        updater.OnUpdate += Update;
    }

    private void Update() {
        Electricity.Value = Mathf.Max(Electricity.Value - electricityDecreaseRate * Time.deltaTime, 0);
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
        Electricity.Value = Mathf.Min(Electricity.Value + amount, 1);
    }

    public void UseElectricity(float amount) {
        Electricity.Value = Mathf.Max(Electricity.Value - amount, 0);
    }

    public bool HasElectricity() {
        return Electricity.Value > 0;
    }
}
