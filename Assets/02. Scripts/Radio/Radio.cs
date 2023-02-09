using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using Crosstales;
using Crosstales.RTVoice.Model.Enum;
using DG.Tweening;
using UnityEngine;
using MikroFramework.Event;
using MikroFramework.Architecture;
using MikroFramework;
using MikroFramework.AudioKit;
using MikroFramework.Singletons;
using MikroFramework.Utilities;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AlienDescriptionData {
    public BodyInfo BodyInfo;
    public float Reality;

    public AlienDescriptionData(BodyInfo bodyInfo, float reality) {
        this.BodyInfo = bodyInfo;
        this.Reality = reality;
    }
}

public class RadioRC : SimpleRC {
    public Action OnRefCleared;
    protected override void OnZeroRef() {
        base.OnZeroRef();
        OnRefCleared?.Invoke();
    }
}

public class Radio : ElectricalApplicance
{
    public Speaker speaker;
    
    public BodyInfo targetAlien;

    private AudioSource radioOpenAudioSource;
    [SerializeField] private AnimationCurve radioRealityCurve;
    [SerializeField] private AnimationCurve unrelatedBodyInfoCountWithDay;
    //[SerializeField] private AnimationCurve realAlienDescriptionRepeatTimeWithDay;

    [SerializeField] protected AudioMixerGroup radioNormalBroadcaseAudioMixerGroup;
   

    private BodyGenerationSystem bodyGenerationSystem;
    private BodyManagmentSystem bodyManagmentSystem;
   
    private GameTimeManager gameTimeManager;
    private RadioModel radioModel;

    private RadioRC lowSoundLock = new RadioRC();
    protected override void Awake() {
        base.Awake();
        radioOpenAudioSource = GetComponent<AudioSource>();
        bodyGenerationSystem = this.GetSystem<BodyGenerationSystem>();
        bodyManagmentSystem = this.GetSystem<BodyManagmentSystem>();
        gameTimeManager = this.GetSystem<GameTimeManager>();

        this.RegisterEvent<OnRadioEnd>(OnRadioEnd).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnRadioStart>(OnRadioStart).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnConstructDescriptionDatas>(OnConstructDescriptionDatas).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.GetSystem<TelephoneSystem>().State.RegisterOnValueChaned(OnTelephoneStateChange).UnRegisterWhenGameObjectDestroyed(gameObject);
        radioModel = this.GetModel<RadioModel>();
        lowSoundLock.OnRefCleared += OnLowSoundReleased;

    }

    protected override void OnNoElectricity() {
        StopRadio(false);
    }

    protected override void OnElectricityRecovered() {
        
    }

    private void OnDestroy() {
        lowSoundLock.OnRefCleared -= OnLowSoundReleased;
    }

    private void OnLowSoundReleased() {
        radioOpenAudioSource.DOFade(0.02f, 1f);
        speaker.AudioMixer.DOSetFloat("volume", -20, 1f);
    }

    private void OnGameSceneChanged(GameScene arg1, GameScene scene) {
      
        if (scene == GameScene.MainGame) {
            lowSoundLock.Release();
        }
        else {
            lowSoundLock.Retain();
            speaker.AudioMixer.DOSetFloat("volume", -45, 1f);
            radioOpenAudioSource.DOFade(0.01f, 1f);
        }
    }

    private void OnTelephoneStateChange(TelephoneState oldState, TelephoneState newState) {
        if ((oldState == TelephoneState.Idle || oldState == TelephoneState.Dealing) &&
            (newState == TelephoneState.IncomingCall || newState == TelephoneState.Waiting ||
             newState == TelephoneState.Talking)) {
            lowSoundLock.Retain();
            speaker.AudioMixer.DOSetFloat("volume", -45, 1f);
            radioOpenAudioSource.DOFade(0.01f, 1f);
        }
        else if ((oldState == TelephoneState.IncomingCall || oldState == TelephoneState.Waiting ||
                 oldState == TelephoneState.Talking) &&
                (newState == TelephoneState.Idle || newState == TelephoneState.Dealing)) {
            lowSoundLock.Release();
        }
    }

    private void OnConstructDescriptionDatas(OnConstructDescriptionDatas obj) {
        int day = this.GetSystem<GameTimeManager>().Day;
        float radioReality = radioRealityCurve.Evaluate(day);
        ConstructDescriptionDatas(radioModel.DescriptionDatas, radioReality, day);
    }

