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
    protected int newspaperStartDay = 2;
    
    protected override void OnInit() {
        this.RegisterEvent<OnNewBodyInfoGenerated>(OnNewBodyInfoGenerated);
        newspaperModel = this.GetModel<NewspaperModel>();
        gameTimeModel = this.GetModel<GameTimeModel>();
        newspaperStartDay =
            int.Parse(this.GetModel<HotUpdateDataModel>().GetData("NewspaperDay").values[0]);
    }

    private void OnNewBodyInfoGenerated(OnNewBodyInfoGenerated e) {
        
        if (gameTimeModel.Day < newspaperStartDay) {
            return;
        }

        if (gameTimeModel.Day == newspaperStartDay) {
            DateTime currentTime = this.GetModel<GameTimeModel>().CurrentTime;
            DateTime eventTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 22, 40, 0);
            DateTime endTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 23, 59, 0);

            //this.GetSystem<GameEventSystem>().AddEvent(new NewspaperTutorialRadio(new TimeRange(eventTime, endTime),
                //AudioMixerList.Singleton.AudioMixerGroups[1]));
        }
        
        
        
        
        List<BodyTimeInfo> newsPaperBodyInfos = new List<BodyTimeInfo>();
        foreach (BodyTimeInfo bodyTimeInfo in e.BodyTimeInfos) {
                BodyInfo newsPaperInfo =
                BodyInfo.GetBodyInfoForDisplay(bodyTimeInfo.BodyInfo, BodyPartDisplayType.Newspaper, false);
                newsPaperBodyInfos.Add(new BodyTimeInfo(bodyTimeInfo.DayRemaining, newsPaperInfo, true));
        }

        newspaperModel.CreateNewspaper(this.GetSystem<GameTimeManager>().CurrentTime,
            newsPaperBodyInfos);
        
    }
}
