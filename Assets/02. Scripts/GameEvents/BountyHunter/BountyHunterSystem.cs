using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;
using Random = UnityEngine.Random;

public class BountyHunterSystem : AbstractSystem {
    public bool ContactedBountyHunter { get; set; } = false;
    public string PhoneNumber { get; private set; } = "";

    private GameTimeManager gameTimeManager;
    private GameEventSystem gameEventSystem;
    private TelephoneSystem telephoneSystem;

    public BindableProperty<bool> IsBountyHunting { get; } = new BindableProperty<bool>(false);

    public BodyTimeInfo QuestBodyTimeInfo { get; private set; }

    //当线索还没有全部透露：发送线索事件结束后，1天后会发送下一个事件；若失败，则会在线索透露事件之前尝试再次发送
    //当线索全部透露：发送线索事件结束后，2-3天后会发送下一个事件；若失败，则会在线索透露事件之前尝试再次发送
    //线索内容每次都是不一样的（即使是同一个头的描述，地点可能也会不一样）
    //每个事件执行成功（线索已发出）后，第0-3天内会发送相关线索信息（分为正确和错误），且正确的线索信息一定是一样的，每条正确/错误事件至少间隔一天
    public bool QuestBodyClueAllHappened { get; set; } = false;
    public bool QuestFinished { get; set; } = false;

    public int FalseClueCount { get; set; } = 0;
    public int MaxFalseClueCountForQuest = 3;
    public int FalseClueCountForPolice = 5;

    public DateTime NextAvailableDate { get; set; }

    public bool IsInJail { get; private set; }

    
    protected override void OnInit() {
      
        PhoneNumber = PhoneNumberGenor.GeneratePhoneNumber(6);
        gameTimeManager = this.GetSystem<GameTimeManager>();
        NextAvailableDate = gameTimeManager.CurrentTime.Value;
        gameEventSystem = this.GetSystem<GameEventSystem>();
        telephoneSystem = this.GetSystem<TelephoneSystem>();
        this.RegisterEvent<OnNewDay>(OnNewDay);
        telephoneSystem.AddContact(PhoneNumber, new BountyHunterPhone());
        Debug.Log("BountyHunterPhone: " + PhoneNumber);

        QuestBodyTimeInfo = new BodyTimeInfo(Random.Range(20, 30),
            BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, true, true,
                new NormalKnockBehavior(3, Random.Range(3, 7), new List<string>() { })));
        
    }

  
    private void OnNewDay(OnNewDay e) {
        if (gameTimeManager.Day == 1) {
            AddEvent();
        }

        if (gameTimeManager.CurrentTime.Value.Date >= NextAvailableDate) {
            IsInJail = false;
        }
    }

    public void GoToJail(int day) {
        NextAvailableDate = NextAvailableDate.AddDays(day);
        NextAvailableDate =
            new DateTime(NextAvailableDate.Year, NextAvailableDate.Month, NextAvailableDate.Day, 22, 0, 0);
        IsInJail = true;
    }
   

    private void AddEvent() {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        
        DateTime nextEventTime = new DateTime(currentTime.Year, currentTime.Month,
            currentTime.Day, Random.Range(22, 24), Random.Range(10, 40), 0);
        Debug.Log("Next Event Time: " + nextEventTime);

        gameEventSystem.AddEvent(new BountyHunterAdEvent(
            new TimeRange(nextEventTime, nextEventTime + new TimeSpan(0, 0, Random.Range(30, 50), 0, 0)),
            BountyHunterAdEvent.GetRandomAD(), 1, Gender.MALE, AudioMixerList.Singleton.AudioMixerGroups[0], 1));
    }
}
