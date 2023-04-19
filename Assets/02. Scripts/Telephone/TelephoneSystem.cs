using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.ChoiceSystem;
using _02._Scripts.Electricity;
using _02._Scripts.GameTime;
using _02._Scripts.Telephone;
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
public class TelephoneSystem : AbstractSavableSystem {
    [field: ES3Serializable]
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

    [field: ES3Serializable]
    public Dictionary<string, TelephoneContact> Contacts { get; } = new Dictionary<string, TelephoneContact>();

    private Coroutine dealWaitCoroutine;
    private Coroutine incomingCallCoroutine;
    private ElectricityModel electricityModel;
    
    [field: ES3Serializable]
    public BindableProperty<TelephoneContact> CurrentTalkingContact { get; } =
        new BindableProperty<TelephoneContact>();


    [field: ES3Serializable] public bool IsBroken { get; set; } = false;
    
    protected override void OnInit() {
        gameTimeManager = this.GetSystem<GameTimeManager>();
        gameTimeManager.OnDayStart += OnDayEnd;
        updater = new GameObject("TelephoneSystemUpdater").AddComponent<TelephoneSystemUpdater>();
        updater.OnUpdate += Update;
        electricityModel = this.GetModel<ElectricityModel>();
        this.RegisterEvent<OnNewDay>(OnNewDayStart);
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

    private void OnDayEnd(int day, int hour) {
        State.Value = TelephoneState.Idle;
    }
    private void OnNewDayStart(OnNewDay e) {
        if (e.Day == 0) {
            IsBroken = true;
            GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
            DateTime telephoneBrokenRadio = gameTimeModel.GetDay(1);
            this.GetSystem<GameEventSystem>().AddEvent(new TelephoneBrokenRadioEvent(
                new TimeRange(telephoneBrokenRadio.AddMinutes(10)),
                AudioMixerList.Singleton.AudioMixerGroups[1]));


            int telephoneFixDay = int.Parse(this.GetModel<HotUpdateDataModel>().GetData("PhoneFixDay").values[0]);
            DateTime telephoneFixedTime = gameTimeModel.GetDay(telephoneFixDay);
            this.GetSystem<GameEventSystem>().AddEvent(new TelephoneFixedEvent(new TimeRange(telephoneFixedTime)));

            ImportantNewspaperModel newspaperModel = this.GetModel<ImportantNewspaperModel>();
            newspaperModel.AddPageToNewspaper(newspaperModel.GetWeekForNews(telephoneFixDay),
                this.GetModel<ImportantNewsTextModel>().GetInfo("LandlineMaintenanceEnds"), 0);
            
            //newspaperModel.AddPageToNewspaper(newspaperModel.GetWeekForNews(telephoneFixDay),
              //  this.GetModel<ImportantNewsTextModel>().GetInfo("ExampleKey"));
        }
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
        if ((State.Value == TelephoneState.Idle || State.Value == TelephoneState.Dealing) && electricityModel.HasElectricity() && contact.OnDealt() && !IsBroken) {
            currentIncomingCallContact = contact;
            State.Value = TelephoneState.IncomingCall;

            incomingCallCoroutine = CoroutineRunner.Singleton.StartCoroutine(IncomingCallWait(maxWaitTime));
        }
        else {
            contact.HangUp(false);
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
        HangUp(false);
    }

    private IEnumerator DealWait() {
        int beepTime = Random.Range(2, 5);
        for (int i = 0; i < beepTime; i++) {
            OnDealWaitBeep?.Invoke();
            yield return new WaitForSeconds(2.5f);
        }

        if (CheckContactExists(dealingDigits)) {
            if (!Contacts[dealingDigits].OnDealt() || IsBroken) {
                for (int i = 0; i < 2; i++) {
                    OnDealWaitBeep?.Invoke();
                    yield return new WaitForSeconds(3f);
                }
                dealWaitCoroutine = null;
                OnDealFailed?.Invoke(PhoneDealErrorType.NumberNotAvailable);
                Contacts[dealingDigits].HangUp(false);
                OnDealFailedCallback();
            }
            else {
                StartCallConversation(Contacts[dealingDigits]);
            }
          
        }
        else {
            if (OnDealFailed == null) {
                //OnDealFailedCallback();
            }else {
                dealWaitCoroutine = null;
                OnDealFailed?.Invoke(PhoneDealErrorType.NumberNotExists);
            }
            OnDealFailedCallback();
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
    public void HangUp(bool hangUpByPlayer) {
        if (State.Value != TelephoneState.Idle && State.Value != TelephoneState.Dealing) {
            State.Value = TelephoneState.Idle;
            this.GetSystem<ChoiceSystem>().StopChoiceGroup(ChoiceType.Telephone);

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
                CurrentTalkingContact.Value.HangUp(hangUpByPlayer);
                CurrentTalkingContact.Value = null;
            }

            if (currentIncomingCallContact != null) {
                currentIncomingCallContact.OnConversationComplete -= OnFinishConversation;
                currentIncomingCallContact.HangUp(hangUpByPlayer);
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
