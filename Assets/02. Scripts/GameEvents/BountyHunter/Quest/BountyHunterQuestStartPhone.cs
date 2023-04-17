using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameEvents.BountyHunter;
using MikroFramework.Architecture;
using UnityEngine;

public class BountyHunterQuestStartPhone : TelephoneContact, ICanGetModel {
    protected BountyHunterModel bountyHunterModel;
    
    public BountyHunterQuestStartPhone() : base() {
        speaker = GameObject.Find("BountyHunterSpeaker").GetComponent<Speaker>();
        bountyHunterModel = this.GetModel<BountyHunterModel>();
    }
    public override bool OnDealt() {
        return !bountyHunterModel.IsInJail;
    }

    protected override void OnStart() {
        string welcome =
            "Hey! My friend! This is the bounty hunter! I don¡¯t know what kind of magic you have, but you have successfully helped me capture several monsters.";
                         welcome +=
            "And I overheard a great job from my companion! A few days ago, <color=yellow>a government official was attacked by an unknown monster!</color> ";
        welcome +=
            "The police launched force to try to arrest it, but <color=yellow>it seems to be more stealthy and agile</color> than any other monsters of this kind! ";
        welcome +=
            "I have gathered some information about it, but I still don't know its physical characteristics. " +
            "I don't want my information to be leaked to other bounty hunters while I'm on the phone with you. So to keep it private, I'll keep you informed of the clues in various ways over the next few days. " +
            "Don't worry, <color=yellow>I will call you ahead of time to let you know how I will deliver the message before I send you the lead.</color> Don't forget call me back when you find out the murderer!";        
        speaker.Speak(welcome, AudioMixerList.Singleton.AudioMixerGroups[2], "Bounty Hunter", 1f, OnSpeakEnd, 1.1f);
    }

    private void OnSpeakEnd(Speaker speaker) {
        EndConversation();


    }

    protected override void OnHangUp() {
       
    }

    protected override void OnEnd() {
       
    }
}
