using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.ArmyEnding;
using _02._Scripts.ArmyEnding.InitialPhoneCalls;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.BodyOutside;
using _02._Scripts.ChoiceSystem;
using _02._Scripts.Dog;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02._Scripts.GameEvents.FoodBorrowEvent
{
	public class FoodBorrowEvent : BodyGenerationEvent
	{
		[ES3Serializable] protected bool isScammer;

		protected FoodBorrowModel foodBorrowModel;
		protected PlayerResourceModel playerResourceModel;
		protected BodyModel bodyModel;

		protected int borrowFoodCount;
		public FoodBorrowEvent() : base()
		{
			foodBorrowModel = this.GetModel<FoodBorrowModel>();
			bodyModel = this.GetModel<BodyModel>();
			playerResourceModel = this.GetModel<PlayerResourceModel>();
		}

		public FoodBorrowEvent(TimeRange startTimeRange, bool isScammer, float eventTriggerChance) :
			base(startTimeRange, null, eventTriggerChance)
		{
			this.isScammer = isScammer;
			foodBorrowModel = this.GetModel<FoodBorrowModel>();
			playerResourceModel = this.GetModel<PlayerResourceModel>();
			bodyModel = this.GetModel<BodyModel>();
			Debug.Log("A food borrow event is generated. The time is between " + startTimeRange.StartTime + " and " + startTimeRange.EndTime + ", " +
			          "and it is " + (isScammer ? "a scammer" : "not a scammer"));

		}

		public override void OnStart()
		{
			base.OnStart();
			bodyInfo = bodyModel.GetBodyInfoByID(isScammer
				? foodBorrowModel.CurrentScammerId
				: foodBorrowModel.CurrentNonScammerId);
		}

		public override EventState OnUpdate()
		{
			if (!started)
			{
				if (bodyInfo == null || bodyInfo.IsDead || !playerResourceModel.HasEnoughResource<FoodResource>(1))
				{
					AddNextWeekEvent();
					return EventState.End;
				}
			}
			return base.OnUpdate();
		}

		private void AddNextWeekEvent()
		{
			//	bool isNextScammer = Random.Range(0, 100) <= 40;
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			int nextDayTime = isScammer ? Random.Range(4, 8) : Random.Range(8, 10);
			DateTime nextTime = gameTimeModel.GetDay(gameTimeModel.Day + nextDayTime);
			nextTime = nextTime.AddMinutes(Random.Range(20, 100));

			this.GetSystem<GameEventSystem>().AddEvent(new FoodBorrowEvent(
				new TimeRange(nextTime, nextTime.AddMinutes(20)),
				isScammer, 1));
		}

		protected void AddMissedEvent() {
			int nextDayTime = isScammer ? Random.Range(2, 5) : Random.Range(1, 3);
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime nextTime = gameTimeModel.GetDay(gameTimeModel.Day + nextDayTime);
			nextTime = nextTime.AddMinutes(Random.Range(20, 100));
			this.GetSystem<GameEventSystem>().AddEvent(new FoodBorrowEvent(
				new TimeRange(nextTime, nextTime.AddMinutes(20)),
				isScammer, 1));
		}


		public override void OnMissed()
		{
			//spawn one tomorrow
			AddMissedEvent();
		}

		protected override Func<bool> OnOpen()
		{
			onClickPeepholeSpeakEnd = false;
			int borrowMaxCount = Mathf.Min(playerResourceModel.GetResourceCount(typeof(FoodResource)), 3);
			borrowFoodCount = Random.Range(1, borrowMaxCount + 1);


			string borrowFoodStr = $"{borrowFoodCount} can{(borrowFoodCount > 1 ? "s" : "")}";
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			List<string> messages = new List<string>() {
				$"Any chance you could hook me up <color=yellow>{borrowFoodStr}</color> of foods? " +
				"I promise to return you double next week!",
				$"Could you loan me <color=yellow>{borrowFoodStr}</color> of your food? " +
				"I promise to give back double next week. I really need your help!",
			};


			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			speaker.Speak(messages[UnityEngine.Random.Range(0, messages.Count)],
				bodyInfo.VoiceTag.VoiceGroup,
				"Food Borrower", 1, OnGreetingEnd,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);



			return () => onClickPeepholeSpeakEnd;
		}

		private void OnGreetingEnd(Speaker obj)
		{
			int foodCount = this.GetModel<PlayerResourceModel>().GetResourceCount(typeof(FoodResource));
			this.GetSystem<ChoiceSystem.ChoiceSystem>().StartChoiceGroup(new ChoiceGroup(ChoiceType.Outside,
				new ChoiceOption($"- \"Sure! (You have {foodCount} foods)\"", OnYes),
				new ChoiceOption("- \"Sorry, I can't.\"", OnNo)));
		}

		private void OnNo(ChoiceOption obj)
		{
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();


			List<string> messages = new List<string>()
			{
			};

			if (isScammer)
			{
				messages.Add(
					"Seriously? You're gonna leave me out to dry like that?");
				messages.Add(
					"Seriously? You won't last long in this city with that attitude.");
			}
			else
			{
				messages.Add("I understand, no hard feelings.");
				messages.Add("Thank you for your time and consideration.");
			}


			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			speaker.Speak(messages[UnityEngine.Random.Range(0, messages.Count)],
				bodyInfo.VoiceTag.VoiceGroup,
				"Food Borrower", 1, OnSpeakEnd,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
		}

		private void OnYes(ChoiceOption obj)
		{
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();


			List<string> messages = new List<string>() {
				"You're a true saint, man.",
				"Thank you for trusting me."
			};


			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			speaker.Speak(messages[UnityEngine.Random.Range(0, messages.Count)],
				bodyInfo.VoiceTag.VoiceGroup,
				"Food Borrower", 1, OnSpeakEnd,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);

			playerResourceModel.RemoveFood(borrowFoodCount);
			if (!isScammer)
			{
				GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
				this.GetSystem<GameEventSystem>().AddEvent(new GoodsRewardEvent(
					new TimeRange(gameTimeModel.GetDay(gameTimeModel.Day + Random.Range(5, 8))), new List<GoodsInfo>()
					{
						GoodsInfo.GetGoodsInfo(new FoodResource(), borrowFoodCount * 2),
						GoodsInfo.GetGoodsInfo(new BulletGoods(), 2)
					}, "Thanks for lending me food!", "Food Borrower"));
				ArmyEndingInitialCallCheck();
			}
		}
		
		private void ArmyEndingInitialCallCheck() {
			ArmyEndingModel armyEndingModel = this.GetModel<ArmyEndingModel>();
			if (!armyEndingModel.TriggeredStartPhoneCall) {
				armyEndingModel.TriggeredStartPhoneCall = true;

				GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
				DateTime nextTime = this.GetModel<GameTimeModel>().GetDay(gameTimeModel.Day + Random.Range(3, 5));
				nextTime = nextTime.AddMinutes(Random.Range(20, 60));
				


				this.GetSystem<GameEventSystem>().AddEvent(new FoodBorrowerArmyEndingInitialPhoneCall(
					new TimeRange(nextTime, nextTime.AddMinutes(60)),
					new FoodBorrowerArmyEndingInitialPhoneCallContact(bodyInfo.VoiceTag.VoiceGroup), 5));
			}
		}

		private void OnSpeakEnd(Speaker obj)
		{
			onClickPeepholeSpeakEnd = true;
			AddNextWeekEvent();
		}

		protected override void OnNotOpen()
		{
			base.OnNotOpen();
			AddNextWeekEvent();
		}


		public override void OnEventEnd()
		{
		}
	}
}