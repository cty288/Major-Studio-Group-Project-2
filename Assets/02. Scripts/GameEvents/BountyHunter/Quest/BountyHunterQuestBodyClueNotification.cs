using System;
using System.Collections;
using System.Collections.Generic;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;


//情报提示的电话事件
public abstract class BountyHunterQuestClueNotification: IncomingCallEvent {
    public override GameEventType GameEventType { get; } = GameEventType.BountyHunterQuestClueNotification;
    protected BountyHunterSystem bountyHunterSystem;
    protected BountyHunterQuestClueNotificationContact NotificationContact;
    public BountyHunterQuestClueNotification(TimeRange startTimeRange, BountyHunterQuestClueNotificationContact notificationContact, int callWaitTime, DateTime clueHappenTime) : base(startTimeRange, notificationContact, callWaitTime) {
        this.bountyHunterSystem = this.GetSystem<BountyHunterSystem>();
        notificationContact.ClueHappenTime = clueHappenTime;
        this.NotificationContact = notificationContact;
    }

    public override void OnStart() {
        if (bountyHunterSystem.QuestBodyTimeInfo.DayRemaining <= 0) {
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
        DateTime currentTime = gameTimeManager.CurrentTime.Value;

        int timeDifference = (NotificationContact.ClueHappenTime - currentTime).Minutes;
        if (timeDifference <= 0) {
            NotificationContact.HangUp();
            return;
        }

        if (timeDifference <= 20) {
            if (NotificationContact.ClueHappenTime.AddMinutes(20).Date == currentTime.Date) {
                NotificationContact.ClueHappenTime = NotificationContact.ClueHappenTime.AddMinutes(20);
            }
        }

        Debug.Log($"Conversation Start. Clue Happen Time: {NotificationContact.ClueHappenTime}");
       
    }

    public override float TriggerChance { get; } = 1;
    public override void OnMissed() {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        if (currentTime < NotificationContact.ClueHappenTime.AddMinutes(-20)) {
            TimeRange timeRange =
                new TimeRange(this.StartTimeRange.StartTime.AddMinutes(1), this.StartTimeRange.EndTime);
            gameEventSystem.AddEvent(GetSameEvent(timeRange, NotificationContact, callWaitTime, NotificationContact.ClueHappenTime));
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
      
        gameEventSystem.AddEvent(GetClueEvent(new TimeRange(NotificationContact.ClueHappenTime)));

        if (bountyHunterSystem.QuestBodyTimeInfo.DayRemaining <= 0) {
            return;
        }
        DateTime nextClueHappenTime = NotificationContact.ClueHappenTime.AddDays(1);
        if (bountyHunterSystem.QuestBodyClueAllHappened) {
            nextClueHappenTime = NotificationContact.ClueHappenTime.AddDays(Random.Range(2, 4));
        }
        nextClueHappenTime = new DateTime(nextClueHappenTime.Year, nextClueHappenTime.Month, nextClueHappenTime.Day, 23, Random.Range(20, 56), 0);

        DateTime nextEventStartTime =
            new DateTime(nextClueHappenTime.Year, nextClueHappenTime.Month, nextClueHappenTime.Day, 22, Random.Range(10, 40), 0);
        DateTime nextEventEndTime = nextClueHappenTime.AddMinutes(-20);
        
      
        gameEventSystem.AddEvent(GetNextEvent(new TimeRange(nextEventStartTime, nextEventEndTime), callWaitTime, nextClueHappenTime));
        Debug.Log("Conversation Complete. Next Clue Happen Time: " + nextClueHappenTime);
    }

    protected override void OnHangUp() {
        if (bountyHunterSystem.QuestBodyTimeInfo.DayRemaining <= 0) {
            return;
        }
        SendSameEventTomorrow();
        Debug.Log("Conversation Hang Up. Delayed to tomorrow");
    }

    private void SendSameEventTomorrow() {
        DateTime tomorrow = gameTimeManager.CurrentTime.Value.AddDays(1);
        DateTime tomorrowClueHappenTime = NotificationContact.ClueHappenTime.AddDays(1);

        DateTime tomorrowEventStartTime =
            new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 22, Random.Range(10, 40), 0);
        DateTime tomorrowEventEndTime = tomorrowClueHappenTime.AddMinutes(-20);

        gameEventSystem.AddEvent(GetSameEvent(new TimeRange(tomorrowEventStartTime, tomorrowEventEndTime), NotificationContact, callWaitTime, tomorrowClueHappenTime));
    }

    protected abstract BountyHunterQuestClueNotification GetNextEvent(TimeRange startTimeRange,
        int callWaitTime, DateTime clueHappenTime);

    protected abstract BountyHunterQuestClueEvent GetClueEvent(TimeRange happenTimeRange);
}
//情报提示的电话
public abstract class BountyHunterQuestClueNotificationContact : TelephoneContact {
    public DateTime ClueHappenTime { get; set; }

