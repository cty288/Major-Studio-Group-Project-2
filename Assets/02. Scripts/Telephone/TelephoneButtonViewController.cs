using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using UnityEngine;
using UnityEngine.UI;

public class TelephoneButtonViewController : AbstractMikroController<MainGame> {
    [SerializeField] private int number;
    [SerializeField] private bool isHangUpButton;
    [SerializeField] private AudioClip pressClip;

    private TelephoneSystem telephoneSystem;

    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        telephoneSystem = this.GetSystem<TelephoneSystem>();
    }

    private void OnClick() {
        AudioSystem.Singleton.Play2DSound(pressClip);
        if (!isHangUpButton) {
            telephoneSystem.AddDealingDigit(number);
        }else {
            switch (telephoneSystem.State.Value) {
                case TelephoneState.Idle:
                    break;
                case TelephoneState.Dealing:
                    telephoneSystem.ClearDigits();
                    break;
                case TelephoneState.Waiting:
                case TelephoneState.Talking:
                case TelephoneState.IncomingCall:
                    telephoneSystem.HangUp(true);
                    break;
            }
        }
    }
}
