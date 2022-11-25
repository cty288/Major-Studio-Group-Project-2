using System;
using System.Collections;
using System.Collections.Generic;
using Crosstales;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public interface IPlayerResource {
    public string PrefabName { get; }

    public int MaxCount { get; }
}
public abstract class MerchantGoods: IPlayerResource {

    public abstract int BaseFoodPerUnit { get; }
    public int FoodPerUnit { get; protected set; }

    public MerchantGoods() {
        FoodPerUnit = BaseFoodPerUnit;
    }
    public abstract void RefreshFoodPerUnit();

    public abstract string SellSentence { get; }
    public abstract string PrefabName { get; }
    public abstract int MaxCount { get; }
}

public class BulletGoods : MerchantGoods {
    public override int BaseFoodPerUnit { get; } = 4;
   
    public override void RefreshFoodPerUnit() {
        FoodPerUnit = Random.Range(BaseFoodPerUnit - 1, BaseFoodPerUnit + 2);
    }

    public override string SellSentence {
        get {
            return $"1 Bullet. Price: {BaseFoodPerUnit} units of food";
        }
    }

    public override string PrefabName { get; } = "Bullet";
    public override int MaxCount { get; } = 6;
}
public class MerchantPhone : TelephoneContact {
    protected GameEventSystem gameEventSystem;    
    private Speaker merchantSpeaker;
    private GameTimeManager gameTimeManager;
    private PlayerResourceSystem playerResourceSystem;
    private float dailyAvailability = 0.7f;
    private bool isTodayAvailable = true;

    private List<MerchantGoods> GoodsList = new List<MerchantGoods>() {new BulletGoods()};
    private int dailyGoodsCount = 1;

    private List<MerchantGoods> todayGoods = new List<MerchantGoods>();

    private AudioMixerGroup mixer;
    public MerchantPhone() {
        merchantSpeaker = GameObject.Find("MerchantSpeaker").GetComponent<Speaker>();
        gameTimeManager = this.GetSystem<GameTimeManager>();
        playerResourceSystem = this.GetSystem<PlayerResourceSystem>();
        gameEventSystem = this.GetSystem<GameEventSystem>();
        this.mixer = merchantSpeaker.GetComponent<AudioSource>().outputAudioMixerGroup;
        gameTimeManager.OnDayStart += OnDayStart;
        RefreshAvailability();
        RefreshDailyGoods();
        
    }

    private void OnDayStart(int obj) {
        RefreshDailyGoods();
        RefreshAvailability();
    }

    private void RefreshAvailability() {
        isTodayAvailable = Random.Range(0f, 1f) <= dailyAvailability;
    }

    private void RefreshDailyGoods() {
        List<MerchantGoods> goodsListCopy = new List<MerchantGoods>(GoodsList);
        goodsListCopy.CTShuffle();
        todayGoods.Clear();
        for (int i = 0; i < dailyGoodsCount; i++) {
            if (goodsListCopy[i].MaxCount > playerResourceSystem.GetResourceCount(goodsListCopy[i].GetType())) {
                todayGoods.Add(goodsListCopy[i]);
                goodsListCopy[i].RefreshFoodPerUnit();
            }
           
        }
    }
    public override bool OnDealt() {
        return isTodayAvailable && todayGoods.Count > 0;
    }

    protected override void OnStart() {
        string welcome = "Hello, here is the best underground merchant in MK Town! The following list is the items sold today, press the corresponding number to buy: ";
        welcome += GetSellListSentence();
        merchantSpeaker.Speak(welcome, mixer,OnWelcomeSpeakFinished);
    }

    private void OnWelcomeSpeakFinished() {
        this.RegisterEvent<OnDialDigit>(OnDialDigit);
    }

    private void OnDialDigit(OnDialDigit e) {
        this.UnRegisterEvent<OnDialDigit>(OnDialDigit);
        if (e.Digit == 9) {
            merchantSpeaker.Speak(GetSellListSentence(), mixer, OnWelcomeSpeakFinished);
        }
        else {
            int index = e.Digit - 1;
            string reply = "";
            if (index >= todayGoods.Count) {
                 reply = "The number you dialed is invalid. Please try again or press 9 to repeat the list.";
            }else {
                MerchantGoods goods = todayGoods[index];
                if (goods.FoodPerUnit >= playerResourceSystem.FoodCount) {
                    reply =
                        "You don't have enough food to purchase this item. Please try again or press 9 to repeat the list.";
                }
                else {
                    playerResourceSystem.RemoveFood(goods.FoodPerUnit);
                   
                    reply =
                        "Thanks for your order. The item will be delivered to you tomorrow. You can continue shopping or press 9 to repeat the list.";

                    DateTime currentTime = gameTimeManager.CurrentTime.Value;
                    currentTime = currentTime.AddDays(1);
                    currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 22,
                        0, 0);
                    gameEventSystem.AddEvent(new GetResourceEvent(new BulletGoods(),1 ,new TimeRange(currentTime)));
                }
            }
            merchantSpeaker.Speak(reply, mixer, OnWelcomeSpeakFinished);
        }
    }

    protected string GetSellListSentence() {
        string welcome = "";
        for (int i = 0; i < todayGoods.Count; i++) {
            int index = i + 1;
            welcome += $"Number {index}.  {todayGoods[i].SellSentence}. ";
        }
        welcome += "To repeat this list, press 9.";
        return welcome;
    }

    protected override void OnHangUp() {
        StopSpeaking();
    }

    protected override void OnEnd() {
       StopSpeaking();
    }

    private void StopSpeaking() {
        merchantSpeaker.Stop();
        this.UnRegisterEvent<OnDialDigit>(OnDialDigit);
    }
}
