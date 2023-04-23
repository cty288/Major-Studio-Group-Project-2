using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;


public class FPSKillMonsterEndingPhoneEvent: IncomingCallEvent {
    public FPSKillMonsterEndingPhoneEvent(TimeRange startTimeRange, TelephoneContact notificationContact, int callWaitTime) : base(startTimeRange, notificationContact, callWaitTime) {
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
        gameEventSystem.AddEvent(new FPSKillMonsterEndingPhoneEvent(new TimeRange(targetTime, targetEndTime), NotificationContact,
            callWaitTime));
    }

    protected override void OnConversationStart() {
      
    }

    protected override void OnComplete() {
        LoadCanvas.Singleton.Load(() => {
            this.GetModel<GameStateModel>().GameState.Value = GameState.End;
            DieCanvas.Singleton.Show("Game Ends", "You solved the crisis!", false, false);
        });
    }

    protected override void OnHangUp(bool hangUpByPlayer) {
        
        OnMissedOrHangUp();
    }
}


public class FPSKillMonsterEndingPhoneContact : TelephoneContact {
    public FPSKillMonsterEndingPhoneContact() {
        speaker = GameObject.Find("TelephoneSpeaker").GetComponent<Speaker>();
        
    }
    public override bool OnDealt() {
        return true;
    }

    protected override void OnStart() {
        string welcome =
            "Good day, mister. I'm the chief of the Dorcha Military. " +
            "We heard from your neighbor that you killed a creature last night. " +
            "We investigated the area near your home and found the creature's body. " +
            "It turns out that it was the source of all the monsters. It's a rare type of creature called a \"Xenovore,\" " +
            "which means it consumes DNA and mutates it into different types of monsters. By killing the Xenovore, " +
            "we believe the other monsters will soon lose their activity with last night's success, " +
            "and the crisis is finally coming to an end. Thank you for your bravery, and please stay safe";
        
        
        
            speaker.Speak(welcome, AudioMixerList.Singleton.AudioMixerGroups[3], "???", 1f, OnSpeakEnd, 1.1f);
    }

    private void OnSpeakEnd(Speaker speaker) {
        EndConversation();
    }

    protected override void OnHangUp(bool hangUpByPlayer) {
        
    }

    protected override void OnEnd() {
       
    }
}

