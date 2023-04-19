using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameEvents.BountyHunter;
using _02._Scripts.Radio.RadioScheduling;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;


//情报提示的电话事件
[ES3Serializable]
public abstract class BountyHunterQuestClueNotification: IncomingCallEvent {
    [field: ES3Serializable]
    public override GameEventType GameEventType { get; } = GameEventType.BountyHunterQuestClueNotification;
    protected BountyHunterModel bountyHunterModel;
    
   
  
    
    protected BodyManagmentSystem bodyManagmentSystem;
    public BountyHunterQuestClueNotification(TimeRange startTimeRange, BountyHunterQuestClueNotificationContact notificationContact, int callWaitTime, DateTime clueHappenTime) : base(startTimeRange, notificationContact, callWaitTime) {
        bodyManagmentSystem = this.GetSystem<BodyManagmentSystem>((system => {
            bodyManagmentSystem = system;
        } ));
        this.bountyHunterModel = this.GetModel<BountyHunterModel>();
        notificationContact.ClueHappenTime = clueHappenTime;
        //Contact = NotificationContact as BountyHunterQuestClueNotificationContact;
    }
    
    public BountyHunterQuestClueNotification():base() {
        this.bountyHunterModel = this.GetModel<BountyHunterModel>();
        bodyManagmentSystem = this.GetSystem<BodyManagmentSystem>((system => {
            system = bodyManagmentSystem;
        } ));
        
    }

    public override void OnStart() {
        BountyHunterQuestClueNotificationContact Contact = NotificationContact as BountyHunterQuestClueNotificationContact;
        if (bountyHunterModel.QuestBodyTimeInfo.DayRemaining <= 0 || bountyHunterModel.QuestFinished) {
            onHangupOrFinish = true;
            Debug.Log("The Quests is end.");
            return;
        }
        NotificationContact.OnConversationComplete += OnFinishConversation;
        NotificationContact.OnTelephoneHangUp += OnConversationHangUp;
        NotificationContact.OnTelephoneStart += OnConversationStart;
        telephoneSystem.IncomingCall(NotificationContact, callWaitTime);
    }

    protected override void OnConversationStart() {
        BountyHunterQuestClueNotificationContact Contact = NotificationContact as BountyHunterQuestClueNotificationContact;
        DateTime currentTime = gameTimeManager.CurrentTime.Value;

        int timeDifference = (Contact.ClueHappenTime - currentTime).Minutes;
        if (timeDifference <= 0) {
            NotificationContact.HangUp(false);
            return;
        }

        if (timeDifference <= 20) {
            if (Contact.ClueHappenTime.AddMinutes(20).Date == currentTime.Date) {
                Contact.ClueHappenTime = Contact.ClueHappenTime.AddMinutes(20);
            }
        }

        Debug.Log($"Conversation Start. Clue Happen Time: {Contact.ClueHappenTime}");
       
    }

    public override float TriggerChance {
        get {
            return 1;
        }
    }

    public override void OnMissed() {
        BountyHunterQuestClueNotificationContact Contact = NotificationContact as BountyHunterQuestClueNotificationContact;
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        if (currentTime < Contact.ClueHappenTime.AddMinutes(-20)) {
            TimeRange timeRange =
                new TimeRange(this.StartTimeRange.StartTime.AddMinutes(1), this.StartTimeRange.EndTime);
            gameEventSystem.AddEvent(GetSameEvent(timeRange, NotificationContact, callWaitTime, Contact.ClueHappenTime));
            Debug.Log("Conversation Missed. Will retry then");            
        }
        else {
            SendSameEventTomorrow();
            Debug.Log("Conversation Missed. Delayed to tomorrow");
        }
    }

    protected abstract BountyHunterQuestClueNotification GetSameEvent(TimeRange startTimeRange, TelephoneContact contact,
        int callWaitTime, DateTime clueHappenTime);

    protected override void OnComplete() {
        BountyHunterQuestClueNotificationContact Contact = NotificationContact as BountyHunterQuestClueNotificationContact;
        gameEventSystem.AddEvent(GetClueEvent(new TimeRange(Contact.ClueHappenTime)));

        if (bountyHunterModel.QuestBodyTimeInfo.DayRemaining <= 0 || bountyHunterModel.QuestFinished)
        {
            onHangupOrFinish = true;
            Debug.Log("The Quests is end.");
            return;
        }
        DateTime nextClueHappenTime = Contact.ClueHappenTime.AddDays(1);
        if (bountyHunterModel.QuestBodyClueAllHappened) {
            nextClueHappenTime = Contact.ClueHappenTime.AddDays(Random.Range(2, 4));
        }
        nextClueHappenTime = new DateTime(nextClueHappenTime.Year, nextClueHappenTime.Month, nextClueHappenTime.Day, 23, Random.Range(20, 56), 0);

        DateTime nextEventStartTime =
            new DateTime(nextClueHappenTime.Year, nextClueHappenTime.Month, nextClueHappenTime.Day, gameTimeManager.NightTimeStart, Random.Range(5, 20), 0);
        DateTime nextEventEndTime = nextClueHappenTime.AddMinutes(-20);
        
      
        gameEventSystem.AddEvent(GetNextEvent(new TimeRange(nextEventStartTime, nextEventEndTime), callWaitTime, nextClueHappenTime));
        Debug.Log("Conversation Complete. Next Clue Happen Time: " + nextClueHappenTime);
    }

