using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public struct OnBountyHunterKillCorrectAlien {
    public int FoodCount;
}
public class BountyHunterHuntEvent : GameEvent{
    private BodyManagmentSystem bodyManagmentSystem;
    private PlayerResourceSystem playerResourceSystem;
    private BodyInfo bodyInfo;
    private bool isAlien;
    public override GameEventType GameEventType { get; } = GameEventType.General;
    public override float TriggerChance { get; } = 1;
    public BountyHunterHuntEvent(TimeRange startTimeRange, BodyInfo bodyInfo) : base(startTimeRange) {
        bodyManagmentSystem = this.GetSystem<BodyManagmentSystem>();
        playerResourceSystem = this.GetSystem<PlayerResourceSystem>();
        this.bodyInfo = bodyInfo;
        this.isAlien = bodyInfo.IsAlien;
    }
    public override void OnStart() {
        
    }

    public override EventState OnUpdate() {
        Debug.Log("Bounty Hunter Hunt Event. Is Alien: " + isAlien);
        if (isAlien) {
            this.SendEvent<OnBountyHunterKillCorrectAlien>(new OnBountyHunterKillCorrectAlien() {
                FoodCount = Random.Range(3,5)
            });
            //playerResourceSystem.AddFood(Random.Range(2, 5));
        }
        bodyManagmentSystem.RemoveBodyInfo(bodyInfo);
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

    public override float TriggerChance { get; } = 1;
    public override void OnEnd() {
     
    }

    public override void OnMissed() {
       
    }

    protected override void OnRadioStart()
    {
        
    }
}