    private void OnRadioStart(OnRadioStart e) {
        if (!electricitySystem.HasElectricity()) {
            return;
        }
        RadioSpeak(e.speakContent, e.speakRate, e.speakGender, e.mixer);
        transform.DOShakeRotation(3f, 5, 20, 90, false).SetLoops(-1);

    }

    private void OnRadioEnd(OnRadioEnd e) {
        StopRadio(true);
        
    }

    
    void Start() {
        this.RegisterEvent<OnNewBodyInfoGenerated>(OnBodyInfoGenerated).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.GetModel<GameSceneModel>().GameScene.RegisterOnValueChaned(OnGameSceneChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);

       // this.GetSystem<GameEventSystem>().AddEvent(new RandomStuffRadio(new TimeRange(gameTimeManager.)));
    }





    private void RadioSpeak(string speakText, float speakRate, Gender speakGender, AudioMixerGroup mixer) {
        if (lowSoundLock.RefCount == 0) {
            radioOpenAudioSource.DOFade(0.06f, 1f);
        }
        radioModel.IsSpeaking = true;
      
        this.Delay(1 + Random.Range(2f, 5f), () => {
            if (this) {
                if (lowSoundLock.RefCount == 0) {
                    radioOpenAudioSource.DOFade(0.02f, 1f);
                }

               
                speaker.Speak(speakText, mixer, "Radio", OnSpeakerStop, speakRate, 1f, speakGender);
            }
        });
    }

    private void ConstructDescriptionDatas(List<AlienDescriptionData> descriptionDatas, float radioReality, int day) {

        List<BodyInfo> todayAliens = null;
        if (bodyManagmentSystem.Aliens.Count > 0) {
            todayAliens = bodyManagmentSystem.Aliens.Select((info => info.BodyInfo)).ToList();
        }
          

        List<BodyInfo> allPossibleBodyInfos =
            bodyManagmentSystem.allBodyTimeInfos.Select((info => info.BodyInfo)).ToList();

        allPossibleBodyInfos.CTShuffle();
        if (todayAliens != null) {
            foreach (BodyInfo bodyInfo in allPossibleBodyInfos) {
                descriptionDatas.Add(new AlienDescriptionData(bodyInfo, radioReality));
            }
        }
       

        for (int i = 0; i < unrelatedBodyInfoCountWithDay.Evaluate(day); i++) {
            if (Random.Range(0, 2) < 1) {
                descriptionDatas.Add(
                    new AlienDescriptionData(BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, false, false,
                            new NormalKnockBehavior(3, Random.Range(3,7),
                                null)),
                        radioReality));
            }
            else {
                BodyInfo info = allPossibleBodyInfos[i];
                descriptionDatas.Add(new AlienDescriptionData(info, radioReality));
            }
        }
        descriptionDatas.CTShuffle();

    }

    private void OnBodyInfoGenerated(OnNewBodyInfoGenerated e) {
        int day = this.GetSystem<GameTimeManager>().Day;
        float radioReality = radioRealityCurve.Evaluate(day);
        
        radioModel.DescriptionDatas.Clear();
       ConstructDescriptionDatas(radioModel.DescriptionDatas, radioReality, day);

       DateTime currentTime = gameTimeManager.CurrentTime.Value;
       if (day == 1) {
            AlienDescriptionData descriptionData = radioModel.DescriptionDatas[0];
            radioModel.DescriptionDatas.RemoveAt(0);


            GameEventSystem eventSystem = this.GetSystem<GameEventSystem>();
            eventSystem.AddEvent(new DailyBodyRadio(
                new TimeRange(currentTime + new TimeSpan(0, 10, 0), currentTime + new TimeSpan(0, 20, 0)),
                AlienDescriptionFactory.GetRadioDescription(descriptionData.BodyInfo, descriptionData.Reality),
                Random.Range(0.85f, 1.2f), Random.Range(0, 2) == 0 ? Gender.MALE : Gender.FEMALE,
                radioNormalBroadcaseAudioMixerGroup));

            eventSystem.AddEvent(new RandomStuffRadio(
                new TimeRange(currentTime + new TimeSpan(0, Random.Range(30, 60), 0)),
                RadioRandomStuff.Singleton.GetNextRandomRadio()));

       }
    }


    private void StopRadio(bool corrupt) {
       
        if (corrupt) {
            speaker.Corrupt(5, () => {
                speaker.Stop();
            });
        }
        else {
            speaker.Stop();
        }
      
       
    }

    private void OnSpeakerStop() {
        radioModel.IsSpeaking = false;
        radioOpenAudioSource.DOFade(0, 1f);
        transform.DOKill(true);
     
    }

   
}