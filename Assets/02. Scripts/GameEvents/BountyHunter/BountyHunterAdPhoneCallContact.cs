using MikroFramework.Architecture;
using UnityEngine;

namespace _02._Scripts.GameEvents.BountyHunter {
	public class BountyHunterAdPhoneCallContact: TelephoneContact {
		protected BountyHunterModel bountyHunterModel;
    
		public BountyHunterAdPhoneCallContact() : base() {
			speaker = GameObject.Find("BountyHunterSpeaker").GetComponent<Speaker>();
			bountyHunterModel = this.GetModel<BountyHunterModel>();
		}

		public override bool OnDealt() {
			return !bountyHunterModel.ContactedBountyHunter;
		}

		protected override void OnStart() {
			string phoneNumberSeparatedBySpaces = bountyHunterModel.PhoneNumber;
			//add a space between each character
			phoneNumberSeparatedBySpaces = string.Join(" ", phoneNumberSeparatedBySpaces.ToCharArray());
			
			string welcome =
				"Hey there, fellow survivor! Are you tired of being chased by those pesky monsters? Well, fear not, cuz  I'm here to help! " +
				"I'm a badass bounty hunter, and I'm offering you a chance to make some serious cash. Just give me a call at " +
				$"<color=yellow>{phoneNumberSeparatedBySpaces}. I'll track them down and take care of business, while you sit back and enjoy your rewards. " +
				$"So whenever you find out any monsters, just pick up that phone and let's kick some monster butt!";
				speaker.Speak(welcome, AudioMixerList.Singleton.AudioMixerGroups[2], "Bounty Hunter", 1f, OnSpeakEnd, 1.1f);
		}

		private void OnSpeakEnd(Speaker obj) {
			this.GetModel<TelephoneNumberRecordModel>().AddOrEditRecord(bountyHunterModel.PhoneNumber, "Bounty Hunter");
			EndConversation();
		}

		protected override void OnHangUp() {
			
		}

		protected override void OnEnd() {
			
		}
	}
}