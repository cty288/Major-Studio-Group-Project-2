using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using UnityEngine;
[ES3Serializable]
public abstract class TelephoneContact: ICanGetSystem, ICanSendEvent, ICanRegisterEvent, ICanGetModel {
   // [field: ES3NonSerializable]
    public Action OnConversationComplete { get; set; }
    protected Speaker speaker;
    //protected TelephoneSystem telephoneSystem;
    public Action<bool> OnTelephoneHangUp { get; set; }
    public Action OnTelephoneStart { get; set; }
    
    protected GameTimeModel gameTimeModel;

    public TelephoneSystem TelephoneSystem {
        get {
            return this.GetSystem<TelephoneSystem>();
        }
    }

    public TelephoneContact() {
        gameTimeModel = this.GetModel<GameTimeModel>();
       // telephoneSystem = this.GetSystem<TelephoneSystem>();
    }
    public void Start() {
        OnTelephoneStart?.Invoke();
        gameTimeModel.LockTime.Retain();
        OnStart();
    }

    public void HangUp(bool hangUpByPlayer) {
        OnTelephoneHangUp?.Invoke(hangUpByPlayer);
        OnConversationComplete = null;
        if (speaker.IsSpeaking) {
            speaker.Stop(false);
        }
        gameTimeModel.LockTime.Release();
        OnHangUp(hangUpByPlayer);
    }
    

    protected void EndConversation() {
        OnConversationComplete?.Invoke();
        if (speaker.IsSpeaking)
        {
            speaker.Stop(true);
        }
        gameTimeModel.LockTime.Release();
        OnEnd();
    }

    public abstract bool OnDealt();

    protected abstract void OnStart();

    protected abstract void OnHangUp(bool hangUpByPlayer);

    protected abstract void OnEnd();
    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
