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
    protected AudioSource barkAudioSource;

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
    
    public override EventState OnUpdate() {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        
        if ((currentTime.Hour == 23 && currentTime.Minute >= 58) || gameStateModel.GameState.Value == GameState.End) {
            if (knockDoorCheckCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(knockDoorCheckCoroutine);
                knockDoorCheckCoroutine = null;
            }
            bodyGenerationModel.CurrentOutsideBody.Value = null;
            OnNotOpen();
            return EventState.End;
        }

        if (!started) {
            if (bodyGenerationModel.CurrentOutsideBody.Value != null) {
                OnNotOpen();
                return EventState.End;
            }
            started = true;
            knockDoorCheckCoroutine = CoroutineRunner.Singleton.StartCoroutine(DogBark());
        }

        return (bodyGenerationModel.CurrentOutsideBody.Value == null && !bodyGenerationModel.CurrentOutsideBodyConversationFinishing) ? EventState.End : EventState.Running;
    }

    protected virtual Func<bool> OnOpen() {
        onClickPeepholeSpeakEnd = false;
        Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>(); 
        //speaker.Speak("Bark! Bark!", null, OnDogCome);
        //Dog Bark
        knockDoorCheckCoroutine = CoroutineRunner.Singleton.StartCoroutine(DogBark());
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

    private IEnumerator DogBark()
    {
        bodyGenerationModel.CurrentOutsideBody.Value = bodyInfo;
        Debug.Log("Start Bark");
        
        for (int i = 0; i < knockTime; i++) {
            string clipName = overrideAudioClipName;
            if (String.IsNullOrEmpty(overrideAudioClipName)) {
                clipName = $"dogBark_{Random.Range(1, 5)}";
            }
           
            barkAudioSource = AudioSystem.Singleton.Play2DSound(clipName, 1, false);
            yield return new WaitForSeconds(barkAudioSource.clip.length + knockDoorTimeInterval);
        }

        bodyGenerationModel.CurrentOutsideBody.Value = null;
        OnNotOpen();
    }
}
