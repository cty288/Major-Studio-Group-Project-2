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

}
public class DogKnockEvent : BodyGenerationEvent, ICanGetModel, ICanRegisterEvent{
    public override GameEventType GameEventType { get; } = GameEventType.BodyGeneration;
    public override float TriggerChance { get; } = 1;

    public DogKnockEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float eventTriggerChance, Action onEnd, Action onMissed) : base(startTimeRange, bodyInfo, eventTriggerChance, onEnd, onMissed) {
    }
    
    
    protected override Func<bool> OnOpen() {
        onClickPeepholeSpeakEnd = false;
       
        this.GetSystem<ITimeSystem>().AddDelayTask(AudioSystem.Singleton.Play2DSound("dogBark_4", 1, false).clip.length, OnOpenFinish);
        LoadCanvas.Singleton.ShowMessage("You adopted this lonely dog.");
        this.SendEvent<OnDogGet>();
        return () => onClickPeepholeSpeakEnd;
    }

    private void OnOpenFinish() {
        onClickPeepholeSpeakEnd = true;
        LoadCanvas.Singleton.HideMessage();
    }

    public static BodyInfo GenerateDog(float knockDoorTimeInterval, int knockTime) {
        HeightType height =HeightType.Short;
        AlienBodyPartInfo leg = AlienBodyPartCollections.Singleton.SpecialBodyPartPrefabs.HeightSubCollections[1]
            .ShadowBodyPartPrefabs.HumanTraitPartsPrefabs[0].GetComponent<AlienBodyPartInfo>();

        return BodyInfo.GetBodyInfo(leg, null, null, height, null,
            new DogKnockBehavior(knockDoorTimeInterval, knockTime, null), BodyPartDisplayType.Shadow, false);

    }
    protected override void OnNotOpen() {
        DateTime time = gameTimeManager.CurrentTime.Value;
        DateTime nextKnockStart = time.AddDays(Random.value < 0.5f ? 1 : 2);

        nextKnockStart = new DateTime(nextKnockStart.Year, nextKnockStart.Month, nextKnockStart.Day, Random.Range(22,24), Random.Range(0, 60), 0);
        DateTime nextKnockEnd = nextKnockStart.AddMinutes(30);

        gameEventSystem.AddEvent(new DogKnockEvent(new TimeRange(nextKnockStart, nextKnockEnd), this.bodyInfo, this.TriggerChance, this.onEnd, this.onMissed));
        
        base.OnNotOpen();
    }
}
