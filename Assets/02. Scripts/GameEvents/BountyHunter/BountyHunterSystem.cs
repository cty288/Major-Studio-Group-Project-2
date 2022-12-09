using System;
using System.Collections;
using System.Collections.Generic;
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

    //��������û��ȫ��͸¶�����������¼�������1���ᷢ����һ���¼�����ʧ�ܣ����������͸¶�¼�֮ǰ�����ٴη���
    //������ȫ��͸¶�����������¼�������2-3���ᷢ����һ���¼�����ʧ�ܣ����������͸¶�¼�֮ǰ�����ٴη���
    //��������ÿ�ζ��ǲ�һ���ģ���ʹ��ͬһ��ͷ���������ص����Ҳ�᲻һ����
    //ÿ���¼�ִ�гɹ��������ѷ������󣬵�0-3���ڻᷢ�����������Ϣ����Ϊ��ȷ�ʹ��󣩣�����ȷ��������Ϣһ����һ���ģ�ÿ����ȷ/�����¼����ټ��һ��
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
            BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, true));
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
