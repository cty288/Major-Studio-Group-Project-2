using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.BodyOutside;
using _02._Scripts.FPSEnding;
using _02._Scripts.Stats;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.TimeSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterMotherSpawnEvent : DailyKnockEvent {
    private BodyModel bodyModel;
    public MonsterMotherSpawnEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float eventTriggerChance) :
        base(startTimeRange) {
        bodyModel = this.GetModel<BodyModel>();
        this.bodyInfo = bodyInfo;
        Debug.Log("Monster mother spawn time: " + startTimeRange.StartTime);
    }

    public MonsterMotherSpawnEvent() : base() {
        bodyModel = this.GetModel<BodyModel>();
    }

    public override void OnStart() {
        this.RegisterEvent<OnOutsideBodyClicked>(OnOutsideBodyClicked);
        statsModel.UpdateStat("TotalDoorKnock",
            new SaveData("Total Visitors", (int) statsModel.GetStat("TotalDoorKnock", 0) + 1));

    }

    public override EventState OnUpdate() {
        if (bodyModel.IsInAllBodyTimeInfos(bodyInfo)) {
            return base.OnUpdate();
        }
        Debug.Log("Bounty Hunter Quest: The body is already dead");
        return EventState.End;
    }

    protected override Func<bool> OnOpen() {
        onClickPeepholeSpeakEnd = false;
        Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
        AudioSystem.Singleton.Play2DSound("door_open");

        this.GetSystem<ITimeSystem>().AddDelayTask(0.3f, () => {
            AudioSource source = AudioSystem.Singleton.Play2DSound("monster_roar_1");
        });
        this.GetSystem<ITimeSystem>().AddDelayTask(0.5f, () => {
            LoadCanvas.Singleton.ShowImage(2, 0f);
           
            this.GetSystem<ITimeSystem>().AddDelayTask(3f, OnAlienClickedOutside);
        });
        return () => onClickPeepholeSpeakEnd;
    }

    protected void OnAlienClickedOutside() {
        DogModel dogModel = this.GetModel<DogModel>();
        
        bool hasBulletAndGun = playerResourceModel.HasEnoughResource<BulletGoods>(1) &&
                               playerResourceModel.HasEnoughResource<GunResource>(1);

        bool hasDog = dogModel.HaveDog && dogModel.isDogAlive && !dogModel.SentDogBack;

        if (hasDog) {
            SacrificeDog();
        }
        
       
        if (!hasBulletAndGun) {
            LoadCanvas.Singleton.HideImage(1f);
            DieCanvas.Singleton.Show("You are killed by the monster!", 3, "Killed_End");
            this.GetModel<GameStateModel>().GameState.Value = GameState.End;
            this.GetSystem<BodyGenerationSystem>().StopCurrentBody();
        }
        else {
            this.GetSystem<ITimeSystem>().AddDelayTask(2f, () => {
                StartFPS();
            });
        }
    }

    private void StartFPS() {
        LoadCanvas.Singleton.StopLoad(null);
        this.GetModel<GameSceneModel>().GameScene.Value = GameScene.MainGame;
        MonsterMotherModel monsterMotherModel = this.GetModel<MonsterMotherModel>();
        monsterMotherModel.isFightingMother.Value = true;
        this.GetSystem<ITimeSystem>().AddDelayTask(0.5f, () => {
            LoadCanvas.Singleton.HideImage(0.5f);
        });
        this.GetModel<RadioModel>().IsOn.Value = false;
        monsterMotherModel.MotherHealth.RegisterOnValueChaned(OnMonsterMotherHealthChanged);
       
    }

    private void OnMonsterMotherHealthChanged(int health) {
        if (health <= 0) {
            onClickPeepholeSpeakEnd = true;
        }
    }

    protected override void SacrificeDog() {
        DogSystem dogSystem = this.GetSystem<DogSystem>();
        AudioSystem.Singleton.Play2DSound("dog_die");
        dogSystem.KillDog();
    }
    private void SpawnNextTime() {
        if (!bodyModel.IsInAllBodyTimeInfos(bodyInfo)) {
            Debug.Log("Bounty Hunter Quest: The body is already dead");
            return;
        }

        DateTime nextStartTime = StartTimeRange.StartTime.AddDays(Random.Range(2, 4));
        nextStartTime = new DateTime(nextStartTime.Year, nextStartTime.Month, nextStartTime.Day, 23,
            Random.Range(0, 39), 0);
        DateTime nextEndTime = nextStartTime.AddMinutes(20);
        gameEventSystem.AddEvent(new MonsterMotherSpawnEvent(new TimeRange(nextStartTime, nextEndTime),
            bodyInfo, 1));
    }

    public override void OnMissed() {
        SpawnNextTime();
    }

    public override void OnEventEnd() {
        SpawnNextTime();
        this.GetModel<MonsterMotherModel>().MotherHealth.UnRegisterOnValueChanged(OnMonsterMotherHealthChanged);
    }
}
