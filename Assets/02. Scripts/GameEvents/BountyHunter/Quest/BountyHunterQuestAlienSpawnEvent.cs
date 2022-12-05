using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

public class BountyHunterQuestAlienSpawnEvent : BodyGenerationEvent {
    private BodyManagmentSystem bodyManagmentSystem;
    public BountyHunterQuestAlienSpawnEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float knockDoorTimeInterval, int knockTime, float eventTriggerChance, string overrideAudioClipName = null) :
        base(startTimeRange, bodyInfo, knockDoorTimeInterval, knockTime, eventTriggerChance, null, null, overrideAudioClipName) {
        bodyManagmentSystem = this.GetSystem<BodyManagmentSystem>();
    }

    public override EventState OnUpdate() {
        if (bodyManagmentSystem.IsInAllBodyTimeInfos(bodyInfo)) {
            return base.OnUpdate();
        }
        Debug.Log("Bounty Hunter Quest: The body is already dead");
        return EventState.End;
    }

    public override void OnEnd() {
        base.OnEnd();
        SpawnNextTime();
    }

    private void SpawnNextTime() {
        if (!bodyManagmentSystem.IsInAllBodyTimeInfos(bodyInfo)) {
            Debug.Log("Bounty Hunter Quest: The body is already dead");
            return;
        }

        DateTime nextStartTime = StartTimeRange.StartTime.AddDays(Random.Range(2, 4));
        nextStartTime = new DateTime(nextStartTime.Year, nextStartTime.Month, nextStartTime.Day, Random.Range(22, 24),
            Random.Range(10, 45), 0);
        DateTime nextEndTime = new DateTime(nextStartTime.Year, nextStartTime.Month, nextStartTime.Day,
            23, 50, 0);
        gameEventSystem.AddEvent(new BountyHunterQuestAlienSpawnEvent(new TimeRange(nextStartTime, nextEndTime),
            bodyInfo, knockDoorTimeInterval, knockTime, 1));
    }

    public override void OnMissed() {
        base.OnMissed();
        SpawnNextTime();
    }
    
}
