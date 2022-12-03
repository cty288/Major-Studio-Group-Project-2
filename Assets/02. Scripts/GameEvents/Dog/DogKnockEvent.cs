using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DogKnockEvent : BodyGenerationEvent {
    public DogKnockEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float knockDoorTimeInterval, int knockTime, float eventTriggerChance, Action onEnd, Action onMissed, string overrideAudioClipName = null) : base(startTimeRange, bodyInfo, knockDoorTimeInterval, knockTime, eventTriggerChance, onEnd, onMissed, overrideAudioClipName) {

    }

    protected override Func<bool> OnOpen() {
        return base.OnOpen();
    }

    protected override void OnNotOpen() {
        DateTime time = gameTimeManager.CurrentTime.Value;
        DateTime tomorrow = time.AddDays(1);
        tomorrow = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, Random.Range(22,24), Random.Range(0, 60), 0);
        
        base.OnNotOpen();
    }
}
