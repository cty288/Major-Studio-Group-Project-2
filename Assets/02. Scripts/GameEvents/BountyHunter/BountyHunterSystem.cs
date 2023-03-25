using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.GameEvents.BountyHunter;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;
using Random = UnityEngine.Random;

public class BountyHunterSystem : AbstractSystem {
  

    private GameTimeManager gameTimeManager;
    private GameEventSystem gameEventSystem;
    private TelephoneSystem telephoneSystem;
    private PlayerControlModel playerControlModel;

    //public BindableProperty<bool> IsBountyHunting { get; } = new BindableProperty<bool>(false);

   

    //当线索还没有全部透露：发送线索事件结束后，1天后会发送下一个事件；若失败，则会在线索透露事件之前尝试再次发送
    //当线索全部透露：发送线索事件结束后，2-3天后会发送下一个事件；若失败，则会在线索透露事件之前尝试再次发送
    //线索内容每次都是不一样的（即使是同一个头的描述，地点可能也会不一样）
    //每个事件执行成功（线索已发出）后，第0-3天内会发送相关线索信息（分为正确和错误），且正确的线索信息一定是一样的，每条正确/错误事件至少间隔一天
    
    private BountyHunterModel bountyHunterModel;

    
    protected override void OnInit() {

        bountyHunterModel = this.GetModel<BountyHunterModel>();
        gameTimeManager = this.GetSystem<GameTimeManager>();

        if (bountyHunterModel.NextAvailableDate == default(DateTime)) {
            bountyHunterModel.NextAvailableDate = gameTimeManager.CurrentTime.Value;
        }
       
        
        gameEventSystem = this.GetSystem<GameEventSystem>();
        telephoneSystem = this.GetSystem<TelephoneSystem>();
        playerControlModel = this.GetModel<PlayerControlModel>();
        
        
        this.RegisterEvent<OnNewDay>(OnNewDay);
        if (!telephoneSystem.Contacts.ContainsKey(bountyHunterModel.PhoneNumber)) {
            telephoneSystem.AddContact(bountyHunterModel.PhoneNumber, new BountyHunterPhone());
        }
       
        Debug.Log("BountyHunterPhone: " + bountyHunterModel.PhoneNumber);

        
        
    }

  
    private void OnNewDay(OnNewDay e) {
        if (gameTimeManager.Day == 1) {
            AddEvent();
        }

        if (gameTimeManager.CurrentTime.Value.Date >= bountyHunterModel.NextAvailableDate) {
            bountyHunterModel.IsInJail = false;
        }
    }

    public void GoToJail(int day) {
        bountyHunterModel.NextAvailableDate = bountyHunterModel.NextAvailableDate.AddDays(day);
        bountyHunterModel. NextAvailableDate =
            new DateTime(bountyHunterModel.NextAvailableDate.Year, bountyHunterModel.NextAvailableDate.Month, bountyHunterModel.NextAvailableDate.Day, gameTimeManager.NightTimeStart, 0, 0);
        bountyHunterModel.IsInJail = true;
    }
   

    private void AddEvent() {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        
        HotUpdateDataModel hotUpdateDataModel = this.GetModel<HotUpdateDataModel>();
        string[] days = hotUpdateDataModel.GetData("BountyHunterDay").values;
        int minDay = int.Parse(days[0]);
        int maxDay = int.Parse(days[1]);

        DateTime nextEventTime = new DateTime(currentTime.Year, currentTime.Month,
            currentTime.Day + Random.Range(minDay,maxDay), Random.Range(gameTimeManager.NightTimeStart, 24), Random.Range(10, 40), 0);
        Debug.Log("Next Event Time: " + nextEventTime);

        gameEventSystem.AddEvent(new BountyHunterAdEvent(
            new TimeRange(nextEventTime, nextEventTime + new TimeSpan(0, 0, Random.Range(30, 50), 0, 0)),
            BountyHunterAdEvent.GetRandomAD(), 1, Gender.MALE, AudioMixerList.Singleton.AudioMixerGroups[0], 1));
    }
}
