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
}

public class BodyManagmentSystem : AbstractSystem {
    private List<BodyTimeInfo> allBodyTimeInfos = new List<BodyTimeInfo>();
    private const int MaxBodyEveryDay = 3;
    protected override void OnInit() {
        this.RegisterEvent<OnNewDay>(OnNewDay);
    }

    private void OnNewDay(OnNewDay e) {
        foreach (BodyTimeInfo timeInfo in allBodyTimeInfos) {
            timeInfo.DayRemaining--;
        }

        allBodyTimeInfos.RemoveAll(timeInfo => timeInfo.DayRemaining <= 0);

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

        this.SendEvent<OnNewBodyInfoGenerated>(new OnNewBodyInfoGenerated() {BodyTimeInfos = newBodyInfos});

    }

}
