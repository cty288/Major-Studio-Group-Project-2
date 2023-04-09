using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.Radio;
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
using TMPro;
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
    //public Speaker speaker;
    
    public BodyInfo targetAlien;

    private AudioSource radioNoiseAudioSource;
    [SerializeField] private AnimationCurve radioRealityCurve;
    [SerializeField] private AnimationCurve unrelatedBodyInfoCountWithDay;
    //[SerializeField] private AnimationCurve realAlienDescriptionRepeatTimeWithDay;

    [SerializeField] protected AudioMixerGroup radioNormalBroadcaseAudioMixerGroup;
    [SerializeField] private OpenableUIPanel panel;
   

    private BodyGenerationSystem bodyGenerationSystem;
    private BodyModel bodyModel;
    protected TMP_Text mouseHoverHint;
   
    private GameTimeManager gameTimeManager;
    private RadioModel radioModel;

    private RadioRC lowSoundLock = new RadioRC();

    [SerializeField] private GameObject speakerPrefab;
    protected Dictionary<RadioChannel, RadioContentPlayer> activePlayers = new Dictionary<RadioChannel, RadioContentPlayer>();

    protected float NoiseVolume {
        get {
            return 1 - radioModel.RelativeVolume;
        }
    }
    protected override void Awake() {
        base.Awake();
        radioNoiseAudioSource = GetComponent<AudioSource>();
        bodyGenerationSystem = this.GetSystem<BodyGenerationSystem>();
        bodyModel = this.GetModel<BodyModel>();
        gameTimeManager = this.GetSystem<GameTimeManager>();

        this.RegisterEvent<OnRadioEnd>(OnRadioEnd).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnRadioProgramStart>(OnRadioStart).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnConstructDescriptionDatas>(OnConstructDescriptionDatas).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.GetSystem<TelephoneSystem>().State.RegisterOnValueChaned(OnTelephoneStateChange).UnRegisterWhenGameObjectDestroyed(gameObject);
        
        radioModel = this.GetModel<RadioModel>();
        InitializePlayers();

       // Crosstales.RTVoice.Speaker.Instance.Caching = false;
        lowSoundLock.OnRefCleared += OnLowSoundReleased;
        mouseHoverHint = transform.Find("RadioCanvas/Hint").GetComponent<TMP_Text>();
        radioModel.IsOn.RegisterWithInitValue(OnRadioOnOffChange).UnRegisterWhenGameObjectDestroyed(gameObject);
        
        radioModel.RelativeVolume.RegisterWithInitValue(OnVolumeChange).UnRegisterWhenGameObjectDestroyed(gameObject);
        radioModel.CurrentChannel.RegisterWithInitValue(OnChannelChange).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);

    }

    private void OnNewDay(OnNewDay e) {
        radioModel.IsOn.Value = true;
    }

    private void InitializePlayers() {
        foreach (RadioChannel channel in Enum.GetValues(typeof(RadioChannel))) {
            activePlayers.Add(channel, null);
        }
    }

    private void OnRadioEnd(OnRadioEnd e) {
        if (activePlayers.ContainsKey(e.channel)) {
            activePlayers[e.channel]?.Stop();
            DestroyPlayer(e.channel);
            radioModel.SetIsSpeaking(e.channel, false);
        }
    }
    
    protected bool IsPlayerPlaying(RadioChannel channel) {
        return activePlayers[channel] != null && activePlayers[channel].IsPlaying();
    }

    private void OnChannelChange(RadioChannel oldChannel, RadioChannel newChannel) {
        if (activePlayers.ContainsKey(oldChannel)) {
            //speakers[oldChannel].SetOverallVolume(0);
            activePlayers[oldChannel]?.Mute(true);
        }
        
        if (activePlayers.ContainsKey(newChannel)) {
            //speakers[newChannel].SetOverallVolume(1);
            activePlayers[newChannel]?.Mute(false);
            if (IsPlayerPlaying(newChannel) && radioModel.IsOn.Value && electricityModel.HasElectricity()) {
                transform.DOShakeRotation(3f, 5, 20, 90, false).SetLoops(-1);
            }
            else {
                transform.DOKill();
            }
            
        }
        else {
            transform.DOKill();
        }
        
        
    }

    private RadioContentPlayer SpawnPlayer(RadioChannel channel, RadioContentType contentType) {
        if (activePlayers[channel] != null) return null;

        RadioContentPlayer player = RadioContentPlayerFactory.Singleton.SpawnPlayerPrefabByType(contentType, transform)
            .GetComponent<RadioContentPlayer>();
        
        activePlayers[channel] = player;
        
        
        return player;
    }
    
    protected void DestroyPlayer(RadioChannel channel) {
        if (activePlayers[channel] == null) return;
        Destroy(activePlayers[channel].gameObject);
        activePlayers[channel] = null;
    }


    private void OnRadioOnOffChange(bool isOn) {
        if (!isOn) {
            TurnRadioOff();
            radioNoiseAudioSource.volume = 0;
            mouseHoverHint.text = "Radio (Off)";
        }
        else {
            TurnRadioOn();
            UpdateSpeakerVolume(false);
            mouseHoverHint.text = "Radio (On)";
        }
    }

    private void OnVolumeChange(float volume) {
        radioNoiseAudioSource.volume = 1 - volume;
        UpdateSpeakerVolume(true);
    }

    
    
    private void UpdateSpeakerVolume(bool isInstant) {
        if (!electricityModel.HasElectricity()) {
            return;
        }
        float noiseMultiplier = radioModel.IsOn.Value ? 1 : 0;
        if (!isInstant) {
            if (lowSoundLock.RefCount > 0) {
                SetAllSpeakersVolume(radioModel.RelativeVolume, false, false);
                radioNoiseAudioSource.DOFade(0.1f * NoiseVolume * noiseMultiplier, 1f);
            }
            else {
                radioNoiseAudioSource.DOFade(NoiseVolume * noiseMultiplier, 1f);
                SetAllSpeakersVolume(radioModel.RelativeVolume, true,false);
            }
        }
        else {
            if (lowSoundLock.RefCount > 0) {
                SetAllSpeakersVolume(radioModel.RelativeVolume, false, true);
                radioNoiseAudioSource.volume = 0.1f * NoiseVolume * noiseMultiplier;
            }
            else {
                SetAllSpeakersVolume(radioModel.RelativeVolume, true, true);
                radioNoiseAudioSource.volume = NoiseVolume * noiseMultiplier;
            }
        }
        
    }

    
    private void SetAllSpeakersVolume(float relativeVolume, bool isLoud, bool isInstant) {
        foreach (RadioContentPlayer player in activePlayers.Values) {
            if (player == null) {
                continue;
            }
            player.SetVolume(relativeVolume, isLoud, isInstant);
        }
    }
        
    
    
    
    protected override void OnNoElectricity() {
        TurnRadioOff();
        radioNoiseAudioSource.volume = 0;
    }

    protected override void OnElectricityRecovered() {
        UpdateSpeakerVolume(false);
        TurnRadioOn();
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

    private void OnConstructDescriptionDatas(OnConstructDescriptionDatas e) {
        
        int day = this.GetSystem<GameTimeManager>().Day;
        float radioReality = radioRealityCurve.Evaluate(day);
        ConstructDescriptionDatas(radioModel.DescriptionDatas, radioReality, day);
    }

    private void OnRadioStart(OnRadioProgramStart e) {
        RadioChannel channel = e.channel;
        float volume = 0f;
        
        if (radioModel.CurrentChannel.Value == channel && radioModel.IsOn.Value && electricityModel.HasElectricity()) {
            volume = 1;
        }
        ContentStart(e.radioContent, channel, volume<=0);
        if (volume > 0) {
            transform.DOShakeRotation(3f, 5, 20, 90, false).SetLoops(-1);
        }
       
    }

 

    
    void Start() {
        this.RegisterEvent<OnNewBodyInfoGenerated>(OnBodyInfoGenerated).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.GetModel<GameSceneModel>().GameScene.RegisterOnValueChaned(OnGameSceneChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        //this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

   


    private void ContentStart(IRadioContent content, RadioChannel channel, bool isMuted) {
        
       
        if (this) {
            if(activePlayers.ContainsKey(channel)) {
                RadioContentPlayer player = activePlayers[channel];
                if (player == null) {
                    player = SpawnPlayer(channel, content.ContentType);
                    radioModel.SetIsSpeaking(channel, true);
                    player.Play(content, OnSpeakerStop, isMuted);
                    UpdateSpeakerVolume(true);
                }
                //activePlayers[channel].Speak(speakText, mixer, "Radio", overallVolume, OnSpeakerStop, speakRate, 1f, speakGender);
                
            }
        }
    }

    private void ConstructDescriptionDatas(List<AlienDescriptionData> descriptionDatas, float radioReality, int day) {

        descriptionDatas.Clear();

        List<BodyInfo> todayBodies = bodyModel.AllTodayDeadBodies.Select((info => info.BodyInfo)).ToList();

        todayBodies.CTShuffle();
        foreach (BodyInfo bodyInfo in todayBodies) {
            descriptionDatas.Add(new AlienDescriptionData(bodyInfo, radioReality));
        }




        /*
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
        }*/
        descriptionDatas.CTShuffle();

    }

    private void OnBodyInfoGenerated(OnNewBodyInfoGenerated e) {
        int day = this.GetSystem<GameTimeManager>().Day;
        float radioReality = radioRealityCurve.Evaluate(day);

        radioModel.DescriptionDatas.Clear();
        ConstructDescriptionDatas(radioModel.DescriptionDatas, radioReality, day);

      
       if (day == 1) {
          
           AddDeadBodyIntroRadio();
           AddInitialRadio();
       }

       if (day == 0) {
           AddPrologueBodyIntroRadio();
       }
    }

    private void AddPrologueBodyIntroRadio() {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        
        GameEventSystem eventSystem = this.GetSystem<GameEventSystem>();



        eventSystem.AddEvent(new PrologueBodyRadio(
            new TimeRange(currentTime + new TimeSpan(0, 10, 0), currentTime + new TimeSpan(0, 30, 0)),
            AudioMixerList.Singleton.AudioMixerGroups[1]));

    }

    private void AddDeadBodyIntroRadio() {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        
        GameEventSystem eventSystem = this.GetSystem<GameEventSystem>();

        string speakContent = this.GetModel<HotUpdateDataModel>().GetData("Radio_Intro").values[0];
        
        eventSystem.AddEvent(new DeadBodyRadioIntroEvent(
            new TimeRange(currentTime + new TimeSpan(0, 0, 0), currentTime + new TimeSpan(0, 20, 0)),
            speakContent,
            1, Gender.MALE,
            AudioMixerList.Singleton.AudioMixerGroups[1]));
    }

    private void AddInitialRadio() {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        
        GameEventSystem eventSystem = this.GetSystem<GameEventSystem>();
        
        eventSystem.AddEvent(new MusicRadio(
            new TimeRange(currentTime + new TimeSpan(0, 1, 0)),
            RadioContentPlayerFactory.Singleton.GetMusicSourceIndex()));

        eventSystem.AddEvent(new FoodTutorialRadio(new TimeRange(currentTime.AddDays(1)),
            AudioMixerList.Singleton.AudioMixerGroups[1]));
    }


    private void TurnRadioOff() {
        foreach (var speaker in activePlayers.Values) {
            //speaker.SetOverallVolume(0);
            if (speaker) {
                speaker.Mute(true);
            }
            
        }
        transform.DOKill(true);
    }
    
    private void TurnRadioOn() {
        RadioChannel channel = radioModel.CurrentChannel.Value;
        if (activePlayers.ContainsKey(channel)) {
            //speakers[channel].SetOverallVolume(1);
            activePlayers[channel]?.Mute(false);
            if (IsPlayerPlaying(channel)) {
                transform.DOShakeRotation(3f, 5, 20, 90, false).SetLoops(-1);
            }
        }
    }

    private void OnSpeakerStop(RadioContentPlayer player) {
        
        foreach (var pair in activePlayers) {
            if (pair.Value == player) {
                radioModel.SetIsSpeaking(pair.Key, false);
                if (pair.Key == radioModel.CurrentChannel.Value) {
                    transform.DOKill(true);
                }

                DestroyPlayer(pair.Key);
                break;
            }
        }
    }


    public void OnPointerClick(PointerEventData eventData) {
        panel.Show(0.5f);
    }
}