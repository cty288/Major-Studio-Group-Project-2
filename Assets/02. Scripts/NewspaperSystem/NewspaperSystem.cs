using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;




public struct OnNewspaperGenerated {
    public Newspaper Newspaper;
}
public class NewspaperSystem : AbstractSystem {
    
    protected NewspaperModel newspaperModel;
    protected override void OnInit() {
        this.RegisterEvent<OnNewBodyInfoGenerated>(OnNewBodyInfoGenerated);
        newspaperModel = this.GetModel<NewspaperModel>();
    }

    private void OnNewBodyInfoGenerated(OnNewBodyInfoGenerated e) {
        
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
