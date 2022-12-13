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
    public bool flashed; 
    private SpriteRenderer flashlight;
    private BodyGenerationModel bodyGenerationModel;
    [SerializeField] private Speaker speaker;
    [SerializeField] private PeepholeSceneUI peepholeSceneUI;
    protected override void Awake() {
        base.Awake();
        bodyGenerationModel = this.GetModel<BodyGenerationModel>();
      
    }

    void Start() {
        flashlight = this.GetComponent<SpriteRenderer>();
        peepholeSceneUI.OnLightButtonPressed += OpenFlashLight;
        flashed = false;
    }
    
   
    void OpenFlashLight()
    {
        if (this.electricitySystem.Electricity.Value > 0.9f && flashed == false) {
            if (bodyGenerationModel.CurrentOutsideBody.Value != null) {
                if (bodyGenerationModel.CurrentOutsideBody.Value.IsAlien) {
                    bodyGenerationModel.CurrentOutsideBody.Value = null;
                }
                else {
                    List<string> replies = new List<string>();
                    replies.Add("Hey! Da fuck are you doing?! Stop pointing that stupid light at me!");
                    replies.Add("Holy! You��ve got a SUN in your house?! Don't tell me you are some kind of giant light bulb alien!");
                    replies.Add("Cut it off, mister! Or I will call the officers!");
                    string reply = replies[UnityEngine.Random.Range(0, replies.Count)];
                    speaker.Speak(reply, null, "",null,Random.Range(0.8f, 1.2f));
                }
            }
            StartCoroutine(FlashCoolDown());
            flashed = true;
            flashlight.DOFade(1, 0.1f);
            flashlight.DOFade(0, 0.1f).SetDelay(0.3f);
            this.electricitySystem.UseElectricity(0.9f);
        }
    }

    IEnumerator FlashCoolDown()
    {
        yield return new WaitForSeconds(Random.Range(60, 120));
        flashed = false;
        yield return null;
    }

    protected override void OnNoElectricity()
    {
        
    }

    protected override void OnElectricityRecovered()
    {
        
    }
}
