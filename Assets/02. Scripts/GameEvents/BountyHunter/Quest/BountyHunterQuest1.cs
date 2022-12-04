using System;
using System.Collections;
using System.Collections.Generic;
using Crosstales.RTVoice.Model.Enum;
using UnityEngine;
using UnityEngine.Audio;

public  class BountyHunterQuest1ClueNotification : BountyHunterQuestClueNotification {
    public BountyHunterQuest1ClueNotification(TimeRange startTimeRange, BountyHunterQuestClueNotificationContact notificationContact, int callWaitTime, DateTime clueHappenTime) 
        : base(startTimeRange, notificationContact, callWaitTime, clueHappenTime) {
    }

    protected override BountyHunterQuestClueNotification GetSameEvent(TimeRange startTimeRange, TelephoneContact contact, int callWaitTime,
        DateTime clueHappenTime) {
        return new BountyHunterQuest1ClueNotification(startTimeRange, (BountyHunterQuestClueNotificationContact) contact,
            callWaitTime, clueHappenTime);
    }

    protected override BountyHunterQuestClueNotification GetNextEvent(TimeRange startTimeRange,  int callWaitTime,
        DateTime clueHappenTime) {
        Debug.Log("BountyHunterQuest1ClueNotification.GetNextEvent");
        return new BountyHunterQuest1ClueNotification(startTimeRange, NotificationContact, callWaitTime,
            clueHappenTime);
    }

    protected override BountyHunterQuestClueEvent GetClueEvent(TimeRange happenTimeRange) {
        return new BountyHunterQuest1ClueEvent(happenTimeRange);
    }

   
}

public class BountyHunterQuest1ClueNotificationNotificationContact : BountyHunterQuestClueNotificationContact
{
    protected override void OnStart() {
        string welcome = $"{ClueHappenTime} Hey! My friend! This is the bounty hunter! I don¡¯t know what kind of magic you have, but you have successfully helped me capture several aliens." +
                         " Based on your recent achievements, I¡¯ve decided to give you a higher-level task.";
     
        speaker.Speak(welcome, AudioMixerList.Singleton.AudioMixerGroups[2], OnSpeakEnd);
    }

    private void OnSpeakEnd() {
        EndConversation();
    }

    protected override void OnHangUp() {
       
    }

    protected override void OnEnd() {
        
    }
}

public class BountyHunterQuest1ClueEvent : BountyHunterQuestClueEvent {
    public BountyHunterQuest1ClueEvent(TimeRange startTimeRange) : base(startTimeRange) {
    }

    public override void OnStart() {
        Debug.Log("Clue Start!");
    }

    public override EventState OnUpdate() {
        return EventState.End;
    }

    protected override BountyHunterQuestClueInfoEvent GetClueInfoEvent(TimeRange happenTimeRange, bool isRealClue, DateTime startDate) {
        return new BountyHunterQuestClueInfoRadioEvent(happenTimeRange, "Bounty Hunter Quest Clue Radio", 1.2f,
            Gender.FEMALE, null, isRealClue, startDate);
    }
}


public class BountyHunterQuestClueInfoRadioEvent : BountyHunterQuestClueInfoEvent {
    public BountyHunterQuestClueInfoRadioEvent(TimeRange startTimeRange, string speakContent, float speakRate, Gender speakGender, AudioMixerGroup mixer, bool isReal, DateTime startDate) : base(startTimeRange, speakContent, speakRate, speakGender, mixer, isReal, startDate) {
    }

    protected override void OnRadioStart() {
       
    }

    protected override GameEvent GetSameEvent(TimeRange timeRange, bool isRealClue, DateTime dateTime) {
        return new BountyHunterQuestClueInfoRadioEvent(timeRange, this.speakContent, this.speakRate, this.speakGender, this.mixer,
            isRealClue, dateTime);
    }
}
