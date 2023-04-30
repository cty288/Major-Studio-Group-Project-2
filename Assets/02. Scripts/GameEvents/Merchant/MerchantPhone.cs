using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.ChoiceSystem;
using _02._Scripts.GameEvents.Merchant;
using _02._Scripts.GameTime;
using Crosstales;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public interface IPlayerResource {
    public string PrefabName { get; }
    public int MaxCount { get; }
    public string DisplayName { get; }
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
    public abstract string DisplayName { get; }
}




public class BulletGoods : MerchantGoods {
    public override int BaseFoodPerUnit { get; } = 3;
   
    public override void RefreshFoodPerUnit() {
        FoodPerUnit = Random.Range(BaseFoodPerUnit - 1, BaseFoodPerUnit + 1);
    }

    public override string SellSentence {
        get {
            return $"1 Bullet. Price: {BaseFoodPerUnit} units of food";
        }
    }

    public override string PrefabName { get; } = "Bullet";
    public override int MaxCount { get; } = 6;
    public override string DisplayName { get; } = "Bullet";
}


public class PowerGeneratorGoods : MerchantGoods {
    public override int BaseFoodPerUnit { get; } = 1;
    public override void RefreshFoodPerUnit() {
        FoodPerUnit = Random.Range(BaseFoodPerUnit - 0, BaseFoodPerUnit + 2);
    }

    public override string SellSentence {
        get {
            return $"Portable Power Generator - I bet you might need it some time! Price: {BaseFoodPerUnit} units of food";
        }
    }

    public override string PrefabName { get; } = "PowerGenerator";
    public override int MaxCount { get; } = 1;
    public override string DisplayName { get; } = "Power Generator";
}



public class MerchantPhone : TelephoneContact, ICanGetModel {
    protected GameEventSystem gameEventSystem;    
   
    private GameTimeManager gameTimeManager;
    private PlayerResourceModel playerResourceModel;
    
    [ES3Serializable]
    private float dailyAvailability = 0.7f;
    [ES3Serializable]
    private bool isTodayAvailable = true;
   
    private List<MerchantGoods> GoodsList = new List<MerchantGoods>() {
        new BulletGoods(),
        new PowerGeneratorGoods()
    };
    [ES3Serializable]
    private int dailyGoodsCount = 2;
    [ES3Serializable]
    private List<MerchantGoods> todayGoods = new List<MerchantGoods>();

    private AudioMixerGroup mixer;

   // private Coroutine noResponseCoroutine = null;
    public MerchantPhone(): base() {
        speaker = GameObject.Find("MerchantSpeaker").GetComponent<Speaker>();
        gameTimeManager = this.GetSystem<GameTimeManager>((manager => {
            gameTimeManager = manager;
        } ));
        playerResourceModel = this.GetModel<PlayerResourceModel>();
        gameEventSystem = this.GetSystem<GameEventSystem>((system => {
            gameEventSystem = system;
        } ));
        this.mixer = speaker.GetComponent<AudioSource>().outputAudioMixerGroup;
        gameTimeManager.OnDayStart += OnDayStart;
        
        RefreshDailyGoods();
        
    }

    private void OnDayStart(int obj, int hour) {
        RefreshDailyGoods();
        RefreshAvailability();
    }

    private void RefreshAvailability() {
        isTodayAvailable = true;
    }

    private void RefreshDailyGoods() {
        List<MerchantGoods> goodsListCopy = new List<MerchantGoods>(GoodsList);
        goodsListCopy.CTShuffle();
        todayGoods.Clear();
        for (int i = 0; i < dailyGoodsCount; i++) {
            if (goodsListCopy[i].MaxCount > playerResourceModel.GetResourceCount(goodsListCopy[i].GetType())) {
                todayGoods.Add(goodsListCopy[i]);
                goodsListCopy[i].RefreshFoodPerUnit();
            }
           
        }
    }
    public override bool OnDealt() {
        return isTodayAvailable && todayGoods.Count > 0;
    }

