using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class OutdoorFlashLight : ElectricalApplicance
{
   
    private Light2D flashlight;
    private BodyGenerationModel bodyGenerationModel;
    [SerializeField] private Speaker speaker;
    [SerializeField] private PeepholeSceneUI peepholeSceneUI;
   
    
    protected override void Awake() {
        base.Awake();
        bodyGenerationModel = this.GetModel<BodyGenerationModel>();
       
    }

    void Start() {
        flashlight = this.GetComponent<Light2D>();
        peepholeSceneUI.OnLightButtonPressed += OpenFlashLight;
        peepholeSceneUI.OnLightStopped += OnCloseFlashLight;
    }

    private void OnDestroy() {
        peepholeSceneUI.OnLightButtonPressed -= OpenFlashLight;
        peepholeSceneUI.OnLightStopped -= OnCloseFlashLight;
    }

    private void OnCloseFlashLight() {
        DOTween.To(() => flashlight.intensity, x => flashlight.intensity = x, 1, 0.2f);
    }


    void OpenFlashLight()
    {
        if (bodyGenerationModel.CurrentOutsideBody.Value != null) {
            IVoiceTag voiceTag = bodyGenerationModel.CurrentOutsideBody.Value.VoiceTag;


            bodyGenerationModel.CurrentOutsideBody.Value.KnockBehavior?.OnHitByFlashlight(speaker, voiceTag,
                bodyGenerationModel.CurrentOutsideBody.Value.IsAlien);
            
        }
       
        DOTween.To(() => flashlight.intensity, x => flashlight.intensity = x, 5, 0.2f);
       
       
    }

   

    protected override void OnNoElectricity()
    {
        
    }

    protected override void OnElectricityRecovered() {
        
    }
}
