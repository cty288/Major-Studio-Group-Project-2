using System;
using System.Collections;
using System.Collections.Generic;
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


    public DogKnockEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float knockDoorTimeInterval, int knockTime, float eventTriggerChance, Action onEnd, Action onMissed) : base(startTimeRange, bodyInfo, knockDoorTimeInterval, knockTime, eventTriggerChance, onEnd, onMissed, null) {
      
    }
    
    
    protected override Func<bool> OnOpen() {
        onClickPeepholeSpeakEnd = false;
        if (knockAudioSource) {
            knockAudioSource.Stop();
        }
        this.GetSystem<ITimeSystem>().AddDelayTask(AudioSystem.Singleton.Play2DSound("dogBark_4", 1, false).clip.length, OnOpenFinish);
        LoadCanvas.Singleton.ShowMessage("You adopted this lonely dog.");
        this.SendEvent<OnDogGet>();
        return () => onClickPeepholeSpeakEnd;
    }

    private void OnOpenFinish() {
        onClickPeepholeSpeakEnd = true;
        LoadCanvas.Singleton.HideMessage();
    }

    public static BodyInfo GenerateDog() {
        HeightType height =HeightType.Short;
        AlienBodyPartInfo leg = AlienBodyPartCollections.Singleton.SpecialBodyPartPrefabs.HeightSubCollections[1]
            .ShadowBodyPartPrefabs.HumanTraitPartsPrefabs[0].GetComponent<AlienBodyPartInfo>();

        return BodyInfo.GetBodyInfo(leg, null, null, height, Gender.MALE, BodyPartDisplayType.Shadow, false);

    }
    protected override void OnNotOpen() {
        DateTime time = gameTimeManager.CurrentTime.Value;
        DateTime nextKnockStart = time.AddDays(Random.value < 0.5f ? 1 : 2);

        nextKnockStart = new DateTime(nextKnockStart.Year, nextKnockStart.Month, nextKnockStart.Day, Random.Range(22,24), Random.Range(0, 60), 0);
        DateTime nextKnockEnd = nextKnockStart.AddMinutes(30);

        gameEventSystem.AddEvent(new DogKnockEvent(new TimeRange(nextKnockStart, nextKnockEnd), this.bodyInfo,
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
