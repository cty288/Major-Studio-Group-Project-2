using System;
using System.Collections;
using System.Collections.Generic;using MikroFramework.Architecture;
using UnityEngine;
[ES3Serializable]
public abstract class TelephoneContact: ICanGetSystem, ICanSendEvent, ICanRegisterEvent, ICanGetModel {
   // [field: ES3NonSerializable]
    public Action OnConversationComplete { get; set; }
    protected Speaker speaker;
    //protected TelephoneSystem telephoneSystem;
    public Action OnTelephoneHangUp { get; set; }
    public Action OnTelephoneStart { get; set; }

    public TelephoneSystem TelephoneSystem {
        get {
            return this.GetSystem<TelephoneSystem>();
        }
    }

    public TelephoneContact() {
       // telephoneSystem = this.GetSystem<TelephoneSystem>();
    }
    public void Start() {
        OnTelephoneStart?.Invoke();
        OnStart();
    }

    public void HangUp() {
        OnTelephoneHangUp?.Invoke();
        OnConversationComplete = null;
        if (speaker.IsSpeaking) {
            speaker.Stop(false);
        }
        OnHangUp();
    }
    

    protected void EndConversation() {
        OnConversationComplete?.Invoke();
        if (speaker.IsSpeaking)
        {
            speaker.Stop(true);
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
