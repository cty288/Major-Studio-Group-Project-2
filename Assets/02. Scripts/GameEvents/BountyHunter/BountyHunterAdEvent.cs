using System;
using System.Collections;
using System.Collections.Generic;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class BountyHunterAdEvent : RadioEvent {
    private BountyHunterSystem bountyHunterSystem;
    private TelephoneSystem telephoneSystem;
    private string phoneNumber;
    private float triggerChance;
   public BountyHunterAdEvent(TimeRange startTimeRange, string speakContent, float speakRate, Gender speakGender, AudioMixerGroup mixer, float triggerChance) : base(startTimeRange, speakContent, speakRate, speakGender, mixer) {
        //this.TriggerChance = triggerChance;
        telephoneSystem = this.GetSystem<TelephoneSystem>();
        bountyHunterSystem = this.GetSystem<BountyHunterSystem>();
        gameTimeManager = this.GetSystem<GameTimeManager>();
        phoneNumber = bountyHunterSystem.PhoneNumber;
        this.triggerChance = triggerChance;
   }

   public override float TriggerChance {
       get {
            BountyHunterPhone phone = telephoneSystem.Contacts[phoneNumber] as BountyHunterPhone;
            if (phone.GetAvailable()) {
                return triggerChance;
            }

            return 0;
       }
   }

   public override void OnEnd() {
        OnStopOrMissed();
    }

    public override void OnMissed() {
        OnStopOrMissed();
    }

    private void OnStopOrMissed() {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        int nextDayInterval = bountyHunterSystem.ContactedBountyHunter ? 1 : 3;

        DateTime nextEventDay = currentTime.AddDays(nextDayInterval);
        DateTime nextEventTime = new DateTime(nextEventDay.Year, nextEventDay.Month,
            nextEventDay.Day, Random.Range(22,24), Random.Range(0, 40), 0);
        DateTime nextEventTimeRange2 = nextEventTime.AddMinutes(Random.Range(30, 60));
      
        gameEventSystem.AddEvent(new BountyHunterAdEvent(new TimeRange(nextEventTime, nextEventTimeRange2), 
            GetRandomAD(), 1, Gender.MALE, AudioMixerList.Singleton.AudioMixerGroups[0], 1));
    }
    protected override void OnRadioStart() {
        
    }

    public static string GetRandomAD() {
        BountyHunterSystem bountyHunterSystem = MainGame.Interface.GetSystem<BountyHunterSystem>();
        string phoneNumber = bountyHunterSystem.PhoneNumber;

        //make a new string that has a space between every character
        string spacedPhoneNumber = string.Join(" ", phoneNumber.ToCharArray());
        

        List<string> ads = new List<string>();
        ads.Add(
            $"Ever wanted to earn some extra foods? Help out our bounty hunters! Help defend our sweet motherland from the disgusting creatures! Dial {spacedPhoneNumber}! Repeat, deal {spacedPhoneNumber}!");
        ads.Add($"Getting troubled by suspicious ¡°human-beings¡±? Call the bounty hunters and we will help you! Dial {spacedPhoneNumber}! Repeat, {spacedPhoneNumber}!");
        return ads[Random.Range(0, ads.Count)];
    }
}
