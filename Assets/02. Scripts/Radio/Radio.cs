using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crosstales;
using Crosstales.RTVoice.Model.Enum;
using DG.Tweening;
using UnityEngine;
using MikroFramework.Event;
using MikroFramework.Architecture;
using MikroFramework;
using MikroFramework.AudioKit;
using Random = UnityEngine.Random;

public class AlienDescriptionData {
    public BodyInfo BodyInfo;
    public float Reality;

    public AlienDescriptionData(BodyInfo bodyInfo, float reality) {
        this.BodyInfo = bodyInfo;
        this.Reality = reality;
    }
}

public class Radio : AbstractMikroController<MainGame>
{
    public Speaker speaker;
    
    public BodyInfo targetAlien;

    private AudioSource radioOpenAudioSource;
    [SerializeField] private AnimationCurve radioRealityCurve;
    [SerializeField] private AnimationCurve unrelatedBodyInfoCountWithDay;
    [SerializeField] private AnimationCurve realAlienDescriptionRepeatTimeWithDay;

 
    private BodyGenerationSystem bodyGenerationSystem;
    private BodyManagmentSystem bodyManagmentSystem;
   
    private GameTimeManager gameTimeManager;
    private RadioModel radioModel;
    private void Awake() {
        radioOpenAudioSource = GetComponent<AudioSource>();
        bodyGenerationSystem = this.GetSystem<BodyGenerationSystem>();
        bodyManagmentSystem = this.GetSystem<BodyManagmentSystem>();
        gameTimeManager = this.GetSystem<GameTimeManager>();

        this.RegisterEvent<OnRadioEnd>(OnRadioEnd).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnRadioStart>(OnRadioStart).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnConstructDescriptionDatas>(OnConstructDescriptionDatas).UnRegisterWhenGameObjectDestroyed(gameObject);
        radioModel = this.GetModel<RadioModel>();

    }

    private void OnConstructDescriptionDatas(OnConstructDescriptionDatas obj) {
        int day = this.GetSystem<GameTimeManager>().Day;
        float radioReality = radioRealityCurve.Evaluate(day);
        ConstructDescriptionDatas(radioModel.DescriptionDatas, radioReality, day);
    }

    private void OnRadioStart(OnRadioStart e) {
        RadioSpeak(e.speakContent, e.speakRate, e.speakGender);
    }

    private void OnRadioEnd(OnRadioEnd e) {
        StopRadio(true);
    }

    
    void Start() {
        this.RegisterEvent<OnNewBodyInfoGenerated>(OnBodyInfoGenerated).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.GetModel<GameSceneModel>().GameScene.RegisterOnValueChaned(OnGameSceneChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnGameSceneChanged(GameScene arg1, GameScene scene) {
        if (scene == GameScene.MainGame) {
            radioOpenAudioSource.DOFade(0.03f, 1f);
            speaker.AudioMixer.DOSetFloat("volume", -20, 1f);
           
        }
        else {
            speaker.AudioMixer.DOSetFloat("volume", -45, 1f);
            radioOpenAudioSource.DOFade(0.01f, 1f);
        }
    }




    private void RadioSpeak(string speakText, float speakRate, Gender speakGender) {
        radioModel.IsSpeaking = true;
        radioOpenAudioSource.DOFade(0.2f, 1f);
        this.Delay(1 + Random.Range(2f, 5f), () => {
            if (this) {
                radioOpenAudioSource.DOFade(0.03f, 1f);
                speaker.Speak(speakText, OnSpeakerStop, speakRate, speakGender);
            }
        });
    }

    private void ConstructDescriptionDatas(List<AlienDescriptionData> descriptionDatas, float radioReality, int day) {
        BodyInfo todayAlien = bodyGenerationSystem.TodayAlien;
        List<BodyInfo> allPossibleBodyInfos =
            bodyManagmentSystem.allBodyTimeInfos.Select((info => info.BodyInfo)).ToList();

        allPossibleBodyInfos.CTShuffle();
        if (todayAlien != null) {
            for (int i = 0; i < realAlienDescriptionRepeatTimeWithDay.Evaluate(day); i++) {
                descriptionDatas.Add(new AlienDescriptionData(todayAlien, radioReality));
            }
        }
       

        for (int i = 0; i < unrelatedBodyInfoCountWithDay.Evaluate(day); i++) {
            if (Random.Range(0, 2) < 1) {
                descriptionDatas.Add(
                    new AlienDescriptionData(BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, false),
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
            
            this.GetSystem<GameEventSystem>().AddEvent(new DailyBodyRadio(
                new TimeRange(currentTime + new TimeSpan(0, 10, 0), currentTime + new TimeSpan(0, 20, 0)),
                AlienDescriptionFactory.GetRadioDescription(descriptionData.BodyInfo, descriptionData.Reality),
                Random.Range(0.85f, 1.2f), Random.Range(0, 2) == 0 ? Gender.MALE : Gender.FEMALE));
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
    }
}