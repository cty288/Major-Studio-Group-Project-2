using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.BodyOutside;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.BindableProperty;
using MikroFramework.Event;
using MikroFramework.TimeSystem;
using UniRx.InternalUtil;

using UnityEngine;
using Random = UnityEngine.Random;


public class BodyGenerationSystem : AbstractSystem {
    //private BodyInfo todayAlien;
    
    //public BodyInfo TodayAlien => todayAlien;

    private int dayNum;
    private BodyManagmentSystem bodyManagmentSystem;
    private GameTimeManager gameTimeManager;

    private GameEventSystem gameEventSystem;

    private int knockDoorCheckTimeInterval = 30;
    private float knockDoorChance = 0.5f;
    private float nonAlienChance = 1f;


    private float knockWaitTimeSinceDayStart = 60f;
    
    //private Coroutine knockDoorCheckCoroutine;
    private BodyModel bodyModel;
    private BodyGenerationModel bodyGenerationModel;
    
    
    protected override void OnInit() {
        gameEventSystem = this.GetSystem<GameEventSystem>();
        this.GetSystem<ITimeSystem>().AddDelayTask(0.1f, () => {
            AudioSystem.Singleton.Initialize(null);
            AudioSystem.Singleton.MasterVolume = 1f;
            AudioSystem.Singleton.MusicVolume = 1f;
            AudioSystem.Singleton.SoundVolume = 1f;
        });
       
        this.GetSystem<GameTimeManager>().OnDayStart += OnEndOfDay;
        this.RegisterEvent<OnNewDay>(OnNewDay);
        //this.RegisterEvent<OnNewBodyInfoGenerated>(OnNewBodyInfoGenerated);
        bodyManagmentSystem = this.GetSystem<BodyManagmentSystem>();
        bodyGenerationModel = this.GetModel<BodyGenerationModel>();
       
        gameTimeManager = this.GetSystem<GameTimeManager>();
        bodyModel = this.GetModel<BodyModel>();
        
    }

    private void OnNewDay(OnNewDay e) {
        
    }


    private void OnEndOfDay(int day, int hour) {
        dayNum = day;
        if (day >= 2) {
            nonAlienChance -= 0.2f;
            nonAlienChance = Mathf.Clamp(nonAlienChance, 0.5f, 1f);
        }

        if (day == 1) {
            SpawnAlienOrDeliverBody();
            
        }
        
     
    }

    private Dictionary<BodyPartType, HashSet<int>> GetAvailableBodyPartIndicesa() {
        Dictionary<BodyPartType, HashSet<int>> indices = bodyModel.AvailableBodyPartIndices;

        foreach (BodyPartType bodyPartType in indices.Keys) {
            BodyPartCollection collection =
                AlienBodyPartCollections.Singleton.GetBodyPartCollectionByBodyType(bodyPartType);
            int count = collection.HeightSubCollections[0].NewspaperBodyPartDisplays.HumanTraitPartsPrefabs.Count;

            int additionalCount = Random.Range(1, 4);
            for (int i = 0; i < additionalCount; i++) {
                indices[bodyPartType].Add(Random.Range(0, count));
            }
        }

        return indices;


    }

    public void SpawnAlienOrDeliverBody() {
        List<BodyTimeInfo> Aliens = bodyModel.Aliens;
        List<BodyTimeInfo> Humans = bodyModel.Humans;
        
        BodyInfo targetBody;

        var availableBodyPartIndices = GetAvailableBodyPartIndicesa();
        
        if (Random.Range(0f, 1f) <= nonAlienChance || dayNum==1 || Aliens.Count==0) {
            if (Random.Range(0f, 1f) <= 0.5f && Aliens.Count > 0) {
                targetBody = BodyInfo.GetBodyInfoOpposite(Aliens[Random.Range(0, Aliens.Count)].BodyInfo, 0.7f, 0.8f, true,
                    false, availableBodyPartIndices);
            }
            else {
                targetBody = BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, false, false,
                    new NormalKnockBehavior(3, Random.Range(4, 7), null), availableBodyPartIndices);
            }
           // Debug.Log("Spawned a non-alien");
        }
        else {
            targetBody = Aliens[Random.Range(0, Aliens.Count)].BodyInfo;
            //Debug.Log("Spawned an alien!");
        }

        int knockDoorTimeInterval = 3;
       
        DateTime currentTime = gameTimeManager.CurrentTime;

        gameEventSystem.AddEvent(new DailyKnockEvent(
            new TimeRange(currentTime + new TimeSpan(0, knockDoorCheckTimeInterval, 0)), targetBody,
            knockDoorChance));
    }

    public void StopCurrentBody() {
        bodyGenerationModel.CurrentOutsideBody.Value = null;
        bodyGenerationModel.CurrentOutsideBodyConversationFinishing = false;
    }
    
    
}
