using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameEvents.Merchant;
using MikroFramework.Architecture;
using Random = UnityEngine.Random;

public class MerchantSystem : AbstractSystem {

    private MerchantModel merchantModel;
    private GameTimeManager gameTimeManager;
    private GameEventSystem gameEventSystem;
    private TelephoneSystem telephoneSystem;
    protected override void OnInit() {
        gameTimeManager = this.GetSystem<GameTimeManager>();
        gameEventSystem = this.GetSystem<GameEventSystem>();
        telephoneSystem = this.GetSystem<TelephoneSystem>();
        merchantModel = this.GetModel<MerchantModel>();
        
        if(String.IsNullOrEmpty(merchantModel.PhoneNumber)) {
            merchantModel.PhoneNumber =  PhoneNumberGenor.GeneratePhoneNumber(7);
            merchantModel.PhoneNumberGenerationDate = Random.Range(2, 5);
            telephoneSystem.AddContact(merchantModel.PhoneNumber, new MerchantPhone());
            AddMerchantPhoneNumebrEvent();
        }
    }

    private void AddMerchantPhoneNumebrEvent() {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day + merchantModel.PhoneNumberGenerationDate, gameTimeManager.NightTimeStart,
            0, 0);
        gameEventSystem.AddEvent(new MerchantPhoneNumberEvent(new TimeRange(currentTime)));
    }
}
