using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.GameTime;
using Crosstales;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

public class BodyTimeInfo {
    public int DayRemaining;
    public BodyInfo BodyInfo;
    public bool IsTodayDead;

    public BodyTimeInfo(int dayRemaining, BodyInfo bodyInfo, bool isTodayDead) {
        DayRemaining = dayRemaining;
        BodyInfo = bodyInfo;
        IsTodayDead = isTodayDead;
    }
}

public struct OnNewBodyInfoGenerated {
    public List<BodyTimeInfo> BodyTimeInfos;
    
}

public class BodyManagmentSystem : AbstractSystem {
    
    private BodyModel bodyModel;
    

    private const int MaxBodyEveryDay = 3;

    

    
    protected override void OnInit() {
        this.RegisterEvent<OnNewDay>(OnNewDay);
        bodyModel = this.GetModel<BodyModel>();
    }

    
    /*
    public bool IsAlien(BodyInfo bodyInfo) {
        return Aliens.Exists(bodyTimeInfo => bodyTimeInfo.BodyInfo == bodyInfo);
    }*/

    
    private void OnNewDay(OnNewDay e) {
        
        
        foreach (BodyTimeInfo timeInfo in bodyModel.allBodyTimeInfos) {
            timeInfo.DayRemaining--;
            timeInfo.IsTodayDead = false;
        }
        GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
        int bodyCount = MaxBodyEveryDay;
        if (gameTimeModel.Day <= 0) {
            bodyCount = 1;
            
            //prologue body
            BodyInfo info = BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, false, Random.Range(0.5f, 1f),
                new NormalKnockBehavior(3, int.MaxValue, null), bodyModel.AvailableBodyPartIndices, 40);
            bodyModel.AddNewBodyTimeInfoToNextDayDeterminedBodiesQueue(new BodyTimeInfo(1, info, true));
        }


        HashSet<BodyTimeInfo> removeSet = new HashSet<BodyTimeInfo>();
        foreach (BodyTimeInfo bodyTimeInfo in bodyModel.allBodyTimeInfos) {
            if (bodyTimeInfo.DayRemaining <= 0) {
                removeSet.Add(bodyTimeInfo);
               
               
            }
        }
       
        foreach (BodyTimeInfo bodyTimeInfo in removeSet) {
            bodyModel.KillBodyInfo(bodyTimeInfo.BodyInfo);
            bodyTimeInfo.BodyInfo.IsDead = true;
        }

        List<BodyTimeInfo> newBodyInfos = new List<BodyTimeInfo>();

        while (bodyModel.NextDayDeternimedBodies.Count > 0 && newBodyInfos.Count < bodyCount) {
            BodyTimeInfo bodyTimeInfo = bodyModel.NextDayDeternimedBodies[0];
            bodyModel.NextDayDeternimedBodies.RemoveAt(0);
            
            if (!bodyModel.allBodyTimeInfos.Contains(bodyTimeInfo)) {
                bodyModel.AddToAllBodyTimeInfos(bodyTimeInfo);
            }
          
            newBodyInfos.Add(bodyTimeInfo);
        }

        if (e.Date.DayOfWeek == DayOfWeek.Sunday) {
            int week = gameTimeModel.Week;
            bodyModel.UpdateAvailableBodyPartIndices(Mathf.RoundToInt(8 + 3 * Mathf.Log(week, (float) Math.E)));
        }
        
        
        for (int i = newBodyInfos.Count; i < bodyCount; i++) {
            BodyInfo info = BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, false, Random.Range(0.1f,0.9f),
                new NormalKnockBehavior(3, Random.Range(3, 7), null),bodyModel.AvailableBodyPartIndices,
                40);
            
            BodyTimeInfo timeInfo = null;
            if (i == 0) {
                timeInfo = new BodyTimeInfo(3, info, true);
            }else {
                timeInfo = new BodyTimeInfo(Random.Range(0, 4), info, true);
            }
            bodyModel.AddToAllBodyTimeInfos(timeInfo);
            newBodyInfos.Add(timeInfo);
            //Debug.Log($"New Body Info Generated! Height: {info.Height}, VoiceType: {info.VoiceType}.");
        }
        newBodyInfos.CTShuffle();
        //transform an alien
        BodyInfo selectedAlien = newBodyInfos[Random.Range(0, newBodyInfos.Count)].BodyInfo;
        selectedAlien.IsAlien = true;
        
        
        this.SendEvent<OnBodyInfoBecomeAlien>(new OnBodyInfoBecomeAlien() {ID = selectedAlien.ID});
        this.SendEvent<OnNewBodyInfoGenerated>(new OnNewBodyInfoGenerated() {
            BodyTimeInfos = newBodyInfos,
        });

    }

}



public struct OnBodyInfoBecomeAlien {
    public long ID;
}

public struct OnBodyInfoKilled {
    public long ID;
}