using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class ES3ReferencableObject<T> where T: ES3ReferencableObject<T> {
    public string guid;
    
    public ES3ReferencableObject(Guid guid) {
        this.guid = guid.ToString();
    }
    
    public ES3ReferencableObject() {
       
    }
}

public class Newspaper: ES3ReferencableObject<Newspaper> {
    public DateTime date;
    public string dateString;
    public List<BodyTimeInfo> timeInfos = new List<BodyTimeInfo>();
    
    public Newspaper(){}

    public Newspaper(Guid guid) : base(guid) {
        
    }
}

public struct OnNewspaperGenerated {
    public Newspaper Newspaper;
}
public class NewspaperSystem : AbstractSystem {
  
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

        Newspaper newspaper = new Newspaper(Guid.NewGuid())
            {date = this.GetSystem<GameTimeManager>().CurrentTime, timeInfos = newsPaperBodyInfos};

        this.SendEvent<OnNewspaperGenerated>(new OnNewspaperGenerated() {Newspaper = newspaper});

    }
}
