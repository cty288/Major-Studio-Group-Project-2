using System;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02._Scripts.GameEvents.BountyHunter {
	public class BountyHunterAdPhoneCallEvent : IncomingCallEvent {
		public override float TriggerChance {
			get {
				if (bountyHunterModel.ContactedBountyHunter) {
					return 0;
				}

				return 1;
			}
		}

		protected BountyHunterModel bountyHunterModel;
		public BountyHunterAdPhoneCallEvent(TimeRange startTimeRange, TelephoneContact contact, int callWaitTime) : base(startTimeRange, contact, callWaitTime) {
			bountyHunterModel = this.GetModel<BountyHunterModel>();
			Debug.Log("BountyHunterAdPhoneCallEvent Start: " + startTimeRange.StartTime);
		}

		public BountyHunterAdPhoneCallEvent(): base() {
			bountyHunterModel = this.GetModel<BountyHunterModel>();
		}
		
		public override void OnMissed() {
			OnMissedOrHangUp();
		}

		protected override void OnConversationStart() {
			
		}

		protected override void OnComplete() {
			
		}

		protected override void OnHangUp() {
			OnMissedOrHangUp();
		}

		private void OnMissedOrHangUp() {
			if (bountyHunterModel.ContactedBountyHunter) {
				return;
			}

			DateTime nextTime = this.GetModel<GameTimeModel>().GetDay(gameTimeManager.Day + Random.Range(1, 3));
			nextTime = nextTime.AddMinutes(Random.Range(20, 60));
			gameEventSystem.AddEvent(new BountyHunterAdPhoneCallEvent(new TimeRange(nextTime, nextTime.AddMinutes(60)),
				NotificationContact, 5));


		}
	}
	
	
	
}