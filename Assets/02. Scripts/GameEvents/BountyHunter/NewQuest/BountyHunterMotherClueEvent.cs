
	using System;
    using _02._Scripts.FPSEnding;
    using _02._Scripts.GameEvents.BountyHunter;
    using _02._Scripts.GameTime;
    using MikroFramework.Architecture;
    using UnityEngine;
    using Random = UnityEngine.Random;

    
    public abstract  class BountyHunterMotherClueEvent : IncomingCallEvent {
        
        [field: ES3Serializable]
    public override GameEventType GameEventType { get; } = GameEventType.IncomingCall;
    protected BountyHunterModel bountyHunterModel;
    
   
    
    protected BodyManagmentSystem bodyManagmentSystem;
    public BountyHunterMotherClueEvent(TimeRange startTimeRange, BountyHunterMonsterClueContact notificationContact, int callWaitTime) : base(startTimeRange, notificationContact, callWaitTime) {
        bodyManagmentSystem = this.GetSystem<BodyManagmentSystem>((system => {
            bodyManagmentSystem = system;
        } ));
        this.bountyHunterModel = this.GetModel<BountyHunterModel>();
    }
    
    public BountyHunterMotherClueEvent():base() {
        this.bountyHunterModel = this.GetModel<BountyHunterModel>();
        bodyManagmentSystem = this.GetSystem<BodyManagmentSystem>((system => {
            system = bodyManagmentSystem;
        } ));
        
    }

    public override void OnStart() {
        telephoneSystem = this.GetSystem<TelephoneSystem>();
        BountyHunterMonsterClueContact Contact = NotificationContact as BountyHunterMonsterClueContact;
        MonsterMotherModel monsterMotherModel = this.GetModel<MonsterMotherModel>();
        if (monsterMotherModel.MotherBodyTimeInfo.DayRemaining <= 0 || bountyHunterModel.QuestFinished) {
            onHangupOrFinish = true;
            Debug.Log("The Quests is end.");
            return;
        }
        NotificationContact.OnConversationComplete += OnFinishConversation;
        NotificationContact.OnTelephoneHangUp += OnConversationHangUp;
        NotificationContact.OnTelephoneStart += OnConversationStart;
        telephoneSystem.IncomingCall(NotificationContact, callWaitTime);
    }



    public override float TriggerChance {
        get {
            return 1;
        }
    }

    public override void OnMissed() {
        BountyHunterMonsterClueContact Contact = NotificationContact as BountyHunterMonsterClueContact;
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        if (currentTime.Hour < 23) {
            TimeRange timeRange =
                new TimeRange(this.StartTimeRange.StartTime.AddMinutes(1), this.StartTimeRange.EndTime);
            gameEventSystem.AddEvent(GetSameEvent(timeRange, NotificationContact, callWaitTime));
            Debug.Log("Conversation Missed. Will retry then");            
        }
        else {
            SendSameEventTomorrow();
            Debug.Log("Conversation Missed. Delayed to tomorrow");
        }
    }

    protected abstract BountyHunterMotherClueEvent GetSameEvent(TimeRange startTimeRange, TelephoneContact contact,
        int callWaitTime);

    protected override void OnComplete() {
        BountyHunterMonsterClueContact Contact = NotificationContact as BountyHunterMonsterClueContact;
        
        MonsterMotherModel monsterMotherModel = this.GetModel<MonsterMotherModel>();
        if (monsterMotherModel.MotherBodyTimeInfo.DayRemaining <= 0 || bountyHunterModel.QuestFinished) { //need to set
            onHangupOrFinish = true;
            Debug.Log("The Quests is end.");
            return;
        }
        
        
        GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();

        DateTime nextClueHappenTime = gameTimeModel.GetDay(gameTimeModel.Day + Random.Range(1, 3));
        if (bountyHunterModel.QuestBodyClueAllHappened) { //need to be set
            nextClueHappenTime = gameTimeModel.GetDay(gameTimeModel.Day + Random.Range(2, 4));
        }

        nextClueHappenTime = nextClueHappenTime.AddMinutes(Random.Range(5, 50));

        var nextEvent = GetNextEvent(new TimeRange(nextClueHappenTime, nextClueHappenTime.AddMinutes(30)),
            callWaitTime);

        if (nextEvent == null) {
            return;
        }
      
        gameEventSystem.AddEvent(nextEvent);
        Debug.Log("Conversation Complete. Next Clue Happen Time: " + nextClueHappenTime);
    }

    protected override void OnHangUp(bool hangUpByPlayer) {
        if (hangUpByPlayer) {
            OnComplete();
            return;
        }
        MonsterMotherModel monsterMotherModel = this.GetModel<MonsterMotherModel>();
        if (monsterMotherModel.MotherBodyTimeInfo.DayRemaining <= 0) {
            return;
        }
        SendSameEventTomorrow();
        Debug.Log("Conversation Hang Up. Delayed to tomorrow");
    }

    private void SendSameEventTomorrow() {
        BountyHunterMonsterClueContact Contact = NotificationContact as BountyHunterMonsterClueContact;
        GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();

        DateTime nextClueHappenTime = gameTimeModel.GetDay(gameTimeModel.Day + 1);


        gameEventSystem.AddEvent(GetSameEvent(new TimeRange(nextClueHappenTime, nextClueHappenTime.AddMinutes(40)),
            NotificationContact, callWaitTime));
    }

    protected abstract BountyHunterMotherClueEvent GetNextEvent(TimeRange startTimeRange, int callWaitTime);
    
    
    
    }


//情报提示的电话
    [ES3Serializable]
    public abstract class BountyHunterMonsterClueContact : TelephoneContact, ICanGetModel {
        protected BountyHunterModel bountyHunterModel;

        public BountyHunterMonsterClueContact(): base() {
            speaker = GameObject.Find("BountyHunterSpeaker").GetComponent<Speaker>();
            bountyHunterModel = this.GetModel<BountyHunterModel>();
        }

   
        public override bool OnDealt() {
            return !bountyHunterModel.IsInJail;
        }
    }

