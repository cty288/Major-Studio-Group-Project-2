using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.TimeSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class DogKnockEvent : BodyGenerationEvent, ICanGetModel, ICanRegisterEvent{
    public override GameEventType GameEventType { get; } = GameEventType.BodyGeneration;
    public override float TriggerChance { get; }


    public DogKnockEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float knockDoorTimeInterval, int knockTime, float eventTriggerChance, Action onEnd, Action onMissed) : base(startTimeRange, bodyInfo, knockDoorTimeInterval, knockTime, eventTriggerChance, onEnd, onMissed, null) {
      
    }
    
    
    protected virtual Func<bool> OnOpen() {
        onClickPeepholeSpeakEnd = false;
        this.GetSystem<ITimeSystem>().AddDelayTask(AudioSystem.Singleton.Play2DSound("dogBark_4", 1, false).clip.length, OnOpenFinish);
        return () => onClickPeepholeSpeakEnd;
    }

    private void OnOpenFinish() {
        onClickPeepholeSpeakEnd = true;
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


    protected override IEnumerator KnockDoorCheck() {
        bodyGenerationModel.CurrentOutsideBody.Value = bodyInfo;
        Debug.Log("Start Knock");

        for (int i = 0; i < knockTime; i++) {
            string clipName =  $"dogBark_{Random.Range(1, 5)}";
            knockAudioSource = AudioSystem.Singleton.Play2DSound(clipName, 1, false);
            yield return new WaitForSeconds(knockAudioSource.clip.length + knockDoorTimeInterval);
        }

        bodyGenerationModel.CurrentOutsideBody.Value = null;
        OnNotOpen();
    }

    
}
