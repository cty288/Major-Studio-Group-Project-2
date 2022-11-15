using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class Newspaper {
    public DateTime date;
    public string dateString;
    public List<BodyTimeInfo> timeInfos = new List<BodyTimeInfo>();
}

public struct OnNewspaperGenerated {
    public Newspaper Newspaper;
}
public class NewspaperSystem : AbstractSystem {
   // public List<Newspaper> SavedNewspapers = new List<Newspaper>();
   public NewspaperViewController CurrentHoldingNewspaper;
    protected override void OnInit() {
        this.RegisterEvent<OnNewBodyInfoGenerated>(OnNewBodyInfoGenerated);
    }

    private void OnNewBodyInfoGenerated(OnNewBodyInfoGenerated e) {
        List<BodyTimeInfo> newsPaperBodyInfos = new List<BodyTimeInfo>();
        foreach (BodyTimeInfo bodyTimeInfo in e.BodyTimeInfos) {
            BodyInfo newsPaperInfo =
                BodyInfo.GetBodyInfoForDisplay(bodyTimeInfo.BodyInfo, BodyPartDisplayType.Newspaper);
            newsPaperBodyInfos.Add(new BodyTimeInfo(bodyTimeInfo.DayRemaining, newsPaperInfo));
        }

        Newspaper newspaper = new Newspaper()
            {date = this.GetSystem<GameTimeManager>().CurrentTime, timeInfos = newsPaperBodyInfos};

        this.SendEvent<OnNewspaperGenerated>(new OnNewspaperGenerated() {Newspaper = newspaper});

    }
}
