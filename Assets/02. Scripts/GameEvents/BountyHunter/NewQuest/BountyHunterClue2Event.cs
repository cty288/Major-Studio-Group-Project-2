using System;
using System.Text;
using _02._Scripts.FPSEnding;
using MikroFramework.Architecture;
using UnityEngine;
using Random = System.Random;

namespace _02._Scripts.GameEvents.BountyHunter.NewQuest {
	public class BountyHunterClue2Event : BountyHunterMotherClueEvent{
		public BountyHunterClue2Event(TimeRange startTimeRange, BountyHunterMonsterClueContact notificationContact, int callWaitTime) 
			: base(startTimeRange, notificationContact, callWaitTime) {
		}

		public BountyHunterClue2Event(): base(){}

		
		
		protected override void OnConversationStart() {
			
		}

		protected override BountyHunterMotherClueEvent GetSameEvent(TimeRange startTimeRange, TelephoneContact contact, int callWaitTime) {
			return new BountyHunterClue2Event(startTimeRange, (BountyHunterClue2Contact) contact, callWaitTime);
		}

		protected override BountyHunterMotherClueEvent GetNextEvent(TimeRange startTimeRange, int callWaitTime) {
			return new BountyHunterClue3Event(startTimeRange, new BountyHunterClue3Contact(), callWaitTime);
		}
	}
	
	
	public class BountyHunterClue2Contact : BountyHunterMonsterClueContact
{
    protected override void OnStart() {
	    
        string description =
	        FashionCatalogUIPanel.GetShortDescription(this.GetModel<MonsterMotherModel>().MotherBodyTimeInfo.BodyInfo
		        .MainBodyInfoPrefab);
	        
        description = description.ToLower();

        string welcome = $"Hey there, my friend! I have some new info about the <color=yellow>Xenovore's clothing</color> that I want to share with you. " +
                         $"According to eyewitnesses, <color=yellow>the Xenovore is often seen wearing a {description} on its upper body</color>.";
        welcome +=
	        "Also, <color=yellow>My sources tell me that the Xenovore always wears the same clothing</color>, so it should be easy to spot once you know what to look for.";
	        speaker.Speak(welcome, AudioMixerList.Singleton.AudioMixerGroups[2], "Bounty Hunter", 1f, OnSpeakEnd);
    }
    
    public BountyHunterClue2Contact(): base(){}

    private void OnSpeakEnd(Speaker speaker) {
	    EndConversation();
    }

    protected override void OnHangUp(bool hangUpByPlayer) {
       
    }

    protected override void OnEnd() {
        
    }
}
}