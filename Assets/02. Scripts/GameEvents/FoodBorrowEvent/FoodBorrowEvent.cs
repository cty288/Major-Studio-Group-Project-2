using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.BodyOutside;
using _02._Scripts.ChoiceSystem;
using _02._Scripts.Dog;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02._Scripts.GameEvents.FoodBorrowEvent {
	public class FoodBorrowEvent: BodyGenerationEvent {
		[ES3Serializable] protected bool isScammer;

		protected FoodBorrowModel foodBorrowModel;
		protected PlayerResourceModel playerResourceModel;
		protected BodyModel bodyModel;
		
		protected int borrowFoodCount;
		public FoodBorrowEvent() : base() {
			foodBorrowModel = this.GetModel<FoodBorrowModel>();
			bodyModel = this.GetModel<BodyModel>();
			playerResourceModel = this.GetModel<PlayerResourceModel>();
		}
		
		public FoodBorrowEvent(TimeRange startTimeRange,  bool isScammer, float eventTriggerChance) :
			base(startTimeRange, null, eventTriggerChance) {
			this.isScammer = isScammer;
			foodBorrowModel = this.GetModel<FoodBorrowModel>();
			playerResourceModel = this.GetModel<PlayerResourceModel>();
			bodyModel = this.GetModel<BodyModel>();
			Debug.Log("A food borrow event is generated. The time is between " + startTimeRange.StartTime + " and " + startTimeRange.EndTime);
			
		}

		public override void OnStart() {
			base.OnStart();
			bodyInfo = bodyModel.GetBodyInfoByID(isScammer
				? foodBorrowModel.CurrentScammerId
				: foodBorrowModel.CurrentNonScammerId);
		}

		public override EventState OnUpdate() {
			if (!started) {
				if (bodyInfo == null || bodyInfo.IsDead || !playerResourceModel.HasEnoughResource<FoodResource>(1)) {
					AddNextWeekEvent();
					return EventState.End;
				}
			}
			return base.OnUpdate();
		}

		private void AddNextWeekEvent() {
		//	bool isNextScammer = Random.Range(0, 100) <= 40;
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			int nextDayTime = isScammer ? Random.Range(3, 7) : Random.Range(7, 10);
			DateTime nextTime = gameTimeModel.GetDay(gameTimeModel.Day + nextDayTime);
			nextTime = nextTime.AddMinutes(Random.Range(20, 100));

			this.GetSystem<GameEventSystem>().AddEvent(new FoodBorrowEvent(
				new TimeRange(nextTime, nextTime.AddMinutes(20)),
				isScammer, 1));
		}

		protected void AddTomorrowEvent() {
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime nextTime = gameTimeModel.GetDay(gameTimeModel.Day + 1);
			nextTime = nextTime.AddMinutes(Random.Range(20, 100));
			this.GetSystem<GameEventSystem>().AddEvent(new FoodBorrowEvent(
				new TimeRange(nextTime, nextTime.AddMinutes(20)),
				isScammer, 1));
		}

		
		public override void OnMissed() {
			//spawn one tomorrow
			AddTomorrowEvent();
		}

		protected override Func<bool> OnOpen() {
			onClickPeepholeSpeakEnd = false;
			int borrowMaxCount = Mathf.Min(playerResourceModel.GetResourceCount(typeof(FoodResource)), 3);
			borrowFoodCount = Random.Range(1, borrowMaxCount + 1);


			string borrowFoodStr = $"{borrowFoodCount} can{(borrowFoodCount > 1 ? "s" : "")}";
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			List<string> messages = new List<string>() {
				$"Sorry to barge in, but I'm about to keel over from hunger. Any chance you could hook me up <color=yellow>{borrowFoodStr}</color> of foods? " +
				"The monsters are making it tough to find enough to eat. I promise to return you double the amount next week!",
				$"I don't mean to be a bother, but I'm running on fumes over here. Could you loan me <color=yellow>{borrowFoodStr}</color> of your food? The monsters are making it harder and harder to find enough to survive." +
				"I promise to give back double the amount of food you lent me next week.",
			};
			
			
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			speaker.Speak(messages[UnityEngine.Random.Range(0, messages.Count)],
				bodyInfo.VoiceTag.VoiceGroup,
				"Food Borrower", 1, OnGreetingEnd,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);

			
			
			return () => onClickPeepholeSpeakEnd;
		}

		private void OnGreetingEnd(Speaker obj) {
			this.GetSystem<ChoiceSystem.ChoiceSystem>().StartChoiceGroup(new ChoiceGroup(ChoiceType.Outside,
				new ChoiceOption("- \"Sure!\"", OnYes),
				new ChoiceOption("- \"Sorry, I can't.\"", OnNo)));
		}

		private void OnNo(ChoiceOption obj) {
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			
			
			List<string> messages = new List<string>() {
			};

			if (isScammer) {
				messages.Add(
					"Seriously? You're gonna leave me out to dry like that? You're gonna regret it when the monsters come knocking.");
				messages.Add(
					"Wow, really? You're gonna be that stingy? You won't last long in this city with that attitude.");
			}
			else {
				messages.Add("I understand, no hard feelings. Thank you for considering it though.");
				messages.Add("I understand, it's a difficult situation. Thank you for your time and consideration.");
			}
			
			
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			speaker.Speak(messages[UnityEngine.Random.Range(0, messages.Count)],
				bodyInfo.VoiceTag.VoiceGroup,
				"Food Borrower", 1, OnSpeakEnd,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
		}

		private void OnYes(ChoiceOption obj) {
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			
			
			List<string> messages = new List<string>() {
				"You're a true saint, man. I'll make sure to make it up to you by giving back double the amount of food next week.",
				"Thank you for trusting me. I'll make sure to honor my word and give back double the amount of food next week."
			};
			
			
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			speaker.Speak(messages[UnityEngine.Random.Range(0, messages.Count)],
				bodyInfo.VoiceTag.VoiceGroup,
				"Food Borrower", 1, OnSpeakEnd,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);

			playerResourceModel.RemoveFood(borrowFoodCount);
			if (!isScammer) {
				GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
				this.GetSystem<GameEventSystem>().AddEvent(new GoodsRewardEvent(
					new TimeRange(gameTimeModel.GetDay(gameTimeModel.Day + Random.Range(5, 8))), new List<GoodsInfo>()
					{
						GoodsInfo.GetGoodsInfo(new FoodResource(), borrowFoodCount * 2),
						GoodsInfo.GetGoodsInfo(new BulletGoods(), 2)
					}, "Thanks for lending me food!", "Food Borrower"));
			}
		}

		private void OnSpeakEnd(Speaker obj) {
			onClickPeepholeSpeakEnd = true;
			AddNextWeekEvent();
		}

		protected override void OnNotOpen() {
			base.OnNotOpen();
			AddNextWeekEvent();
		}
		

		public override void OnEventEnd() {
		}
	}
}