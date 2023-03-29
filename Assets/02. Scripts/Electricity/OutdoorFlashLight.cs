using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework.Architecture;
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
            if (bodyGenerationModel.CurrentOutsideBody.Value.IsAlien) {
                //bodyGenerationModel.CurrentOutsideBody.Value = null;
            }
            else {
                bool speak = Random.Range(0, 100) <= 40;
                if (speak) {
                    List<string> replies = new List<string>();
                    replies.Add("Hey! Da fuck are you doing?! Stop pointing that stupid light at me!");
                    replies.Add("Holy! You've got a SUN in your house?! Don't tell me you are some kind of giant light bulb alien!");
                    replies.Add("Cut it off, mister! Or I will call the officers!");
                    string reply = replies[UnityEngine.Random.Range(0, replies.Count)];

                    IVoiceTag voiceTag = bodyGenerationModel.CurrentOutsideBody.Value.VoiceTag;
                    if (voiceTag != null) {
                        speaker.Speak(reply, AudioMixerList.Singleton.AlienVoiceGroups[voiceTag.VoiceIndex],
                            "",1f, null,voiceTag.VoiceSpeed,1f,
                            voiceTag.VoiceType);
                    }
                    else {
                        speaker.Speak(reply, null, "",1f, null,Random.Range(0.8f, 1.2f));
                    }
                }
            }
        }
       
        DOTween.To(() => flashlight.intensity, x => flashlight.intensity = x, 5, 0.2f);
       
       
    }

   

    protected override void OnNoElectricity()
    {
        
    }

    protected override void OnElectricityRecovered() {
        
    }
}
