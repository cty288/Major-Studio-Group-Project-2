using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.BindableProperty;
using MikroFramework.FSM;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TelephoneState {
    Idle,
    Dealing,
    Waiting,
    IncomingCall,
    Talking,
}

public class TelephoneSystemUpdater : MonoBehaviour {
    public Action OnUpdate;

    private void Update() {
        OnUpdate?.Invoke();
    }
}

public struct OnDialDigit {
    public int Digit;
}

public enum PhoneDealErrorType {
    NumberNotExists,
    NumberNotAvailable,
}
public class TelephoneSystem : AbstractSystem {
    public BindableProperty<TelephoneState> State { get; private set; } =
        new BindableProperty<TelephoneState>(TelephoneState.Idle);

    private string dealingDigits="";
    
    private TelephoneSystemUpdater updater;
    private float dealWaitTime = 3f;
    private float dealWaitTimer = 0f;
    private GameTimeManager gameTimeManager;
    
    public Action OnDealWaitBeep { get; set; }

    public Action OnIncomingCallBeep { get; set; }
    public Func<PhoneDealErrorType, Func<bool>> OnDealFailed { get; set; }

    public Action OnHangUp { get; set; }

    public Action OnPickUp { get; set; }

    public Dictionary<string, TelephoneContact> Contacts { get; } = new Dictionary<string, TelephoneContact>();

    private Coroutine dealWaitCoroutine;
    private Coroutine incomingCallCoroutine;
    private ElectricitySystem electricitySystem;
    public BindableProperty<TelephoneContact> CurrentTalkingContact { get; } =
        new BindableProperty<TelephoneContact>();
    protected override void OnInit() {
        gameTimeManager = this.GetSystem<GameTimeManager>();
        gameTimeManager.OnDayStart += OnDayEnd;
        updater = new GameObject("TelephoneSystemUpdater").AddComponent<TelephoneSystemUpdater>();
        updater.OnUpdate += Update;
        electricitySystem = this.GetSystem<ElectricitySystem>();
        State.RegisterOnValueChaned(OnStateChanged);
    }

    private bool dayEndLocked = false;
    private void OnStateChanged(TelephoneState oldState, TelephoneState newState) {
        if (newState == TelephoneState.Waiting || newState == TelephoneState.IncomingCall ||
            newState == TelephoneState.Talking) {
            if (!dayEndLocked) {
                gameTimeManager.LockDayEnd.Retain();
                dayEndLocked = true;
            }
            
            
            dealWaitTimer = 0;
        }

        if (newState == TelephoneState.Idle) {
            gameTimeManager.LockDayEnd.Release();
            dayEndLocked = false;
        }
    }

    private void OnDayEnd(int obj) {
        State.Value = TelephoneState.Idle;
    }

    void Update() {
        if (State.Value == TelephoneState.Dealing) {
            dealWaitTimer += Time.deltaTime;
            if (dealWaitTimer >= dealWaitTime) {
                State.Value = TelephoneState.Waiting;
                dealWaitCoroutine = CoroutineRunner.Singleton.StartCoroutine(DealWait());
            }
        }
    }
    public bool CheckContactExists(string digits) {
        return Contacts.ContainsKey(digits);
    }

    private TelephoneContact currentIncomingCallContact = null;

    public TelephoneContact CurrentIncomingCallContact => currentIncomingCallContact;
    public void IncomingCall(TelephoneContact contact, int maxWaitTime) {
        if ( (State.Value == TelephoneState.Idle || State.Value == TelephoneState.Dealing) && electricitySystem.HasElectricity()) {
            currentIncomingCallContact = contact;
            State.Value = TelephoneState.IncomingCall;

            incomingCallCoroutine = CoroutineRunner.Singleton.StartCoroutine(IncomingCallWait(maxWaitTime));
        }
        else {
            contact.HangUp();
        }
    }

