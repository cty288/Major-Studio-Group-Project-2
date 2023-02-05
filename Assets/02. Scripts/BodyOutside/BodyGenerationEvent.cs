using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.TimeSystem;
using UnityEngine;
using Random = UnityEngine.Random;


public  class BodyGenerationEvent : GameEvent, ICanGetModel, ICanRegisterEvent {
    public override GameEventType GameEventType { get; } = GameEventType.BodyGeneration;
    public override float TriggerChance { get; }

    protected BodyGenerationModel bodyGenerationModel;

    protected Coroutine knockDoorCheckCoroutine;
    protected BodyInfo bodyInfo;
    protected float knockDoorTimeInterval;
    protected int knockTime;
    protected Action onEnd;
    protected Action onMissed;
    protected bool started = false;
    protected string overrideAudioClipName;
    protected bool onClickPeepholeSpeakEnd = false;
    protected PlayerResourceSystem playerResourceSystem;
    protected ITimeSystem timeSystem;
    protected AudioSource knockAudioSource;
    
    public BodyGenerationEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float knockDoorTimeInterval, int knockTime, float eventTriggerChance,
        Action onEnd, Action onMissed, string overrideAudioClipName = null) : base(startTimeRange) {
        bodyGenerationModel = this.GetModel<BodyGenerationModel>();
        this.bodyInfo = bodyInfo;
        this.knockDoorTimeInterval = knockDoorTimeInterval;
        this.knockTime = knockTime;
        this.TriggerChance = eventTriggerChance;
        this.onEnd = onEnd;
        this.onMissed = onMissed;
        this.overrideAudioClipName = overrideAudioClipName;

        playerResourceSystem = this.GetSystem<PlayerResourceSystem>();
        timeSystem = this.GetSystem<ITimeSystem>();
    }

    public override void OnStart() {
        this.RegisterEvent<OnOutsideBodyClicked>(OnOutsideBodyClicked);
    }

    private void OnOutsideBodyClicked(OnOutsideBodyClicked e) {
        if (e.BodyInfo == bodyInfo) {
            if (knockDoorCheckCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(knockDoorCheckCoroutine);
                knockDoorCheckCoroutine = null;
              
            }
            LoadCanvas.Singleton.LoadUntil(OnOpen, OnFinishOutsideBodyInteraction);
            if (knockAudioSource) {
                knockAudioSource.Stop();
            }
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
                "Here��s the food for you today. Take care!",
                "Hey, I brought you some foods! Take care!"
            };
            speaker.Speak(messages[Random.Range(0, messages.Count)], null, "Deliver", OnDelivererClickedOutside);
        }
        return () => onClickPeepholeSpeakEnd;
    }

    private void OnDelivererClickedOutside() {
        this.GetSystem<PlayerResourceSystem>().AddFood(Random.Range(1, 3));
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
        } else if (playerResourceSystem.HasEnoughResource<BulletGoods>(1) && playerResourceSystem.HasEnoughResource<GunResource>(1)) {
            playerResourceSystem.RemoveResource<BulletGoods>(1);
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
        bodyGenerationModel.CurrentOutsideBody.Value = bodyInfo;
        Debug.Log("Start Knock");
        
        for (int i = 0; i < knockTime; i++) {
            string clipName = overrideAudioClipName;
            if (String.IsNullOrEmpty(overrideAudioClipName)) {
                 clipName = $"knock_{Random.Range(1, 8)}";
            }
           
            knockAudioSource = AudioSystem.Singleton.Play2DSound(clipName, 1, false);
            yield return new WaitForSeconds(knockAudioSource.clip.length + knockDoorTimeInterval);
        }

        bodyGenerationModel.CurrentOutsideBody.Value = null;
        OnNotOpen();
    }
}
