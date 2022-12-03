using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogKnockEvent : BodyGenerationEvent {
    public DogKnockEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float knockDoorTimeInterval, int knockTime, float eventTriggerChance, Action onEnd, Action onMissed, string overrideAudioClipName = null) : base(startTimeRange, bodyInfo, knockDoorTimeInterval, knockTime, eventTriggerChance, onEnd, onMissed, overrideAudioClipName) {

    }
}