    protected BountyHunterPhone bountyHunterPhone;

    public BountyHunterQuestClueNotificationContact() {
        speaker = GameObject.Find("BountyHunterSpeaker").GetComponent<Speaker>();
        bountyHunterPhone =
            telephoneSystem.Contacts[this.GetSystem<BountyHunterSystem>().PhoneNumber] as BountyHunterPhone;
    }
    public override bool OnDealt() {
        return !bountyHunterPhone.IsInJail;
    }
}
//情报发送的事件
public abstract class BountyHunterQuestClueEvent : GameEvent {
    protected BountyHunterQuestClueEvent(TimeRange startTimeRange) : base(startTimeRange) {
        
    }

    public override GameEventType GameEventType { get; } = GameEventType.BountyHunterQuestClue;
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
                Random.Range(22, 24), Random.Range(10, 50), 0);
        }

        DateTime clueEndTime = new DateTime(clueHappenTime.Year, clueHappenTime.Month, clueHappenTime.Day,
            23, 59, 0);
        gameEventSystem.AddEvent(GetClueInfoEvent(new TimeRange(clueHappenTime, clueEndTime), isReal, currentTime));
        Debug.Log("Clue Happen Time: " + clueHappenTime);
    }
    public override void OnMissed() {
        
    }

    protected abstract BountyHunterQuestClueInfoEvent GetClueInfoEvent(TimeRange happenTimeRange, bool isRealClue,
        DateTime startDate);
}

//相关情报的收音机内容
public abstract class BountyHunterQuestClueInfoEvent : RadioEvent {
    protected bool IsRealClue { get; set; }
    protected DateTime startDate;
    public BountyHunterQuestClueInfoEvent(TimeRange startTimeRange, string speakContent, float speakRate, Gender speakGender, AudioMixerGroup mixer, bool isReal, DateTime startDate) : base(startTimeRange, speakContent, speakRate, speakGender, mixer) {
        this.IsRealClue = isReal;
        this.startDate = startDate;
    }

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

    public override void OnEnd() {
        DateTime currentTime = this.GetSystem<GameTimeManager>().CurrentTime.Value;
        if ((currentTime - startDate).Days > 3) {
            return;
        }

        DateTime nextStartTime = currentTime.AddDays(1);
        nextStartTime = new DateTime(nextStartTime.Year, nextStartTime.Month, nextStartTime.Day, Random.Range(22,24),
            Random.Range(5, 55), 0);
        DateTime nextEventEndTime = new DateTime(nextStartTime.Year, nextStartTime.Month, nextStartTime.Day, 23, 58, 0);

        gameEventSystem.AddEvent(GetSameEvent(new TimeRange(nextStartTime, nextEventEndTime), IsRealClue, startDate));
    }

    public override void OnMissed() {
        DateTime currentTime = this.GetSystem<GameTimeManager>().CurrentTime.Value;
        if ((currentTime - startDate).Days > 3) {
            return;
        }

        DateTime nextStartTime = currentTime.AddMinutes(1);
        DateTime nextEventEndTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 23, 58, 0);
        if (nextStartTime > nextEventEndTime) {
            nextStartTime = currentTime.AddDays(1);
            nextStartTime = new DateTime(nextStartTime.Year, nextStartTime.Month, nextStartTime.Day, 22,
                Random.Range(10, 30), 0);
            nextEventEndTime = new DateTime(nextStartTime.Year, nextStartTime.Month, nextStartTime.Day, 23, 58, 0);
            
            gameEventSystem.AddEvent(GetSameEvent(new TimeRange(nextStartTime, nextEventEndTime), IsRealClue, startDate));
        }
        else {
            gameEventSystem.AddEvent(GetSameEvent(new TimeRange(nextStartTime, nextEventEndTime), IsRealClue, startDate));
        }
    }

    protected abstract GameEvent GetSameEvent(TimeRange timeRange, bool isRealClue, DateTime dateTime);
}