using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.FPSEnding;
using _02._Scripts.GameEvents.BountyHunter;
using _02._Scripts.GameEvents.BountyHunter.NewQuest;
using _02._Scripts.GameTime;
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

    protected override void OnHangUp(bool hangUpByPlayer) {
        if (hangUpByPlayer) {
            OnComplete();
            return;
        }
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
        BodyModel bodyModel = this.GetModel<BodyModel>();
        MonsterMotherModel monsterMotherModel = this.GetModel<MonsterMotherModel>();
        if (!this.GetModel<MonsterMotherModel>().MonsterMotherSpawned && !bodyModel.IsInAllBodyTimeInfos(monsterMotherModel.MotherBodyTimeInfo.BodyInfo)) {
            bodyModel.AddToAllBodyTimeInfos(monsterMotherModel.MotherBodyTimeInfo);
        }
        
       
        GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
        DateTime nextStartTime = gameTimeModel.GetDay(gameTimeModel.Day + Random.Range(2, 4));
        nextStartTime = new DateTime(nextStartTime.Year, nextStartTime.Month, nextStartTime.Day, 23, Random.Range(0,40), 0);
        DateTime nextEndTime = nextStartTime.AddMinutes(20);

        if (!monsterMotherModel.MonsterMotherSpawned) {
            monsterMotherModel.MonsterMotherSpawned = true;
            this.GetSystem<GameEventSystem>().AddEvent(new MonsterMotherSpawnEvent(new TimeRange(nextStartTime, nextEndTime), monsterMotherModel.MotherBodyTimeInfo.BodyInfo, 1));
        }
       


        DateTime nextClueHappenTime = gameTimeManager.CurrentTime.Value.AddDays(1);
        nextClueHappenTime = nextClueHappenTime.AddMinutes(Random.Range(5, 60));



        gameEventSystem.AddEvent(new BountyHunterClue1Event(
            new TimeRange(nextClueHappenTime, nextClueHappenTime.AddMinutes(30)),
            new BountyHunterClue1Contact(), 6));

        Debug.Log("Bounty Hunter First Clue Happen Time: " + nextClueHappenTime);
    }


}
