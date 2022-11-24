using System;
using System.Collections;
using System.Collections.Generic;using MikroFramework.Architecture;
using UnityEngine;

public abstract class TelephoneContact: ICanGetSystem, ICanSendEvent, ICanRegisterEvent {
    public Action OnConversationComplete { get; set; }
   
    public void Start() {
        OnStart();
    }

    public void HangUp() {
        OnConversationComplete = null;
        OnHangUp();
    }
    

    protected void EndConversation() {
        OnConversationComplete?.Invoke();
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