    protected override void OnStart() {
        string welcome = "Hello, here is the best underground merchant in Dorcha! The following list is the items sold today, press the corresponding number to buy: ";
        speaker.Speak(welcome, mixer, "???", 1f, OnWelcomeSpeakFinished);
        
       
    }

    private void OnWelcomeSpeakFinished(Speaker speaker){
        
        this.RegisterEvent<OnDialDigit>(OnDialDigit);
        this.GetModel<TelephoneNumberRecordModel>()
            .AddOrEditRecord(this.GetModel<MerchantModel>().PhoneNumber, "Merchant");
        string welcome = GetSellListSentence();
        GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
       
        speaker.Speak(welcome, mixer, "Merchant", 1f, null);
        
        //gameTimeModel.CurrentTime.RegisterWithInitValue(OnTimeChanged);
        //noResponseCoroutine = CoroutineRunner.Singleton.StartCoroutine(OnTimeChanged());
        this.GetSystem<ChoiceSystem>().StartChoiceGroup(new ChoiceGroup(ChoiceType.Telephone, new ChoiceOption(
            "Hang up", (option) => {
                isTodayAvailable = false;
                this.UnRegisterEvent<OnDialDigit>(OnDialDigit);
                TelephoneSystem.HangUp(false);
            })));

    }

    private IEnumerator OnTimeChanged() {
        yield return new WaitForSeconds(60f);
        isTodayAvailable = false;
        this.UnRegisterEvent<OnDialDigit>(OnDialDigit);
        //this.GetModel<GameTimeModel>().CurrentTime.UnRegisterOnValueChanged(OnTimeChanged);
        TelephoneSystem.HangUp(false);
    }

    private void OnDialDigit(OnDialDigit e) {
      
        this.UnRegisterEvent<OnDialDigit>(OnDialDigit);
        if (speaker.IsSpeaking) {
            speaker.Stop(true);
        }
        if (e.Digit == 9) {
            speaker.Speak(GetSellListSentence(), mixer,"Merchant", 1f, OnWelcomeSpeakFinished);
        }
        else {
            int index = e.Digit - 1;
            string reply = "";
            if (index >= todayGoods.Count) {
                 reply = "The number you dialed is invalid. Please try again or press 9 to repeat the list.";
            }else {
                MerchantGoods goods = todayGoods[index];
                if (goods.FoodPerUnit > playerResourceModel.GetResourceCount<FoodResource>()) {
                    reply =
                        "You don't have enough food to purchase this item!";
                }
                else {
                    playerResourceModel.RemoveFood(goods.FoodPerUnit);
                    List<string> replies = new List<string>();
                    replies.Add("Thanks for your order. The item will be delivered to you tomorrow.");
                    replies.Add("There you go, sir! You’ve made the right decision! It will be delivered tomorrow. Good luck!");
                    reply = replies[Random.Range(0, replies.Count)];


                    DateTime currentTime = gameTimeManager.CurrentTime.Value;
                    currentTime = currentTime.AddDays(1);
                    currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, gameTimeManager.NightTimeStart,
                        0, 0);
                    gameEventSystem.AddEvent(new GetResourceEvent(goods,1 ,new TimeRange(currentTime)));
                }
            }
            speaker.Speak(reply, mixer, "Merchant", 1f, OnMerchantSpeakEnd);
        }
    }

    private void OnMerchantSpeakEnd(Speaker speaker) {
      
        EndConversation();
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

    protected override void OnHangUp(bool hangUpByPlayer) {
        StopSpeaking(true);
    }

    protected override void OnEnd() {
       StopSpeaking(true);
    }

    private void StopSpeaking(bool invokeEndCallback) {
        if (speaker.IsSpeaking) {
            speaker.Stop(invokeEndCallback);
        }
       
       // this.GetModel<GameTimeModel>().CurrentTime.UnRegisterOnValueChanged(OnTimeChanged);
        this.UnRegisterEvent<OnDialDigit>(OnDialDigit);
    }
}
