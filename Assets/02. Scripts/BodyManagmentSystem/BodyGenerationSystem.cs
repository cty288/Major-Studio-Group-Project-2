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
    private float knockDoorChance = 0.7f;
   // private float nonAlienChance = 1f;
   //private float knockWaitTimeSinceDayStart = 60f;
    
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
       
        if (day == 1) {
            InitialSpawnBodyEvent();
            
        }
        
     
    }

    private Dictionary<BodyPartType, HashSet<int>> GetAvailableBodyPartIndicesa() {
        Dictionary<BodyPartType, HashSet<int>> indices = bodyModel.AvailableBodyPartIndices;

        foreach (BodyPartType bodyPartType in indices.Keys) {
            BodyPartCollection collection =
                AlienBodyPartCollections.Singleton.GetBodyPartCollectionByBodyType(bodyPartType, false);
            int count = collection.HeightSubCollections[0].NewspaperBodyPartDisplays.HumanTraitPartsPrefabs.Count;

            int additionalCount = Random.Range(1, 4);
            for (int i = 0; i < additionalCount; i++) {
                //indices[bodyPartType].Add(Random.Range(0, count));
            }
        }

        return indices;


    }

    public BodyInfo GetAlienOrDeliverBody() {
        List<BodyTimeInfo> Aliens = bodyModel.Aliens;
       // List<BodyTimeInfo> TodayAliens = bodyModel.AllTodayAliens;
        BodyInfo targetBody = null;
        //TODO: 改成如果今天报纸上没人那就啥怪物都不刷
        
        HotUpdateDataModel hotUpdateDataModel = this.GetModel<HotUpdateDataModel>();
        float baseProb = float.Parse(hotUpdateDataModel.GetData("Monster_Knock_Prob_Base").values[0]);
        var availableBodyPartIndices = GetAvailableBodyPartIndicesa();
        float alienChance = Mathf.Min((baseProb + Mathf.Min(0.15f, dayNum * 0.007f)) * Aliens.Count, 0.8f);
        if (dayNum >= 7) {
            alienChance += bodyModel.ConsecutiveNonAlianSpawnCount * 0.05f;
        }
        else {
            alienChance += bodyModel.ConsecutiveNonAlianSpawnCount * 0.02f;
        }
       
        alienChance = Mathf.Min(alienChance, 0.9f);
        
        
        Debug.Log("alienChance: " + alienChance);
        PlayerResourceModel playerResourceModel = this.GetModel<PlayerResourceModel>();
        if (bodyModel.AllTodayDeadBodies.Count ==0 || playerResourceModel.GetResourceCount<FoodResource>()<=0) {
            alienChance = 0;
            bodyModel.ConsecutiveNonAlianSpawnCount = 0;
        }
        float nonAlienChance = 1f - alienChance;
        
        if (Random.Range(0f, 1f) <= nonAlienChance || dayNum==1 || Aliens.Count==0) {
            if (Random.Range(0f, 1f) <= 0.5f && Aliens.Count > 0) {
                targetBody = BodyInfo.GetBodyInfoOpposite(Aliens[Random.Range(0, Aliens.Count)].BodyInfo, 0.7f, 0.8f, true,
                    false, availableBodyPartIndices);
            }
            else {
                targetBody = BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, false, 0,
                    new NormalKnockBehavior(4, Random.Range(4, 7), null), availableBodyPartIndices, 40);
            }
            // Debug.Log("Spawned a non-alien");
            //bodyModel.ConsecutiveNonAlianSpawnCount++;
        }
        else {
            float todayAlien = Random.Range(0f, 1f);
            List<BodyTimeInfo> todayAliens = bodyModel.AllTodayAliens;
            if (todayAlien <= 0.6f && todayAliens!=null && todayAliens.Count > 0) {
                targetBody = todayAliens[Random.Range(0, todayAliens.Count)].BodyInfo;
            }
            else {
                targetBody = Aliens[Random.Range(0, Aliens.Count)].BodyInfo;
            }
            Debug.Log("Spawned an alien!");
            //bodyModel.ConsecutiveNonAlianSpawnCount = 0;
        }

      
        return targetBody;
    }

    public void InitialSpawnBodyEvent() {
        
        int knockDoorTimeInterval = 3;
       
        DateTime currentTime = gameTimeManager.CurrentTime;

        DateTime nextTime = currentTime + new TimeSpan(0, knockDoorCheckTimeInterval + Random.Range(0, 30), 0);
        if (nextTime.Hour < gameTimeManager.NightTimeStart) {
            nextTime = new DateTime(nextTime.Year, nextTime.Month, nextTime.Day, gameTimeManager.NightTimeStart,
                Random.Range(20, 40), 0);
        }
        gameEventSystem.AddEvent(new DailyKnockEvent(
            new TimeRange(nextTime)));
    }

    public void StopCurrentBody() {
        bodyGenerationModel.CurrentOutsideBody.Value = null;
        bodyGenerationModel.CurrentOutsideBodyConversationFinishing = false;
    }
    
    
}
