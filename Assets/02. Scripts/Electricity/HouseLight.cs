using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HouseLight : ElectricalApplicance
{
    private Light2D houseLight;
    protected override void Awake() {
        base.Awake();
        houseLight = this.GetComponent<Light2D>();
    }

    void Update() {
        SetLightIntensity();
    }

    void SetLightIntensity()
    {
        houseLight.intensity = 0.25f + 1.25f * this.electricityModel.Electricity.Value;
    }
    
    protected override void OnNoElectricity()
    {
        
    }

    protected override void OnElectricityRecovered()
    {
        
    }
}
