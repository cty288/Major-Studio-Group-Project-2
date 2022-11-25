using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class BodyTimeInfo {
    public int DayRemaining;
    public BodyInfo BodyInfo;

    public BodyTimeInfo(int dayRemaining, BodyInfo bodyInfo) {
        DayRemaining = dayRemaining;
        BodyInfo = bodyInfo;
    }
}

public struct OnNewBodyInfoGenerated {
    public List<BodyTimeInfo> BodyTimeInfos;
    public List<BodyTimeInfo> Aliens;
}

public class BodyManagmentSystem : AbstractSystem {
    public List<BodyTimeInfo> allBodyTimeInfos = new List<BodyTimeInfo>();

    public List<BodyTimeInfo> Aliens {
        get {
            return allBodyTimeInfos.FindAll(bodyTimeInfo => bodyTimeInfo.BodyInfo.IsAlien);
        }
    }

    public List<BodyTimeInfo> Humans {
        get {
            return allBodyTimeInfos.FindAll(bodyTimeInfo => !bodyTimeInfo.BodyInfo.IsAlien);
        }
    }

    private const int MaxBodyEveryDay = 3;
    protected override void OnInit() {
        this.RegisterEvent<OnNewDay>(OnNewDay);
    }

    public void RemoveBodyInfo(BodyInfo bodyInfo) {
        allBodyTimeInfos.RemoveAll(bodyTimeInfo => {

            if (bodyTimeInfo.BodyInfo.ID == bodyInfo.ID) {
                this.SendEvent<OnBodyInfoRemoved>(new OnBodyInfoRemoved() {ID = bodyTimeInfo.BodyInfo.ID});
                return true;
            }

            return false;
        });
    }
    /*
    public bool IsAlien(BodyInfo bodyInfo) {
        return Aliens.Exists(bodyTimeInfo => bodyTimeInfo.BodyInfo == bodyInfo);
    }*/
    private void OnNewDay(OnNewDay e) {
        foreach (BodyTimeInfo timeInfo in allBodyTimeInfos) {
            timeInfo.DayRemaining--;
        }

        allBodyTimeInfos.RemoveAll(timeInfo => {
            if (timeInfo.DayRemaining <= 0) {
                this.SendEvent<OnBodyInfoRemoved>(new OnBodyInfoRemoved() {ID = timeInfo.BodyInfo.ID});
                return true;
            }

            return false;
        });

        List<BodyTimeInfo> newBodyInfos = new List<BodyTimeInfo>();
        for (int i = 0; i < MaxBodyEveryDay; i++) {
            BodyInfo info = BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, false);
            BodyTimeInfo timeInfo = null;
            if (i == 0) {
                timeInfo = new BodyTimeInfo(3, info);
            }else {
                timeInfo = new BodyTimeInfo(Random.Range(0, 4), info);
            }
            allBodyTimeInfos.Add(timeInfo);
            newBodyInfos.Add(timeInfo);
            Debug.Log($"New Body Info Generated! Height: {info.Height}, VoiceType: {info.VoiceType}.");
        }
        //transform an alien
        BodyInfo selectedAlien = allBodyTimeInfos[Random.Range(0, allBodyTimeInfos.Count)].BodyInfo;
        selectedAlien.IsAlien = true;
        this.SendEvent<OnBodyInfoBecomeAlien>(new OnBodyInfoBecomeAlien() {ID = selectedAlien.ID});
        this.SendEvent<OnNewBodyInfoGenerated>(new OnNewBodyInfoGenerated() {
            BodyTimeInfos = newBodyInfos,
            Aliens = Aliens
        });

    }

}



public struct OnBodyInfoBecomeAlien {
    public long ID;
}

public struct OnBodyInfoRemoved {
    public long ID;
}