using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.BodyManagmentSystem;
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
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

[ES3Serializable]
public class AlienDescriptionData {
    public BodyInfo BodyInfo;
    public float Reality;

    public AlienDescriptionData(BodyInfo bodyInfo, float reality) {
        this.BodyInfo = bodyInfo;
        this.Reality = reality;
    }

    public AlienDescriptionData() {
        
    }
}

public class RadioRC : SimpleRC {
    public Action OnRefCleared;
    protected override void OnZeroRef() {
        base.OnZeroRef();
        OnRefCleared?.Invoke();
    }
}

public class Radio : ElectricalApplicance, IPointerClickHandler
{
    public Speaker speaker;
    
    public BodyInfo targetAlien;

    private AudioSource radioNoiseAudioSource;
    [SerializeField] private AnimationCurve radioRealityCurve;
    [SerializeField] private AnimationCurve unrelatedBodyInfoCountWithDay;
    //[SerializeField] private AnimationCurve realAlienDescriptionRepeatTimeWithDay;

    [SerializeField] protected AudioMixerGroup radioNormalBroadcaseAudioMixerGroup;
    [SerializeField] private OpenableUIPanel panel;
   

    private BodyGenerationSystem bodyGenerationSystem;
    private BodyModel bodyModel;
   
    private GameTimeManager gameTimeManager;
    private RadioModel radioModel;

    private RadioRC lowSoundLock = new RadioRC();

    protected float NoiseVolume {
        get {
            return 1 - radioModel.Volume;
        }
    }
    protected override void Awake() {
        base.Awake();
        radioNoiseAudioSource = GetComponent<AudioSource>();
        bodyGenerationSystem = this.GetSystem<BodyGenerationSystem>();
        bodyModel = this.GetModel<BodyModel>();
        gameTimeManager = this.GetSystem<GameTimeManager>();

        this.RegisterEvent<OnRadioEnd>(OnRadioEnd).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnRadioStart>(OnRadioStart).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnConstructDescriptionDatas>(OnConstructDescriptionDatas).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.GetSystem<TelephoneSystem>().State.RegisterOnValueChaned(OnTelephoneStateChange).UnRegisterWhenGameObjectDestroyed(gameObject);
        radioModel = this.GetModel<RadioModel>();
        lowSoundLock.OnRefCleared += OnLowSoundReleased;
        
        radioModel.Volume.RegisterWithInitValue(OnVolumeChange).UnRegisterWhenGameObjectDestroyed(gameObject);

    }

    private void OnVolumeChange(float volume) {
        radioNoiseAudioSource.volume = 1 - volume;
        UpdateSpeakerVolume(true);
    }

    private void UpdateSpeakerVolume(bool isInstant) {
        if (!electricityModel.HasElectricity()) {
            return;
        }
        if (!isInstant) {
            if (lowSoundLock.RefCount > 0) {
                speaker.AudioMixer.DOSetFloat("volume", -45* (1/radioModel.Volume), 1f);
                radioNoiseAudioSource.DOFade(0.1f * NoiseVolume, 1f);
            }
            else {
                radioNoiseAudioSource.DOFade(NoiseVolume, 1f);
                speaker.AudioMixer.DOSetFloat("volume", -20 * (1/radioModel.Volume), 1f);
            }
        }
        else {
            if (lowSoundLock.RefCount > 0) {
                speaker.AudioMixer.SetFloat("volume", -45 * (1/radioModel.Volume));
                radioNoiseAudioSource.volume = 0.1f * NoiseVolume;
            }
            else {
                speaker.AudioMixer.SetFloat("volume", -20 * (1/radioModel.Volume));
                radioNoiseAudioSource.volume = NoiseVolume;
            }
        }
        
    }

    protected override void OnNoElectricity() {
        StopRadio(false);
        radioNoiseAudioSource.volume = 0;
    }

    protected override void OnElectricityRecovered() {
        UpdateSpeakerVolume(false);
    }

    private void OnDestroy() {
        lowSoundLock.OnRefCleared -= OnLowSoundReleased;
    }

    private void OnLowSoundReleased() {
        UpdateSpeakerVolume(false);
    }

    private void OnGameSceneChanged(GameScene arg1, GameScene scene) {
      
        if (scene == GameScene.MainGame) {
            lowSoundLock.Release();
        }
        else {
            lowSoundLock.Retain();
            UpdateSpeakerVolume(false);
        }
    }

    private void OnTelephoneStateChange(TelephoneState oldState, TelephoneState newState) {
        if ((oldState == TelephoneState.Idle || oldState == TelephoneState.Dealing) &&
            (newState == TelephoneState.IncomingCall || newState == TelephoneState.Waiting ||
             newState == TelephoneState.Talking)) {
            lowSoundLock.Retain();
            UpdateSpeakerVolume(false);
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
        if (!electricityModel.HasElectricity()) {
            return;
        }
        RadioSpeak(e.speakContent, e.speakRate, e.speakGender, e.mixer);
        transform.DOShakeRotation(3f, 5, 20, 90, false).SetLoops(-1);

    }

    private void OnRadioEnd(OnRadioEnd e) {
        StopRadio(false);
        
    }

    
    void Start() {
        this.RegisterEvent<OnNewBodyInfoGenerated>(OnBodyInfoGenerated).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.GetModel<GameSceneModel>().GameScene.RegisterOnValueChaned(OnGameSceneChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);

       // this.GetSystem<GameEventSystem>().AddEvent(new RandomStuffRadio(new TimeRange(gameTimeManager.)));
    }





    private void RadioSpeak(string speakText, float speakRate, Gender speakGender, AudioMixerGroup mixer) {
        
        radioModel.IsSpeaking = true;
        if (this) {
            speaker.Speak(speakText, mixer, "Radio", OnSpeakerStop, speakRate, 1f, speakGender);
        }
    }

    private void ConstructDescriptionDatas(List<AlienDescriptionData> descriptionDatas, float radioReality, int day) {

        List<BodyInfo> todayAliens = null;
        if (bodyModel.Aliens.Count > 0) {
            todayAliens = bodyModel.Aliens.Select((info => info.BodyInfo)).ToList();
        }
          

        List<BodyInfo> allPossibleBodyInfos =
            bodyModel.allBodyTimeInfos.Select((info => info.BodyInfo)).ToList();

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
        transform.DOKill(true);
     
    }


    public void OnPointerClick(PointerEventData eventData) {
        panel.Show(0.5f);
    }
}