using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.GameTime;
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
            
            
            BodyInfo targetInfo = bodyModel.GetBodyInfoByID(bodyInfo.ID);   //bodyInfos.Add();
            if (targetInfo == null) {
                targetInfo = bodyInfo;
            }
           
            
           
            if (targetInfo != null) {
                if (!targetInfo.IsDead) {
                    bodyModel.KillBodyInfo(targetInfo);
                    targetInfo.IsDead = true;
                    
                    if (targetInfo.IsAlien) {
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
            AddToImportantNews(true);
        }

        if (killGood) {
            AddToImportantNews(false);
        }

        return EventState.End;
    }

    private void AddToImportantNews(bool killAlien) {
        ImportantNewspaperModel importantNewspaperModel = this.GetModel<ImportantNewspaperModel>();
        ImportantNewsTextModel importantNewsTextModel = this.GetModel<ImportantNewsTextModel>();
        int week = importantNewspaperModel.GetWeekForNews(this.GetModel<GameTimeModel>().Day);
        if (importantNewspaperModel.HasPageOfID(week, "BountyHunterKill")) {
            importantNewspaperModel.RemoveAllPagesOfID(week, "BountyHunterKill");
        }

        importantNewspaperModel.AddPageToNewspaper(week,
            importantNewsTextModel.GetInfo(killAlien ? "BountyHunterSuccess" : "BountyHunterFail"), 1);
    }

    public override void OnEnd() {
       
    }

    public override void OnMissed() {
      
    }
}

public class BountyHunterHuntWrongPersonRadio : RadioEvent<RadioTextContent> {
    [field: ES3Serializable]
    protected RadioTextContent radioContent { get; set; }

    protected override RadioTextContent GetRadioContent() {
        return radioContent;
    }
    protected override void SetRadioContent(RadioTextContent radioContent) {
        this.radioContent = radioContent;
    }
    public BountyHunterHuntWrongPersonRadio(TimeRange startTimeRange, string speakContent, float speakRate, Gender speakGender, AudioMixerGroup mixer) : base(startTimeRange, new RadioTextContent(speakContent, speakRate, speakGender, mixer), RadioChannel.AllChannels) {
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
    
    protected override void OnPlayedWhenRadioOff() {
        
    }
}
