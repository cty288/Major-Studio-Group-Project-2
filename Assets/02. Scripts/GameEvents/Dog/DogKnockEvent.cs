using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.TimeSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public struct OnDogGet {
    public BodyInfo BodyInfo;
}
public class DogKnockEvent : BodyGenerationEvent, ICanGetModel, ICanRegisterEvent{
    [field: ES3Serializable]
    public override GameEventType GameEventType { get; } = GameEventType.BodyGeneration;
    [field: ES3Serializable]
    public override float TriggerChance { get; } = 1;

    public override void OnMissed() {
        OnNotOpen();
    }

    public DogKnockEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float eventTriggerChance) : base(startTimeRange, bodyInfo, eventTriggerChance) {
    }
    
    public DogKnockEvent(): base(){}
    
    
    protected override Func<bool> OnOpen() {
        onClickPeepholeSpeakEnd = false;
       
        this.GetSystem<ITimeSystem>().AddDelayTask(AudioSystem.Singleton.Play2DSound("dogBark_4", 1, false).clip.length, OnOpenFinish);
        LoadCanvas.Singleton.ShowMessage("You adopted this lonely dog.");
        this.SendEvent<OnDogGet>(new OnDogGet() {
            BodyInfo = bodyInfo
        });
        return OnPeepholeSpeakEnd;
    }

    protected bool OnPeepholeSpeakEnd() {
        return onClickPeepholeSpeakEnd;
    }
    
    
    public override void OnEventEnd() {
      
    }

    private void OnOpenFinish() {
        onClickPeepholeSpeakEnd = true;
        LoadCanvas.Singleton.HideMessage();
    }


    protected override void OnNotOpen() {
        DateTime time = gameTimeManager.CurrentTime.Value;
        DateTime nextKnockStart = time.AddDays(Random.value < 0.5f ? 1 : 2);

        nextKnockStart = new DateTime(nextKnockStart.Year, nextKnockStart.Month, nextKnockStart.Day, Random.Range(gameTimeManager.NightTimeStart,24), Random.Range(0, 60), 0);
        DateTime nextKnockEnd = nextKnockStart.AddMinutes(30);

        gameEventSystem.AddEvent(new DogKnockEvent(new TimeRange(nextKnockStart, nextKnockEnd), this.bodyInfo, this.TriggerChance));
        
        base.OnNotOpen();
    }
}
