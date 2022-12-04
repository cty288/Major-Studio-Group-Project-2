using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public abstract class IncomingCallEvent : GameEvent {
    protected TelephoneSystem telephoneSystem;
    protected TelephoneContact NotificationContact;
    protected int callWaitTime;
    protected IncomingCallEvent(TimeRange startTimeRange, TelephoneContact notificationContact, int callWaitTime) : base(startTimeRange) {
        telephoneSystem = this.GetSystem<TelephoneSystem>();
        this.NotificationContact = notificationContact;
        this.callWaitTime = callWaitTime;
    }

    public override GameEventType GameEventType { get; } = GameEventType.IncomingCall;
    protected bool onHangupOrFinish = false;
    public override void OnStart() {
        NotificationContact.OnConversationComplete += OnFinishConversation;
        NotificationContact.OnTelephoneHangUp += OnConversationHangUp;
        NotificationContact.OnTelephoneStart += OnConversationStart;
        telephoneSystem.IncomingCall(NotificationContact, callWaitTime);
    }

    protected abstract void OnConversationStart();


    protected void OnFinishConversation() {
        UnRegisterEvents();
        OnComplete();
        onHangupOrFinish = true;
    }

    protected void OnConversationHangUp() {
        UnRegisterEvents();
        OnHangUp();
        onHangupOrFinish = true;
    }    

    protected abstract void OnComplete();
    protected abstract void OnHangUp();

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
