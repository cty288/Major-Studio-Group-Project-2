using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

public class BountyHunterQuestStartEvent : IncomingCallEvent{
    public BountyHunterQuestStartEvent(TimeRange startTimeRange, TelephoneContact contact, int callWaitTime) : base(startTimeRange, contact, callWaitTime) {
    }

    public override float TriggerChance { get; } = 1;
    public override void OnMissed() {
        OnMissedOrHangUp();
    }

    protected override void OnHangUp() {
        OnMissedOrHangUp();
    }

    private void OnMissedOrHangUp() {
        DateTime today = gameTimeManager.CurrentTime.Value;
        DateTime targetNextTime = today.AddDays(1);
        DateTime targetTime = new DateTime(targetNextTime.Year, targetNextTime.Month, targetNextTime.Day, 22,
            Random.Range(30, 60), 0);
        DateTime targetEndTime = new DateTime(targetTime.Year, targetTime.Month, targetTime.Day, 23, 58, 0);
        gameEventSystem.AddEvent(new BountyHunterQuestStartEvent(new TimeRange(targetTime, targetEndTime), NotificationContact,
            callWaitTime));
    }

    protected override void OnConversationStart() {
        
    }

    protected override void OnComplete() {
        this.GetSystem<BodyManagmentSystem>().AddToAllBodyTimeInfos(this.GetSystem<BountyHunterSystem>().QuestBodyTimeInfo);


        DateTime nextClueHappenTime = gameTimeManager.CurrentTime.Value.AddDays(1);
       
        nextClueHappenTime = new DateTime(nextClueHappenTime.Year, nextClueHappenTime.Month, nextClueHappenTime.Day, 23, Random.Range(20, 56), 0);

        DateTime nextEventStartTime =
            new DateTime(nextClueHappenTime.Year, nextClueHappenTime.Month, nextClueHappenTime.Day, 22, Random.Range(10, 40), 0);
        DateTime nextEventEndTime = nextClueHappenTime.AddMinutes(-20);

        gameEventSystem.AddEvent(new BountyHunterQuest1ClueNotification(
            new TimeRange(nextEventStartTime, nextEventEndTime),
            new BountyHunterQuest1ClueNotificationNotificationContact(), 6, nextClueHappenTime));

        Debug.Log("Bounty Hunter First Clue Happen Time: " + nextClueHappenTime);
    }


}
