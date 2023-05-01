using System;
using System.Text;
using _02._Scripts.FPSEnding;
using MikroFramework.Architecture;
using UnityEngine;
using Random = System.Random;


	public class BountyHunterClue3Event : BountyHunterMotherClueEvent{
		public BountyHunterClue3Event(TimeRange startTimeRange, BountyHunterMonsterClueContact notificationContact, int callWaitTime) 
			: base(startTimeRange, notificationContact, callWaitTime) {
		}

		public BountyHunterClue3Event(): base(){}

		
		
		protected override void OnConversationStart() {
			
		}

		protected override BountyHunterMotherClueEvent GetSameEvent(TimeRange startTimeRange, TelephoneContact contact, int callWaitTime) {
			return new BountyHunterClue3Event(startTimeRange, (BountyHunterClue3Contact) contact, callWaitTime);
		}

		protected override BountyHunterMotherClueEvent GetNextEvent(TimeRange startTimeRange, int callWaitTime) {
			return null;
		}
	}
	
	
	public class BountyHunterClue3Contact : BountyHunterMonsterClueContact
{
    protected override void OnStart() {
	    
        string description =
	        FashionCatalogUIPanel.GetShortDescription(this.GetModel<MonsterMotherModel>().MotherBodyTimeInfo.BodyInfo
		        .LegInfoPreab);

        
        if (description == "Naked Bottom") {
	        description = "nothing, not even any underwear";
        }

        description = description.ToLower();

        string welcome =
	        $"Hey, it's me again. I have some more information on Xenovore's physical appearance. I managed to catch a glimpse of its lower half: " +
	        $"<color=yellow>It is wearing {description}</color>.";
		        welcome +=
		        "We need to catch this thing before it causes any more damage.";
	        speaker.Speak(welcome, AudioMixerList.Singleton.AudioMixerGroups[2], "Bounty Hunter", 1f, OnSpeakEnd);
    }
    
    public BountyHunterClue3Contact(): base(){}

    private void OnSpeakEnd(Speaker speaker) {
	    EndConversation();
    }

    protected override void OnHangUp(bool hangUpByPlayer) {
       
    }

    protected override void OnEnd() {
        
    }
}
