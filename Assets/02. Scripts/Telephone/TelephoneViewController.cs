using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class TelephoneViewController : ElectricalApplicance, IPointerClickHandler {
    [SerializeField] private OpenableUIPanel panel;
    [SerializeField] private Speaker speaker;
    
    private TelephoneSystem telephoneSystem;
    private AudioSource incomingCallAudioSource;
    protected override void Awake() {
        base.Awake();
        telephoneSystem = this.GetSystem<TelephoneSystem>();
        telephoneSystem.OnDealWaitBeep += OnDealWaitBeep;
        telephoneSystem.OnDealFailed += OnDealFailed;
        telephoneSystem.OnHangUp += OnHangUp;
        telephoneSystem.OnIncomingCallBeep += OnIncomingCallBeep;
        telephoneSystem.OnPickUp += OnPickUp;
        this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnNewDay(OnNewDay e) {
        if (e.Day == 0) {
            gameObject.SetActive(false);
        }
        else {
            gameObject.SetActive(true);
        }
    }

    private void OnPickUp() {
        if (incomingCallAudioSource)
        {
            incomingCallAudioSource.Stop();
            incomingCallAudioSource = null;
        }
    }

    private void OnIncomingCallBeep() {
        incomingCallAudioSource =  AudioSystem.Singleton.Play2DSound("incoming_call");
        transform.DOShakeRotation(2.5f, 1, 30, 90, false);
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
        if (electricityModel.HasElectricity()) {
            AudioSystem.Singleton.Play2DSound("hang_out");
        }
      
        if (incomingCallAudioSource) {
            incomingCallAudioSource.Stop();
            incomingCallAudioSource = null;
        }
        speaker.Stop();
    }

    private Func<bool> OnDealFailed(PhoneDealErrorType failType) {
        string speakText = "";
        switch (failType) {
            case PhoneDealErrorType.NumberNotAvailable:
                if (telephoneSystem.IsBroken) {
                    speakText = "Unable to deal with the call. Please check your phone line wiring and try again.";
                }
                else {
                    speakText = "The number you dialed is not available. Please try again later.";
                }
               
                break;
            case PhoneDealErrorType.NumberNotExists:
                speakText = "The number you dialed does not exist. Please check the number and try again.";
                break;
        }

        speaker.Speak(speakText, null,"",1f, null, 1f, 1.5f);
        return () => !speaker.IsSpeaking;
    }

    private void OnDealWaitBeep() {
        AudioSystem.Singleton.Play2DSound("phone_dialing");
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (!electricityModel.HasElectricity()) {
            return;
        }
        panel.Show(0.5f);
        if (telephoneSystem.CurrentIncomingCallContact != null) {
            telephoneSystem.ReceiveIncomingCall();
        }
    }
}
