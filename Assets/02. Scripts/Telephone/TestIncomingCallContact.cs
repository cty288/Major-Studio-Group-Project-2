using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class TestIncomingCallContact : TelephoneContact {

    public TestIncomingCallContact() {
        speaker = GameObject.Find("BountyHunterSpeaker").GetComponent<Speaker>();
    }
    public override bool OnDealt() {
        return true;
    }

    protected override void OnStart() {
       
        string welcome = "Howdy! I'm the Bounty Hunter! I can reward you foods if you give me correct information about those creatures! Do you have any information about them?";
        speaker.Speak(welcome,null, "Bounty Hunter", 1f, Finish);
    }

    private void Finish(Speaker speaker) {
        EndConversation();
    }

    protected override void OnHangUp(bool h) {
      End();
    }

    protected override void OnEnd() {
       End();
    }

    private void End()
    {
      
    }
}
