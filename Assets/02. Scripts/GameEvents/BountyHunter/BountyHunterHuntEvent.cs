using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.BodyManagmentSystem;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public struct OnBountyHunterKillCorrectAlien {
    public int FoodCount;
}
public class BountyHunterHuntEvent : GameEvent{
    private BodyModel bodyModel;
    private PlayerResourceSystem playerResourceSystem;
    [ES3Serializable]
    private List<BodyInfo> bodyInfos;
    //private bool isAlien;
    [field: ES3Serializable]
    public override GameEventType GameEventType { get; } = GameEventType.General;
    [field: ES3Serializable]
    public override float TriggerChance { get; } = 1;
    public BountyHunterHuntEvent(TimeRange startTimeRange, List<BodyInfo> infos) : base(startTimeRange) {
        bodyModel = this.GetModel<BodyModel>();
        playerResourceSystem = this.GetSystem<PlayerResourceSystem>();
        this.bodyInfos = infos;
    }

    public BountyHunterHuntEvent() : base() {
        bodyModel = this.GetModel<BodyModel>();
        playerResourceSystem = this.GetSystem<PlayerResourceSystem>((system => {
            playerResourceSystem = system as PlayerResourceSystem;
        } ));
    }
    public override void OnStart() {
        
    }

    public override EventState OnUpdate() {

        bool killAlien = false;
        bool killGood = false;
        foreach (BodyInfo bodyInfo in bodyInfos) {
            if (bodyInfo == null) {
                continue;
            }
            BodyInfo updatedInfo = bodyModel.GetBodyInfoByID(bodyInfo.ID);
            if (updatedInfo != null) {
                if (!updatedInfo.IsDead) {
                    bodyModel.RemoveBodyInfo(updatedInfo);
                    updatedInfo.IsDead = true;
                    
                    if (updatedInfo.IsAlien) {
                        killAlien = true;
                    }
                    else {
                        killGood = true;
                    }
                }
            }
        }

        if (killAlien && !killGood) {
            this.SendEvent<OnBountyHunterKillCorrectAlien>(new OnBountyHunterKillCorrectAlien() {
                FoodCount = Random.Range(3, 5)
            });
        }

        return EventState.End;
    }

    public override void OnEnd() {
       
    }

    public override void OnMissed() {
      
    }
}

public class BountyHunterHuntWrongPersonRadio : RadioEvent {
    public BountyHunterHuntWrongPersonRadio(TimeRange startTimeRange, string speakContent, float speakRate, Gender speakGender, AudioMixerGroup mixer) : base(startTimeRange, speakContent, speakRate, speakGender, mixer) {
    }
    
    public BountyHunterHuntWrongPersonRadio(): base(){}
    [field: ES3Serializable]
    public override float TriggerChance { get; } = 1;
    public override void OnEnd() {
     
    }

    public override void OnMissed() {
       
    }

    protected override void OnRadioStart()
    {
        
    }
}
