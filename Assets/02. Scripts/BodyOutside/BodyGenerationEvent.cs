using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.TimeSystem;
using UnityEngine;
using Random = UnityEngine.Random;


public  class BodyGenerationEvent : GameEvent, ICanGetModel, ICanRegisterEvent {
    [field: ES3Serializable]
    public override GameEventType GameEventType { get; } = GameEventType.BodyGeneration;
    [field: ES3Serializable]
    public override float TriggerChance { get; }

    protected BodyGenerationModel bodyGenerationModel;

    protected Coroutine knockDoorCheckCoroutine;
    protected Coroutine nestedKnockDoorCheckCoroutine;
    
    
    [field: ES3Serializable]
    protected BodyInfo bodyInfo;
    
    protected Action onEnd;
    protected Action onMissed;
    
    [field: ES3Serializable]
    protected bool started = false;
   
    [field: ES3Serializable]
    protected bool onClickPeepholeSpeakEnd = false;
    protected PlayerResourceModel playerResourceModel;
    protected ITimeSystem timeSystem;
  
    
    public BodyGenerationEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float eventTriggerChance,
        Action onEnd, Action onMissed) : base(startTimeRange) {
        
        bodyGenerationModel = this.GetModel<BodyGenerationModel>();
        this.bodyInfo = bodyInfo;
        this.TriggerChance = eventTriggerChance;
        this.onEnd = onEnd;
        this.onMissed = onMissed;
       
        
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
        
        if ((currentTime.Hour == 23 && currentTime.Minute >= 58) || gameStateModel.GameState.Value == GameState.End) {
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

    protected virtual Func<bool> OnOpen() {
        onClickPeepholeSpeakEnd = false;
        Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
        if (bodyInfo.IsAlien) {
            LoadCanvas.Singleton.ShowImage(0, 0.2f);
            List<string> messages = new List<string>() {
                "goOD dAy sIR. buT iT'S yOuR tiME!",
                "hI, Hi! iT IS yOur tiMe!",
                "I nEeD yOU cLotHEs!",
                "YOur bRaIN iS MiNE!",
                "YOuR TimE IS oVeR!"
            };
            speaker.Speak(messages[Random.Range(0, messages.Count)], AudioMixerList.Singleton.AudioMixerGroups[4], "???", OnAlienClickedOutside);
        }
        else {
            
            LoadCanvas.Singleton.ShowImage(1, 0.2f);
            List<string> messages = new List<string>() {
                "Delivery service! Take care!",
                "Here's the food for you today. Take care!",
                "Hey, I brought you some foods! Take care!"
            };

            IVoiceTag voiceTag = bodyInfo.VoiceTag;

            speaker.Speak(messages[Random.Range(0, messages.Count)],
                AudioMixerList.Singleton.AlienVoiceGroups[bodyInfo.VoiceTag.VoiceIndex],
                "Deliver", OnDelivererClickedOutside,
                voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
        }
        return () => onClickPeepholeSpeakEnd;
    }

    private void OnDelivererClickedOutside() {
        this.GetModel<PlayerResourceModel>().AddFood(Random.Range(1, 3));
       // this.SendEvent<OnShowFood>();
        timeSystem.AddDelayTask(1f, () => {
            onClickPeepholeSpeakEnd = true;
            LoadCanvas.Singleton.HideImage(0.5f);
        });
    }

    protected void OnAlienClickedOutside() {
        DogSystem dogSystem = this.GetSystem<DogSystem>();
        
        
        if (dogSystem.HaveDog && dogSystem.isDogAlive) {
            float clipLength = AudioSystem.Singleton.Play2DSound("dogBark_4").clip.length;
            timeSystem.AddDelayTask(clipLength, () => {
                LoadCanvas.Singleton.HideImage(1f);
                AudioSystem.Singleton.Play2DSound("dog_die");
                LoadCanvas.Singleton.ShowMessage("Your friend is gone...");
                dogSystem.KillDog();
                timeSystem.AddDelayTask(2f, () => {
                    LoadCanvas.Singleton.HideMessage();
                    timeSystem.AddDelayTask(1f, () => {
                        onClickPeepholeSpeakEnd = true;
                    });                    
                });
            });
        } else if (playerResourceModel.HasEnoughResource<BulletGoods>(1) && playerResourceModel.HasEnoughResource<GunResource>(1)) {
            playerResourceModel.RemoveResource<BulletGoods>(1);
            float clipLength = AudioSystem.Singleton.Play2DSound("gun_fire").clip.length;

            timeSystem.AddDelayTask(1f, () => {
                LoadCanvas.Singleton.HideImage(1f);
                LoadCanvas.Singleton.ShowMessage("You shot the creature and it fleed.\n\nBullet - 1");
                timeSystem.AddDelayTask(2f, () => {
                    LoadCanvas.Singleton.HideMessage();
                    timeSystem.AddDelayTask(1f, () => {
                        onClickPeepholeSpeakEnd = true;
                    });
                });
            });
        }else {
            LoadCanvas.Singleton.HideImage(1f);
            DieCanvas.Singleton.Show("You are killed by the creature!");
            this.GetModel<GameStateModel>().GameState.Value = GameState.End;
            this.GetSystem<BodyGenerationSystem>().StopCurrentBody();
        }
    }

    public override void OnEnd() {
        if (knockDoorCheckCoroutine != null) {
            CoroutineRunner.Singleton.StopCoroutine(knockDoorCheckCoroutine);
            knockDoorCheckCoroutine = null;
        }
        
        if (nestedKnockDoorCheckCoroutine != null) {
            CoroutineRunner.Singleton.StopCoroutine(nestedKnockDoorCheckCoroutine);
            nestedKnockDoorCheckCoroutine = null;
        }
        
        
        onEnd?.Invoke();
        UnregisterListeners();
        this.UnRegisterEvent<OnOutsideBodyClicked>(OnOutsideBodyClicked);
    }

    private void UnregisterListeners() {
        onEnd = null;
        onMissed = null;
        
    }

    public override void OnMissed() {
       onMissed?.Invoke();
       UnregisterListeners();
       
    }


    protected virtual IEnumerator KnockDoorCheck() {
        Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
        bodyGenerationModel.CurrentOutsideBody.Value = bodyInfo;
      
        nestedKnockDoorCheckCoroutine = CoroutineRunner.Singleton.StartCoroutine(bodyInfo.KnockBehavior?.OnKnockDoor(speaker,
            bodyInfo.VoiceTag));
        yield return nestedKnockDoorCheckCoroutine;
        
        knockDoorCheckCoroutine = null;
        bodyGenerationModel.CurrentOutsideBody.Value = null;
        bodyInfo.KnockBehavior?.OnStopKnock();
        OnNotOpen();
    }
}
