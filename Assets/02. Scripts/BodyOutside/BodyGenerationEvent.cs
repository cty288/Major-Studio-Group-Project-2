using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.TimeSystem;
using UnityEngine;
using Random = UnityEngine.Random;


public abstract  class BodyGenerationEvent : GameEvent, ICanGetModel, ICanRegisterEvent {
    [field: ES3Serializable]
    public override GameEventType GameEventType { get; } = GameEventType.BodyGeneration;
    [field: ES3Serializable]
    public override float TriggerChance { get; }

    protected BodyGenerationModel bodyGenerationModel;

    protected Coroutine knockDoorCheckCoroutine;
    protected Coroutine nestedKnockDoorCheckCoroutine;
    
    
    [field: ES3Serializable]
    protected BodyInfo bodyInfo;
    
 
    
    [field: ES3Serializable]
    protected bool started = false;
   
    [field: ES3Serializable]
    protected bool onClickPeepholeSpeakEnd = false;
    protected PlayerResourceModel playerResourceModel;
    protected ITimeSystem timeSystem;
    protected bool interacted = false;

    public BodyGenerationEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float eventTriggerChance) : base(startTimeRange) {
        
        bodyGenerationModel = this.GetModel<BodyGenerationModel>();
        this.bodyInfo = bodyInfo;
        this.TriggerChance = eventTriggerChance;
     
       
        
        playerResourceModel = this.GetModel<PlayerResourceModel>();
        timeSystem = this.GetSystem<ITimeSystem>();
    }

    public BodyGenerationEvent() : base() {
        bodyGenerationModel = this.GetModel<BodyGenerationModel>();
        playerResourceModel = this.GetModel<PlayerResourceModel>();
        timeSystem = this.GetSystem<ITimeSystem>();
    }

    public override void OnStart() {
        this.RegisterEvent<OnOutsideBodyClicked>(OnOutsideBodyClicked);
    }

    private void OnOutsideBodyClicked(OnOutsideBodyClicked e) {
        if (e.BodyInfo.ID == bodyInfo.ID) {
            interacted = true;
            if (knockDoorCheckCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(knockDoorCheckCoroutine);
                knockDoorCheckCoroutine = null;
            }
            
            if (nestedKnockDoorCheckCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(nestedKnockDoorCheckCoroutine);
                nestedKnockDoorCheckCoroutine = null;
            }
            
            LoadCanvas.Singleton.LoadUntil(OnOpen, OnFinishOutsideBodyInteraction);
            bodyInfo.KnockBehavior?.OnStopKnock();
        }

        this.UnRegisterEvent<OnOutsideBodyClicked>(OnOutsideBodyClicked);
    }

    private void OnFinishOutsideBodyInteraction() {
        this.GetModel<GameSceneModel>().GameScene.Value = GameScene.MainGame;
        this.GetSystem<BodyGenerationSystem>().StopCurrentBody();
    }

    public override EventState OnUpdate() {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        
        if ((currentTime.Hour == 23 && currentTime.Minute >= 58 && !interacted) || gameStateModel.GameState.Value == GameState.End) {
            if (knockDoorCheckCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(knockDoorCheckCoroutine);
                knockDoorCheckCoroutine = null;
            }
            
            if (nestedKnockDoorCheckCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(nestedKnockDoorCheckCoroutine);
                nestedKnockDoorCheckCoroutine = null;
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
            knockDoorCheckCoroutine = CoroutineRunner.Singleton.StartCoroutine(KnockDoorCheck());
        }

        return (bodyGenerationModel.CurrentOutsideBody.Value == null && !bodyGenerationModel.CurrentOutsideBodyConversationFinishing) ? EventState.End : EventState.Running;
    }

    protected virtual void OnNotOpen() {

    }

    protected abstract Func<bool> OnOpen();

   

    public override void OnEnd() {
        if (knockDoorCheckCoroutine != null) {
            CoroutineRunner.Singleton.StopCoroutine(knockDoorCheckCoroutine);
            knockDoorCheckCoroutine = null;
        }
        
        if (nestedKnockDoorCheckCoroutine != null) {
            CoroutineRunner.Singleton.StopCoroutine(nestedKnockDoorCheckCoroutine);
            nestedKnockDoorCheckCoroutine = null;
        }
        
        
        this.UnRegisterEvent<OnOutsideBodyClicked>(OnOutsideBodyClicked);
        OnEventEnd();
    }

    public abstract void OnEventEnd();


    protected virtual IEnumerator KnockDoorCheck() {
        Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
        bodyGenerationModel.CurrentOutsideBody.Value = bodyInfo;
      
        nestedKnockDoorCheckCoroutine = CoroutineRunner.Singleton.StartCoroutine(bodyInfo.KnockBehavior?.OnKnockDoor(speaker,
            bodyInfo.VoiceTag, bodyInfo.IsAlien));
        yield return nestedKnockDoorCheckCoroutine;
        
        knockDoorCheckCoroutine = null;
        bodyGenerationModel.CurrentOutsideBody.Value = null;
        bodyInfo.KnockBehavior?.OnStopKnock();
        OnNotOpen();
    }
}
