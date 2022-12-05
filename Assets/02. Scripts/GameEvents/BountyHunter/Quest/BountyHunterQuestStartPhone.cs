using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class BountyHunterQuestStartPhone : TelephoneContact {
    protected BountyHunterPhone bountyHunterPhone;
    public BountyHunterQuestStartPhone() {
        speaker = GameObject.Find("BountyHunterSpeaker").GetComponent<Speaker>();
        bountyHunterPhone =
            telephoneSystem.Contacts[this.GetSystem<BountyHunterSystem>().PhoneNumber] as BountyHunterPhone;
    }
    public override bool OnDealt() {
        return !bountyHunterPhone.IsInJail;
    }

    protected override void OnStart() {
        string welcome = "Hey! My friend! This is the bounty hunter! I don��t know what kind of magic you have, but you have successfully helped me capture several monsters." +
                         " Based on your recent achievements, I��ve decided to give you a higher-level task.";
        welcome +=
            "I overheard a great job from my companion! A few days ago, a government official was attacked by an unknown creature! ";
        welcome +=
            "The police launched force to try to arrest it, but it seems to be more stealthy and agile than any other monsters of this kind! ";
        welcome +=
            "I have gathered some information about it, but I still don't know its physical characteristics. " +
            "I don't want my information to be leaked to other bounty hunters while I'm on the phone with you. So to keep it private, I'll keep you informed of the clues in various ways over the next few days. " +
            "Don't worry, I will call you ahead of time to let you know how I will deliver the message before I send you the lead.";        
        speaker.Speak(welcome, AudioMixerList.Singleton.AudioMixerGroups[2], OnSpeakEnd, 20);
    }

    private void OnSpeakEnd() {
        EndConversation();


    }

    protected override void OnHangUp() {
       
    }

    protected override void OnEnd() {
       
    }
}