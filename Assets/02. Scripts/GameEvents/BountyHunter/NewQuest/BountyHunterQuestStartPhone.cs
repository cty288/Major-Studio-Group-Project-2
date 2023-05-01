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
            "And I overheard a great job from my companion! A few days ago, the police launched force to try to arrest the <color=yellow>\"Xenovore\"</color> that posted on the newspaper.";
        welcome +=
            "I have gathered some information about it, but I still don't all of its physical characteristics. " +
            "<color=yellow>I will keep you updated on its characteristics these days by calling you</color>. <color=yellow>If you can provide me with a clear image of the \"Xenovore\", I will give you some extra bullets as a reward</color>. " +
            "I think this might be the crucial piece of information we need to end this monster crisis once and for all. If you manage to get a good photo, <color=yellow>just call me and I'll make sure you're rewarded</color>!";
            speaker.Speak(welcome, AudioMixerList.Singleton.AudioMixerGroups[2], "Bounty Hunter", 1f, OnSpeakEnd, 1.1f);
    }

    private void OnSpeakEnd(Speaker speaker) {
        EndConversation();


    }

    protected override void OnHangUp(bool hangUpByPlayer) {
       
    }

    protected override void OnEnd() {
       
    }
}
