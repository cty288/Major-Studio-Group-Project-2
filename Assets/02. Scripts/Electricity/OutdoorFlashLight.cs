using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework.Architecture;
using MikroFramework.Event;
using Random = UnityEngine.Random;

public class OutdoorFlashLight : ElectricalApplicance
{
    private SpriteRenderer flashlight;
    private BodyGenerationModel bodyGenerationModel;
    [SerializeField] private Speaker speaker;
    protected override void Awake() {
        base.Awake();
        bodyGenerationModel = this.GetModel<BodyGenerationModel>();
    }

    void Start() {
        flashlight = this.GetComponent<SpriteRenderer>();
    }
    
    private void Update() {
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            OpenFlashLight();
        }
    }

    void OpenFlashLight()
    {
        if (this.electricitySystem.Electricity.Value > 0.95f) {
            if (bodyGenerationModel.CurrentOutsideBody.Value != null) {
                if (bodyGenerationModel.CurrentOutsideBody.Value.IsAlien) {
                    bodyGenerationModel.CurrentOutsideBody.Value = null;
                }
                else {
                    List<string> replies = new List<string>();
                    replies.Add("Hey! Da fuck are you doing?! Stop pointing that stupid light at me!");
                    replies.Add("Holy! You¡¯ve got a SUN in your house?! Don't tell me you are some kind of giant light bulb alien!");
                    replies.Add("Cut it off, mister! Or I will call the officers!");
                    string reply = replies[UnityEngine.Random.Range(0, replies.Count)];
                    speaker.Speak(reply, null, null, Random.Range(0.8f, 1.2f));
                }
            }
            flashlight.DOFade(1, 0.1f);
            flashlight.DOFade(0, 0.1f).SetDelay(0.3f);
            this.electricitySystem.UseElectricity(0.95f);
        }
    }

    protected override void OnNoElectricity()
    {
        
    }

    protected override void OnElectricityRecovered()
    {
        
    }
}
