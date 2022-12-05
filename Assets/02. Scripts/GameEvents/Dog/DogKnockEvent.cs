using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

public class DogKnockEvent : BodyGenerationEvent, ICanGetModel, ICanRegisterEvent{
    public override GameEventType GameEventType { get; } = GameEventType.BodyGeneration;
    public override float TriggerChance { get; }
    
    public DogKnockEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float knockDoorTimeInterval, int knockTime, float eventTriggerChance, Action onEnd, Action onMissed, string overrideAudioClipName = null) : base(startTimeRange, bodyInfo, knockDoorTimeInterval, knockTime, eventTriggerChance, onEnd, onMissed, overrideAudioClipName) {
        bodyGenerationModel = this.GetModel<BodyGenerationModel>();
        this.bodyInfo = bodyInfo;
        this.knockDoorTimeInterval = knockDoorTimeInterval;
        this.knockTime = knockTime;
        this.TriggerChance = eventTriggerChance;
        this.onEnd = onEnd;
        this.onMissed = onMissed;
        this.overrideAudioClipName = overrideAudioClipName;

        playerResourceSystem = this.GetSystem<PlayerResourceSystem>();
    }

    protected virtual Func<bool> OnOpen() {
        onClickPeepholeSpeakEnd = false;
        Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>(); 
        speaker.Speak("Bark! Bark!", null, OnDogCome);
        //Dog Bark
        return () => onClickPeepholeSpeakEnd;
    }
    
    private void OnDogCome() {
        this.GetSystem<DogSystem>().SpawnDog();
    }

    protected override void OnNotOpen() {
        DateTime time = gameTimeManager.CurrentTime.Value;
        DateTime nextKnock = time.AddDays(Random.value < 0.5f ? 1 : 2);

        nextKnock = new DateTime(nextKnock.Year, nextKnock.Month, nextKnock.Day, Random.Range(22,24), Random.Range(0, 60), 0);

        gameEventSystem.AddEvent(new DogKnockEvent(this.StartTimeRange, this.bodyInfo,
            this.knockDoorTimeInterval, this.knockTime
            , this.TriggerChance, this.onEnd, this.onMissed));
        
        base.OnNotOpen();
    }
}
