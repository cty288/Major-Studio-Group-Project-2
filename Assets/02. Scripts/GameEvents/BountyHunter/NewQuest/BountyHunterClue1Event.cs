using System;
using System.Text;
using _02._Scripts.FPSEnding;
using MikroFramework.Architecture;
using UnityEngine;
using Random = System.Random;

namespace _02._Scripts.GameEvents.BountyHunter.NewQuest {
	public class BountyHunterClue1Event : BountyHunterMotherClueEvent{
		public BountyHunterClue1Event(TimeRange startTimeRange, BountyHunterMonsterClueContact notificationContact, int callWaitTime) 
			: base(startTimeRange, notificationContact, callWaitTime) {
		}

		public BountyHunterClue1Event(): base(){}

		
		
		protected override void OnConversationStart() {
			
		}

		protected override BountyHunterMotherClueEvent GetSameEvent(TimeRange startTimeRange, TelephoneContact contact, int callWaitTime) {
			return new BountyHunterClue1Event(startTimeRange, (BountyHunterClue1Contact) contact, callWaitTime);
		}

		protected override BountyHunterMotherClueEvent GetNextEvent(TimeRange startTimeRange, int callWaitTime) {
			return new BountyHunterClue2Event(startTimeRange, new BountyHunterClue2Contact(), callWaitTime);
		}
	}
	
	
	public class BountyHunterClue1Contact : BountyHunterMonsterClueContact
{
    protected override void OnStart() {
	    
        string description =
	        FashionCatalogUIPanel.GetShortDescription(this.GetModel<MonsterMotherModel>().MotherBodyTimeInfo.BodyInfo
		        .HeadInfoPrefab);
	        
        description = description.ToLower();

        string welcome = $"Buddy, I got some clues about <color=yellow>the Xenovore's hairstyle</color>. I heard from my friend that <color=yellow>its hair is {description}</color>. ";
        welcome +=
	        "Also, it seems like the Xenovore is <color=yellow>much stronger than other monsters</color>, and it's not like one bullet can take it down. " +
	        "It's best to keep your distance and snap a photo from a safe spot.";
        speaker.Speak(welcome, AudioMixerList.Singleton.AudioMixerGroups[2], "Bounty Hunter", 1f, OnSpeakEnd);
    }
    
    public BountyHunterClue1Contact(): base(){}

    private void OnSpeakEnd(Speaker speaker) {
	    EndConversation();
    }

    protected override void OnHangUp(bool hangUpByPlayer) {
       
    }

    protected override void OnEnd() {
        
    }
}
}