using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using Random = UnityEngine.Random;

public class MerchantSystem : AbstractSystem {
    private string phoneNumber;

    public string PhoneNumber => phoneNumber;
    private int phoneNumberGenerationDate;
    private GameTimeManager gameTimeManager;
    private GameEventSystem gameEventSystem;
    private TelephoneSystem telephoneSystem;
    protected override void OnInit() {
        gameTimeManager = this.GetSystem<GameTimeManager>();
        gameEventSystem = this.GetSystem<GameEventSystem>();
        telephoneSystem = this.GetSystem<TelephoneSystem>();
        
        phoneNumber = PhoneNumberGenor.GeneratePhoneNumber(10);
        phoneNumberGenerationDate = Random.Range(2, 5);

        AddMerchantPhoneNumebrEvent();

        telephoneSystem.AddContact(phoneNumber, new MerchantPhone());
    }

    private void AddMerchantPhoneNumebrEvent() {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day + phoneNumberGenerationDate, 22,
            0, 0);
        gameEventSystem.AddEvent(new MerchantPhoneNumberEvent(new TimeRange(currentTime)));
    }
}
