using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
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

    private int knockDoorCheckTimeInterval = 10;
    private float knockDoorChance = 0.3f;
    private float nonAlienChance = 1f;


    private float knockWaitTimeSinceDayStart = 60f;
    
    //private Coroutine knockDoorCheckCoroutine;
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
        //this.RegisterEvent<OnNewBodyInfoGenerated>(OnNewBodyInfoGenerated);
        bodyManagmentSystem = this.GetSystem<BodyManagmentSystem>();
        bodyGenerationModel = this.GetModel<BodyGenerationModel>();
        gameTimeManager = this.GetSystem<GameTimeManager>();

        
    }

  

    private void OnEndOfDay(int day) {
        dayNum = day;
        if (day >= 2) {
            nonAlienChance -= 0.2f;
            nonAlienChance = Mathf.Clamp(nonAlienChance, 0.5f, 1f);
        }

        if (day == 1) {
            this.GetSystem<ITimeSystem>().AddDelayTask(knockWaitTimeSinceDayStart, SpawnAlienOrDeliverBody);
            
        }
        
     
    }

  
    
    private void SpawnAlienOrDeliverBody() {
        List<BodyTimeInfo> Aliens = bodyManagmentSystem.Aliens;
        List<BodyTimeInfo> Humans = bodyManagmentSystem.Humans;
        
        BodyInfo targetBody;
       
        
        if (Random.Range(0f, 1f) <= nonAlienChance || dayNum==1 || Aliens.Count==0) {
            if (Random.Range(0f, 1f) <= 0.5f && Aliens.Count > 0) {
                targetBody = BodyInfo.GetBodyInfoOpposite(Aliens[Random.Range(0, Aliens.Count)].BodyInfo, 0.7f, 0.8f, true,
                    false);
            }
            else {
                targetBody = BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, false, false,
                    new NormalKnockBehavior(3, Random.Range(4, 7), null));
            }
           // Debug.Log("Spawned a non-alien");
        }
        else {
            targetBody = Aliens[Random.Range(0, Aliens.Count)].BodyInfo;
            //Debug.Log("Spawned an alien!");
        }

        int knockDoorTimeInterval = 3;
        int knockTime = Random.Range(6, 9);
        DateTime currentTime = gameTimeManager.CurrentTime;

        gameEventSystem.AddEvent(new BodyGenerationEvent(
            new TimeRange(currentTime + new TimeSpan(0, knockDoorCheckTimeInterval, 0)), targetBody,
            knockDoorChance, SpawnAlienOrDeliverBody, SpawnAlienOrDeliverBody));
    }

    public void StopCurrentBody() {
        bodyGenerationModel.CurrentOutsideBody.Value = null;
        bodyGenerationModel.CurrentOutsideBodyConversationFinishing = false;
    }
    
    
}
