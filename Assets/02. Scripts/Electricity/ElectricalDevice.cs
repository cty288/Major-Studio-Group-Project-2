using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalDevice : ElectricalApplicance
{
    public bool enabled;

    private void Start()
    {
        enabled = this.electricityModel.Electricity.Value > 0;
    }

    protected override void OnNoElectricity()
    {
        enabled = false;
    }

    protected override void OnElectricityRecovered()
    {
        enabled = true;
    }
}
