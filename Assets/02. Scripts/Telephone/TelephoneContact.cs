using System;
using System.Collections;
using System.Collections.Generic;using MikroFramework.Architecture;
using UnityEngine;
[ES3Serializable]
public abstract class TelephoneContact: ICanGetSystem, ICanSendEvent, ICanRegisterEvent {
   // [field: ES3NonSerializable]
    public Action OnConversationComplete { get; set; }
    protected Speaker speaker;
    protected TelephoneSystem telephoneSystem;
    public Action OnTelephoneHangUp { get; set; }
    public Action OnTelephoneStart { get; set; }

    public TelephoneContact() {
        telephoneSystem = this.GetSystem<TelephoneSystem>();
    }
    public void Start() {
        OnTelephoneStart?.Invoke();
        OnStart();
    }

    public void HangUp() {
        OnTelephoneHangUp?.Invoke();
        OnConversationComplete = null;
        if (speaker.IsSpeaking) {
            speaker.Stop();
        }
        OnHangUp();
    }
    

    protected void EndConversation() {
        OnConversationComplete?.Invoke();
        if (speaker.IsSpeaking)
        {
            speaker.Stop();
        }
        OnEnd();
    }

    public abstract bool OnDealt();

    protected abstract void OnStart();

    protected abstract void OnHangUp();

    protected abstract void OnEnd();
    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
