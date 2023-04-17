using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.BodyOutside;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

public class BountyHunterQuestAlienSpawnEvent : DailyKnockEvent {
    private BodyModel bodyModel;
    public BountyHunterQuestAlienSpawnEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float eventTriggerChance) :
        base(startTimeRange) {
        bodyModel = this.GetModel<BodyModel>();
        this.bodyInfo = bodyInfo;
    }

    public BountyHunterQuestAlienSpawnEvent() : base() {
        bodyModel = this.GetModel<BodyModel>();
    }

    public override void OnStart() {
        
    }

    public override EventState OnUpdate() {
        if (bodyModel.IsInAllBodyTimeInfos(bodyInfo)) {
            return base.OnUpdate();
        }
        Debug.Log("Bounty Hunter Quest: The body is already dead");
        return EventState.End;
    }

    

    private void SpawnNextTime() {
        if (!bodyModel.IsInAllBodyTimeInfos(bodyInfo)) {
            Debug.Log("Bounty Hunter Quest: The body is already dead");
            return;
        }

        DateTime nextStartTime = StartTimeRange.StartTime.AddDays(Random.Range(2, 4));
        nextStartTime = new DateTime(nextStartTime.Year, nextStartTime.Month, nextStartTime.Day, Random.Range(gameTimeManager.NightTimeStart, 24),
            Random.Range(10, 45), 0);
        DateTime nextEndTime = new DateTime(nextStartTime.Year, nextStartTime.Month, nextStartTime.Day,
            23, 50, 0);
        gameEventSystem.AddEvent(new BountyHunterQuestAlienSpawnEvent(new TimeRange(nextStartTime, nextEndTime),
            bodyInfo, 1));
    }

    public override void OnMissed() {
        SpawnNextTime();
    }

    public override void OnEventEnd() {
        SpawnNextTime();
    }
}
