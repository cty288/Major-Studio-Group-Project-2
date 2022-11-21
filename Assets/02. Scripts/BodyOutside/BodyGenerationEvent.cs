using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using UnityEngine;
using Random = UnityEngine.Random;


public  class BodyGenerationEvent : GameEvent, ICanGetModel {
    public override GameEventType GameEventType { get; } = GameEventType.BodyGeneration;
    public override float TriggerChance { get; }

    private BodyGenerationModel bodyGenerationModel;

    private Coroutine knockDoorCheckCoroutine;
    private BodyInfo bodyInfo;
    private float knockDoorTimeInterval;
    private int knockTime;
    private Action onEnd;
    private Action onMissed;
    private bool started = false;
    public BodyGenerationEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float knockDoorTimeInterval, int knockTime, float eventTriggerChance,
        Action onEnd, Action onMissed) : base(startTimeRange) {
        bodyGenerationModel = this.GetModel<BodyGenerationModel>();
        this.bodyInfo = bodyInfo;
        this.knockDoorTimeInterval = knockDoorTimeInterval;
        this.knockTime = knockTime;
        this.TriggerChance = eventTriggerChance;
        this.onEnd = onEnd;
        this.onMissed = onMissed;
    }

    public override void OnStart() {
        
    }

    public override EventState OnUpdate() {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        if ((currentTime.Hour == 23 && currentTime.Minute >= 58) || gameStateModel.GameState.Value == GameState.End) {
            if (knockDoorCheckCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(knockDoorCheckCoroutine);
                knockDoorCheckCoroutine = null;
            }
            bodyGenerationModel.CurrentOutsideBody.Value = null;
            return EventState.End;
        }

        if (!started) {
            started = true;
            knockDoorCheckCoroutine = CoroutineRunner.Singleton.StartCoroutine(KnockDoorCheck());
        }

        if (bodyGenerationModel.CurrentOutsideBodyConversationFinishing) {
            if(knockDoorCheckCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(knockDoorCheckCoroutine);
                knockDoorCheckCoroutine = null;
            }
        }
      
        return (bodyGenerationModel.CurrentOutsideBody.Value == null && !bodyGenerationModel.CurrentOutsideBodyConversationFinishing) ? EventState.End : EventState.Running;
    }

    public override void OnEnd() {
        if (knockDoorCheckCoroutine != null) {
            CoroutineRunner.Singleton.StopCoroutine(knockDoorCheckCoroutine);
            knockDoorCheckCoroutine = null;
        }
        onEnd?.Invoke();
        UnregisterListeners();
    }

    private void UnregisterListeners() {
        onEnd = null;
        onMissed = null;
    }

    public override void OnMissed() {
       onMissed?.Invoke();
       UnregisterListeners();
    }


    private IEnumerator KnockDoorCheck() {
        bodyGenerationModel.CurrentOutsideBody.Value = bodyInfo;
        Debug.Log("Start Knock");
        
        for (int i = 0; i < knockTime; i++) {
            string clipName = $"knock_{Random.Range(1, 8)}";
            AudioSource source = AudioSystem.Singleton.Play2DSound(clipName, 1, false);
            yield return new WaitForSeconds(source.clip.length + knockDoorTimeInterval);
        }

        bodyGenerationModel.CurrentOutsideBody.Value = null;
    }
}
