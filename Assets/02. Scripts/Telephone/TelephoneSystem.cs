using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
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
    public Func<PhoneDealErrorType, Func<bool>> OnDealFailed { get; set; }

    public Action OnHangUp { get; set; }

    public Dictionary<string, TelephoneContact> Contacts { get; } = new Dictionary<string, TelephoneContact>();

    private Coroutine dealWaitCoroutine;

    public BindableProperty<TelephoneContact> CurrentTalkingContact { get; } =
        new BindableProperty<TelephoneContact>();
    protected override void OnInit() {
        gameTimeManager = this.GetSystem<GameTimeManager>();
        gameTimeManager.OnDayStart += OnDayEnd;
        updater = new GameObject("TelephoneSystemUpdater").AddComponent<TelephoneSystemUpdater>();
        updater.OnUpdate += Update;

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
    private IEnumerator DealWait() {
        int beepTime = Random.Range(3, 6);
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
