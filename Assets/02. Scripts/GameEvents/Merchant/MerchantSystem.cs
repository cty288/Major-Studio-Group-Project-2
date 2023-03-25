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

            merchantModel.PhoneNumberGenerationDate = int.Parse(this.GetModel<HotUpdateDataModel>()
                .GetData("MerchantNoteDay").values[0]) + 1; //day 3 in actual game
            
            
            merchantModel.PhoneNumber =  PhoneNumberGenor.GeneratePhoneNumber(7);
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
