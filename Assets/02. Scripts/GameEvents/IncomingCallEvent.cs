using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public abstract class IncomingCallEvent : GameEvent {
    protected TelephoneSystem telephoneSystem;
    [ES3Serializable]
    protected TelephoneContact NotificationContact;
    [ES3Serializable]
    protected int callWaitTime;
    
    
    protected IncomingCallEvent(TimeRange startTimeRange, TelephoneContact notificationContact, int callWaitTime) : base(startTimeRange) {
        telephoneSystem = this.GetSystem<TelephoneSystem>(sys => {
            telephoneSystem = sys;
        });
        this.NotificationContact = notificationContact;
        this.callWaitTime = callWaitTime;
    }

    public IncomingCallEvent(): base() {
        telephoneSystem = this.GetSystem<TelephoneSystem>(sys => {
            telephoneSystem = sys;
        });
    }
    [field: ES3Serializable]
    public override GameEventType GameEventType { get; } = GameEventType.IncomingCall;
    [ES3Serializable]
    protected bool onHangupOrFinish = false;
    public override void OnStart() {
        telephoneSystem = this.GetSystem<TelephoneSystem>();
        NotificationContact.OnConversationComplete += OnFinishConversation;
        NotificationContact.OnTelephoneHangUp += OnConversationHangUp;
        NotificationContact.OnTelephoneStart += OnConversationStart;
      //  if (telephoneSystem == null) {
           // telephoneSystem = this.GetSystem<TelephoneSystem>();
       // }
        telephoneSystem.IncomingCall(NotificationContact, callWaitTime);
    }

    protected abstract void OnConversationStart();


    protected void OnFinishConversation() {
        UnRegisterEvents();
        OnComplete();
        onHangupOrFinish = true;
    }

    protected void OnConversationHangUp(bool hangUpByPlayer) {
        UnRegisterEvents();
        OnHangUp(hangUpByPlayer);
        onHangupOrFinish = true;
    }    

    protected abstract void OnComplete();
    protected abstract void OnHangUp(bool hangUpByPlayer);

    protected void UnRegisterEvents() {
        NotificationContact.OnConversationComplete -= OnFinishConversation;
        NotificationContact.OnTelephoneHangUp -= OnConversationHangUp;
        NotificationContact.OnTelephoneStart -= OnConversationStart;
    }

    public override EventState OnUpdate() {
        return onHangupOrFinish ? EventState.End : EventState.Running;
    }

    public override void OnEnd() {
        
    }
}
