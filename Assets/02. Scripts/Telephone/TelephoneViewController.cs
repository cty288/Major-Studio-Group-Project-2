using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class TelephoneViewController : ElectricalApplicance, IPointerClickHandler {
    [SerializeField] private OpenableUIPanel panel;
    [SerializeField] private Speaker speaker;
    
    private TelephoneSystem telephoneSystem;

    protected override void Awake() {
        base.Awake();
        telephoneSystem = this.GetSystem<TelephoneSystem>();
        telephoneSystem.OnDealWaitBeep += OnDealWaitBeep;
        telephoneSystem.OnDealFailed += OnDealFailed;
        telephoneSystem.OnHangUp += OnHangUp;
    }

    protected override void OnNoElectricity() {
        telephoneSystem.HangUp();
    }

    protected override void OnElectricityRecovered() {
       
    }

    private void OnDestroy() {
        telephoneSystem.OnDealWaitBeep -= OnDealWaitBeep;
        telephoneSystem.OnDealFailed -= OnDealFailed;
        telephoneSystem.OnHangUp -= OnHangUp;
    }

    private void OnHangUp() {
        AudioSystem.Singleton.Play2DSound("hang_out");
        speaker.Stop();
    }

    private Func<bool> OnDealFailed(PhoneDealErrorType failType) {
        string speakText = "";
        switch (failType) {
            case PhoneDealErrorType.NumberNotAvailable:
                speakText = "The number you dialed is not available. Please try again later.";
                break;
            case PhoneDealErrorType.NumberNotExists:
                speakText = "The number you dialed does not exist. Please check the number and try again.";
                break;
        }

        speaker.Speak(speakText, null,null, 1f, 1.5f);
        return () => !speaker.IsSpeaking;
    }

    private void OnDealWaitBeep() {
        AudioSystem.Singleton.Play2DSound("phone_dialing");
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (!electricitySystem.HasElectricity()) {
            return;
        }
        panel.Show();
    }
}