    public void ReceiveIncomingCall() {
        if (State.Value == TelephoneState.IncomingCall && currentIncomingCallContact != null) {
            CoroutineRunner.Singleton.StopCoroutine(incomingCallCoroutine);
            StartCallConversation(currentIncomingCallContact);
            OnPickUp.Invoke();
            currentIncomingCallContact = null;
        }
    }
    private IEnumerator IncomingCallWait(int maxWaitTime) {
        for (int i = 0; i < maxWaitTime; i++) {
            OnIncomingCallBeep?.Invoke();
            yield return new WaitForSeconds(4.5f);
        }
        incomingCallCoroutine = null;
        HangUp();
    }

    private IEnumerator DealWait() {
        int beepTime = Random.Range(2, 5);
        for (int i = 0; i < beepTime; i++) {
            OnDealWaitBeep?.Invoke();
            yield return new WaitForSeconds(2.5f);
        }

        if (CheckContactExists(dealingDigits)) {
            if (!Contacts[dealingDigits].OnDealt()) {
                for (int i = 0; i < 2; i++) {
                    OnDealWaitBeep?.Invoke();
                    yield return new WaitForSeconds(3f);
                }
                dealWaitCoroutine = null;
                UntilAction untilAction = UntilAction.Allocate(OnDealFailed(PhoneDealErrorType.NumberNotAvailable));
                untilAction.OnEndedCallback += OnDealFailedCallback;
                untilAction.Execute();
            }
            else {
                StartCallConversation(Contacts[dealingDigits]);
            }
          
        }
        else {
            if (OnDealFailed == null) {
                OnDealFailedCallback();
            }else {
                dealWaitCoroutine = null;
                UntilAction untilAction = UntilAction.Allocate(OnDealFailed(PhoneDealErrorType.NumberNotExists));
                untilAction.OnEndedCallback += OnDealFailedCallback;
                untilAction.Execute();
            }
        }
        ClearDigits();
    }

    public void AddContact(string phoneNumber, TelephoneContact contact) {
        Contacts.Add(phoneNumber, contact);
    }

    public void StartCallConversation(TelephoneContact contact) {
        State.Value = TelephoneState.Talking;
        
        if (CurrentTalkingContact.Value == null) {
            dealWaitCoroutine = null;
            CurrentTalkingContact.Value = contact;
            CurrentTalkingContact.Value.OnConversationComplete += OnFinishConversation;
            CurrentTalkingContact.Value.Start();
        }
    }

    private void OnFinishConversation() {
        CurrentTalkingContact.Value.OnConversationComplete -= OnFinishConversation;
        CurrentTalkingContact.Value = null;
        currentIncomingCallContact = null;
        State.Value = TelephoneState.Idle;
    }

    private void OnDealFailedCallback() {
        State.Value = TelephoneState.Idle;
    }

    /// <summary>
    /// This does not work for idle and dealing
    /// </summary>
    public void HangUp() {
        if (State.Value != TelephoneState.Idle && State.Value != TelephoneState.Dealing) {
            State.Value = TelephoneState.Idle;

            if (incomingCallCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(incomingCallCoroutine);
                incomingCallCoroutine = null;
            }

            if (State.Value == TelephoneState.Waiting && dealWaitCoroutine == null) {
                return; //error messasge speaking
            }
            if (dealWaitCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(dealWaitCoroutine);
                dealWaitCoroutine = null;
            }

            if (CurrentTalkingContact.Value != null) {
                CurrentTalkingContact.Value.OnConversationComplete -= OnFinishConversation;
                CurrentTalkingContact.Value.HangUp();
                CurrentTalkingContact.Value = null;
            }

            if (currentIncomingCallContact != null) {
                currentIncomingCallContact.OnConversationComplete -= OnFinishConversation;
                currentIncomingCallContact.HangUp();
                currentIncomingCallContact = null;
            }
            OnHangUp?.Invoke();
        }
    }
    public void AddDealingDigit(int digit) {
        if (State == TelephoneState.Idle || State == TelephoneState.Dealing) {
            State.Value = TelephoneState.Dealing;
            dealWaitTimer = 0;
            dealingDigits += digit;
        }

        this.SendEvent<OnDialDigit>(new OnDialDigit() {Digit = digit});
    }

    public void ClearDigits() {
        dealingDigits = "";

        if (State.Value == TelephoneState.Dealing) {
            State.Value = TelephoneState.Idle;
        }
    }
}