    protected override void OnHangUp(bool hangUpByPlayer) {
        if (hangUpByPlayer) {
            OnComplete();
            return;
        }
        if (bountyHunterModel.QuestBodyTimeInfo.DayRemaining <= 0) {
            return;
        }
        SendSameEventTomorrow();
        Debug.Log("Conversation Hang Up. Delayed to tomorrow");
    }

    private void SendSameEventTomorrow() {
        BountyHunterQuestClueNotificationContact Contact = NotificationContact as BountyHunterQuestClueNotificationContact;
        DateTime tomorrow = gameTimeManager.CurrentTime.Value.AddDays(1);
        DateTime tomorrowClueHappenTime = Contact.ClueHappenTime.AddDays(1);

        DateTime tomorrowEventStartTime =
            new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, gameTimeManager.NightTimeStart, Random.Range(5, 20), 0);
        DateTime tomorrowEventEndTime = tomorrowClueHappenTime.AddMinutes(-20);

        gameEventSystem.AddEvent(GetSameEvent(new TimeRange(tomorrowEventStartTime, tomorrowEventEndTime), NotificationContact, callWaitTime, tomorrowClueHappenTime));
    }

    protected abstract BountyHunterQuestClueNotification GetNextEvent(TimeRange startTimeRange,
        int callWaitTime, DateTime clueHappenTime);

    protected abstract BountyHunterQuestClueEvent GetClueEvent(TimeRange happenTimeRange);
}









//情报提示的电话
[ES3Serializable]
public abstract class BountyHunterQuestClueNotificationContact : TelephoneContact, ICanGetModel {
    [field: ES3Serializable]
    public DateTime ClueHappenTime { get; set; }

    protected BountyHunterModel bountyHunterModel;

    public BountyHunterQuestClueNotificationContact(): base() {
        speaker = GameObject.Find("BountyHunterSpeaker").GetComponent<Speaker>();
        bountyHunterModel = this.GetModel<BountyHunterModel>();
    }

   
    public override bool OnDealt() {
        return !bountyHunterModel.IsInJail;
    }
}





//情报发送的事件
public abstract class BountyHunterQuestClueEvent : GameEvent {
    protected BountyHunterQuestClueEvent(TimeRange startTimeRange) : base(startTimeRange) {
        
    }
    [field: ES3Serializable]
    public override GameEventType GameEventType { get; } = GameEventType.BountyHunterQuestClue;
    [field: ES3Serializable]
    public override float TriggerChance { get; } = 1;
  

    public override void OnEnd() {
        //send 2 events, one is for real clue info, one is for fake clue info
        SendClueEvent(true);
        SendClueEvent(false);
    }

    protected void SendClueEvent(bool isReal) {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;

        DateTime clueHappenTime = currentTime.AddMinutes(Random.Range(20, 60));

        if (clueHappenTime.Date != currentTime.Date)
        {
            clueHappenTime = new DateTime(clueHappenTime.Year, clueHappenTime.Month, clueHappenTime.Day,
                Random.Range(gameTimeManager.NightTimeStart, 24), Random.Range(10, 50), 0);
        }

        DateTime clueEndTime = new DateTime(clueHappenTime.Year, clueHappenTime.Month, clueHappenTime.Day,
            23, 59, 0);
        gameEventSystem.AddEvent(GetClueInfoEvent(new TimeRange(clueHappenTime, clueEndTime), isReal, currentTime));
        Debug.Log("Clue Radio will Happen at: " + clueHappenTime);
    }
    public override void OnMissed() {
        
    }

    protected abstract BountyHunterQuestClueInfoEvent GetClueInfoEvent(TimeRange happenTimeRange, bool isRealClue,
        DateTime startDate);
}












//相关情报的收音机内容
public abstract class BountyHunterQuestClueInfoEvent : ScheduledRadioEvent<RadioTextContent> {
    [field: ES3Serializable]
    protected override RadioProgramType ProgramType { get; set; } = RadioProgramType.DailyDeadBody;

    [field: ES3Serializable]
    protected bool IsRealClue { get; set; }
    [field: ES3Serializable]
    protected DateTime startDate;
    public BountyHunterQuestClueInfoEvent(TimeRange startTimeRange, string speakContent, float speakRate, Gender speakGender, AudioMixerGroup mixer, bool isReal, DateTime startDate) 
        : base(startTimeRange, new RadioTextContent(speakContent, speakRate, speakGender, mixer), RadioChannel.FM96) {
        this.IsRealClue = isReal;
        this.startDate = startDate;
    }
    
    public BountyHunterQuestClueInfoEvent(): base(){}

    public override float TriggerChance {
        get {
            DateTime currentTime = this.GetSystem<GameTimeManager>().CurrentTime.Value;
            if ((currentTime - startDate).Days > 3) {
                Debug.Log("Clue Info Event Out of Date");
                return 0;
            }

            return 1;
        }
    }

    
    protected override ScheduledRadioEvent<RadioTextContent> OnGetNextRadioProgramMessage(TimeRange nextTimeRange, bool playSuccess) {
        DateTime currentTime = this.GetSystem<GameTimeManager>().CurrentTime.Value;
        if ((currentTime - startDate).Days > 3) {
            return null;
        }

       
        return GetSameEvent(nextTimeRange, IsRealClue, startDate) as ScheduledRadioEvent<RadioTextContent>;
    }

    protected abstract GameEvent GetSameEvent(TimeRange timeRange, bool isRealClue, DateTime dateTime);
}