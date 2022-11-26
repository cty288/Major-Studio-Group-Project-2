using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework.Architecture;
using MikroFramework.Event;

public class OutdoorFlashLight : ElectricalApplicance
{
    public SpriteRenderer flashlight;
    public Color color;
    public float alpha;

    void Start()
    {
        flashlight = this.GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        color = new Color(1, 1, 1, alpha);
        flashlight.color = color;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OpenFlashLight();
        }
    }

    void OpenFlashLight()
    {
        if (this.electricitySystem.Electricity.Value > 0.95f)
        {
            var seq = DOTween.Sequence();
            seq.Append(DOTween.To(()=> alpha, x=> alpha = x, 1, 0.1f));
            seq.Append(DOTween.To(()=> alpha, x=> alpha = x, 0, 0.1f));
            seq.Restart();

            this.electricitySystem.UseElectricity(0.95f);
            BodyGenerationModel bodyGenerationModel = this.GetModel<BodyGenerationModel>();
            if (bodyGenerationModel.CurrentOutsideBody.Value!=null)
            {
                if (bodyGenerationModel.CurrentOutsideBody.Value.IsAlien)
                {
                    //Destroy(bodyGenerationModel.CurrentOutsideBody.Value);
                }
            }
        }
    }

    protected override void OnNoElectricity()
    {
        
    }

    protected override void OnElectricityRecovered()
    {
        
    }
}
