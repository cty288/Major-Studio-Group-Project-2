using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;


public class BountyHunterQuest2 : BountyHunterQuestClueNotification {
    public BountyHunterQuest2(TimeRange startTimeRange, BountyHunterQuestClueNotificationContact notificationContact, int callWaitTime, DateTime clueHappenTime) : base(startTimeRange, notificationContact, callWaitTime, clueHappenTime) {
    }

    protected override BountyHunterQuestClueNotification GetSameEvent(TimeRange startTimeRange, TelephoneContact contact, int callWaitTime,
        DateTime clueHappenTime) {
        return new BountyHunterQuest2(startTimeRange, (BountyHunterQuest2ClueNotificationNotificationContact) contact, callWaitTime, clueHappenTime);
    }

    protected override BountyHunterQuestClueNotification GetNextEvent(TimeRange startTimeRange, int callWaitTime, DateTime clueHappenTime) {
        throw new NotImplementedException();
    }

    protected override BountyHunterQuestClueEvent GetClueEvent(TimeRange happenTimeRange) {
        TimeRange newTimeRange = new TimeRange(happenTimeRange.StartTime, happenTimeRange.StartTime.AddMinutes(10));
        
        throw new NotImplementedException();
    }
}

public class BountyHunterQuest2ClueNotificationNotificationContact : BountyHunterQuestClueNotificationContact {
    protected override void OnStart() {
        string hourIn12_1 = String.Format("{0: h}", ClueHappenTime);
        
        
        string welcome =
            $"Hey friend, I hope you got some clues. I have gathered more information about the location of a dead body killed by the creature we are looking for." +
            $" Between {hourIn12_1}:{ClueHappenTime.Minute} and {hourIn12_1}:{ClueHappenTime.Minute+10} pm," +
            $" someone will deliver a note to you. Make sure there's no one outside your home when he arrives, otherwise he might be scared away. ";

        speaker.Speak(welcome, AudioMixerList.Singleton.AudioMixerGroups[2], OnSpeakEnd);
    }

    protected override void OnHangUp() {
        
    }

    protected override void OnEnd() {
       
    }

    private void OnSpeakEnd() {
        this.GetSystem<BountyHunterSystem>().QuestBodyClueAllHappened = true;
        EndConversation();
    }
}
