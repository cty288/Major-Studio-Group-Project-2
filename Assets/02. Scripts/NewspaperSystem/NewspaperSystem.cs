using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using UnityEngine;




public struct OnNewspaperGenerated {
    public Newspaper Newspaper;
}
public class NewspaperSystem : AbstractSystem {
    
    protected NewspaperModel newspaperModel;
    protected GameTimeModel gameTimeModel;
    
    protected override void OnInit() {
        this.RegisterEvent<OnNewBodyInfoGenerated>(OnNewBodyInfoGenerated);
        newspaperModel = this.GetModel<NewspaperModel>();
        gameTimeModel = this.GetModel<GameTimeModel>();
    }

    private void OnNewBodyInfoGenerated(OnNewBodyInfoGenerated e) {
        if (gameTimeModel.Day == 0) {
            return;
        }
        List<BodyTimeInfo> newsPaperBodyInfos = new List<BodyTimeInfo>();
        foreach (BodyTimeInfo bodyTimeInfo in e.BodyTimeInfos) {
                BodyInfo newsPaperInfo =
                BodyInfo.GetBodyInfoForDisplay(bodyTimeInfo.BodyInfo, BodyPartDisplayType.Newspaper);
            newsPaperBodyInfos.Add(new BodyTimeInfo(bodyTimeInfo.DayRemaining, newsPaperInfo));
        }

        newspaperModel.CreateNewspaper(this.GetSystem<GameTimeManager>().CurrentTime,
            newsPaperBodyInfos);
        
    }
}
