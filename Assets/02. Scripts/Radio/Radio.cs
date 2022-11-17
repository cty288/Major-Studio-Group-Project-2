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
    [SerializeField]
    private AudioClip radioOpenSound;
    public BodyInfo targetAlien;

    private AudioSource radioOpenAudioSource;
    [SerializeField] private AnimationCurve radioRealityCurve;
    [SerializeField] private AnimationCurve unrelatedBodyInfoCountWithDay;
    [SerializeField] private AnimationCurve realAlienDescriptionRepeatTimeWithDay;

    private Coroutine dailyRadioCoroutine;
    private BodyGenerationSystem bodyGenerationSystem;
    private BodyManagmentSystem bodyManagmentSystem;
    private bool isSpeaking = false;
    private GameTimeManager gameTimeManager;
    private void Awake() {
        radioOpenAudioSource = GetComponent<AudioSource>();
        bodyGenerationSystem = this.GetSystem<BodyGenerationSystem>();
        bodyManagmentSystem = this.GetSystem<BodyManagmentSystem>();
        gameTimeManager = this.GetSystem<GameTimeManager>();
        gameTimeManager.CurrentTime.RegisterOnValueChaned(OnTimeChanged).UnRegisterWhenGameObjectDestroyed(gameObject);// += OnDayEnd;
        this.GetModel<GameStateModel>().GameState.RegisterOnValueChaned(OnGameStateChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnTimeChanged(DateTime time) {
        if (time.Hour == 23 && time.Minute >= 50) {
            if (dailyRadioCoroutine != null) {
                StopCoroutine(dailyRadioCoroutine);
                dailyRadioCoroutine = null;
                StopRadio();
            }
           
        }
        //throw new NotImplementedException();
    }

    private void OnGameStateChanged(GameState arg1, GameState state) {
        if (state == GameState.End) {
            speaker.Stop();
        }
    }

    private void OnDayEnd(int obj) {
        
    }

    void Start() {
        this.RegisterEvent<OnNewBodyInfoGenerated>(PlayRadioInformation).UnRegisterWhenGameObjectDestroyed(gameObject);
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


    private IEnumerator DailyRadioCoroutine() {
        int day = this.GetSystem<GameTimeManager>().Day;
        float radioReality = radioRealityCurve.Evaluate(day);

        List<AlienDescriptionData> descriptionDatas = new List<AlienDescriptionData>();

        yield return new WaitForSeconds(Random.Range(5, 10));
        while (true) {
            if (!isSpeaking) {
                yield return new WaitForSeconds(Random.Range(6f, 20f));
                if (gameTimeManager.CurrentTime.Value.Hour == 23 && gameTimeManager.CurrentTime.Value.Minute >= 50) {
                    
                }
                if (descriptionDatas.Count == 0) {
                    ConstructDescriptionDatas(descriptionDatas, radioReality, day);
                }

                AlienDescriptionData descriptionData = descriptionDatas[0];
                descriptionDatas.RemoveAt(0);
                if (!speaker.IsSpeaking) {
                    RadioSpeak(AlienDescriptionFactory.GetRadioDescription(descriptionData.BodyInfo,
                        descriptionData.Reality));
                }
            }
            else {
                yield return new WaitForSeconds(Random.Range(15f, 25f));
                if (Random.Range(0, 100) <= 30) {
                    StopRadio();
                }
            }
        }
    }

    private void RadioSpeak(string speakText) {
        isSpeaking = true;
        radioOpenAudioSource.DOFade(0.2f, 1f);
        this.Delay(1 + Random.Range(2f, 5f), () => {
            if (this) {
                radioOpenAudioSource.DOFade(0.03f, 1f);
                Gender randomGender = Random.Range(0, 2) == 0 ? Gender.MALE : Gender.FEMALE;
                speaker.Speak(speakText, StopRadio, Random.Range(0.8f, 1.2f), randomGender);
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

    private void PlayRadioInformation(OnNewBodyInfoGenerated e) {
        if (dailyRadioCoroutine != null) {
            StopCoroutine(dailyRadioCoroutine);
            StopRadio();
        }
        dailyRadioCoroutine = StartCoroutine(DailyRadioCoroutine());
    }


    private void StopRadio() {
        isSpeaking = false;
        speaker.Corrupt(5, () => {
            speaker.Stop();
        });
        radioOpenAudioSource.DOFade(0, 1f);
    }
}