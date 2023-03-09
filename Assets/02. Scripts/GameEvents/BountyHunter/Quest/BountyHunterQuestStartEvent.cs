using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.GameEvents.BountyHunter;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

public class BountyHunterQuestStartEvent : IncomingCallEvent {
    protected BountyHunterModel bountyHunterModel;
    public BountyHunterQuestStartEvent(TimeRange startTimeRange, TelephoneContact contact, int callWaitTime) : base(startTimeRange, contact, callWaitTime) {
        bountyHunterModel = this.GetModel<BountyHunterModel>();
    }

    public BountyHunterQuestStartEvent(): base() {
        bountyHunterModel = this.GetModel<BountyHunterModel>();
    }

    public override float TriggerChance {
        get {
            if (bountyHunterModel.IsInJail) {
                return 0;
            }
            return 1;
        }
    }

    public override void OnMissed() {
        OnMissedOrHangUp();
    }

    protected override void OnHangUp() {
        OnMissedOrHangUp();
    }

    private void OnMissedOrHangUp() {
        DateTime today = gameTimeManager.CurrentTime.Value;
        DateTime targetNextTime = today.AddDays(1);
        DateTime targetTime = new DateTime(targetNextTime.Year, targetNextTime.Month, targetNextTime.Day, gameTimeManager.NightTimeStart,
            Random.Range(30, 60), 0);
        DateTime targetEndTime = new DateTime(targetTime.Year, targetTime.Month, targetTime.Day, 23, 58, 0);
        gameEventSystem.AddEvent(new BountyHunterQuestStartEvent(new TimeRange(targetTime, targetEndTime), NotificationContact,
            callWaitTime));
    }

    protected override void OnConversationStart() {
        
    }

    protected override void OnComplete() {
        this.GetModel<BodyModel>().AddToAllBodyTimeInfos(this.GetModel<BountyHunterModel>().QuestBodyTimeInfo);


        DateTime nextClueHappenTime = gameTimeManager.CurrentTime.Value.AddDays(1);
       
        nextClueHappenTime = new DateTime(nextClueHappenTime.Year, nextClueHappenTime.Month, nextClueHappenTime.Day, 23, Random.Range(20, 56), 0);

        DateTime nextEventStartTime =
            new DateTime(nextClueHappenTime.Year, nextClueHappenTime.Month, nextClueHappenTime.Day, gameTimeManager.NightTimeStart, Random.Range(5, 20), 0);
        DateTime nextEventEndTime = nextClueHappenTime.AddMinutes(-20);

        gameEventSystem.AddEvent(new BountyHunterQuest1ClueNotification(
            new TimeRange(nextEventStartTime, nextEventEndTime),
            new BountyHunterQuest1ClueNotificationNotificationContact(), 6, nextClueHappenTime));

        Debug.Log("Bounty Hunter First Clue Happen Time: " + nextClueHappenTime);
    }


}
