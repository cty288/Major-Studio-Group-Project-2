using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HouseLight : ElectricalApplicance
{
    public Light2D houseLight;
    void Update()
    {
        SetLightIntensity();
    }

    void SetLightIntensity()
    {
        houseLight.intensity = 0.25f + 1.5f * this.electricitySystem.Electricity.Value;
    }
    
    protected override void OnNoElectricity()
    {
        
    }

    protected override void OnElectricityRecovered()
    {
        
    }
}
