using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;


public class BountyHunterQuestFinishPhoneEvent: IncomingCallEvent {
    public BountyHunterQuestFinishPhoneEvent(TimeRange startTimeRange, TelephoneContact notificationContact, int callWaitTime) : base(startTimeRange, notificationContact, callWaitTime) {
    }
    [field: ES3Serializable]
    public override float TriggerChance { get; } = 1;
    public override void OnMissed() {
        OnMissedOrHangUp();
    }

    private void OnMissedOrHangUp() {
        DateTime today = gameTimeManager.CurrentTime.Value;
        DateTime targetNextTime = today.AddDays(1);
        DateTime targetTime = new DateTime(targetNextTime.Year, targetNextTime.Month, targetNextTime.Day, gameTimeManager.NightTimeStart,
            Random.Range(30, 60), 0);
        DateTime targetEndTime = new DateTime(targetTime.Year, targetTime.Month, targetTime.Day, 23, 58, 0);
        gameEventSystem.AddEvent(new BountyHunterQuestFinishPhoneEvent(new TimeRange(targetTime, targetEndTime), NotificationContact,
            callWaitTime));
    }

    protected override void OnConversationStart() {
      
    }

    protected override void OnComplete() {
        LoadCanvas.Singleton.Load(() => {
            DieCanvas.Singleton.Show("Game Ends", "You solved the crisis!");
        });
    }

    protected override void OnHangUp() {
        OnMissedOrHangUp();
    }
}


public class BountyHunterQuestFinishContact : TelephoneContact {
    public BountyHunterQuestFinishContact() {
        speaker = GameObject.Find("BountyHunterSpeaker").GetComponent<Speaker>();
        
    }
    public override bool OnDealt() {
        return true;
    }

    protected override void OnStart() {
        string welcome =
            "Good day, mister. I'm the chief of the Police Department of Dorcha. Congratulations on your glory achievement in protecting our community.";
        welcome +=
            "With your help in hunting down the dangerous creatures, we have successfully suppressed recent waves of infiltration in our neighborhood.";
        welcome +=
            "Thanks again for your contribution to our neighborhood.";
        speaker.Speak(welcome, AudioMixerList.Singleton.AudioMixerGroups[3], "???", OnSpeakEnd, 1.1f);
    }

    private void OnSpeakEnd() {
        EndConversation();
    }

    protected override void OnHangUp() {
        
    }

    protected override void OnEnd() {
       
    }
}

