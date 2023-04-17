using System;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using Random = UnityEngine.Random;

namespace _02._Scripts.GameEvents.FoodBorrowEvent {
	public class FoodBorrowSystem: AbstractSystem {
		protected FoodBorrowModel foodBorrowModel;
		protected override void OnInit() {
			foodBorrowModel = this.GetModel<FoodBorrowModel>();
			this.RegisterEvent<OnNewDay>(OnNewDay);
		}

		private void OnNewDay(OnNewDay e) {
			if (e.Day % 7 == 0) {
				GenerateNewBodies(false);
			}

			if (e.Day % 15 == 0) {
				GenerateNewBodies(true);
			}

			if (e.Day == 0) {
				//bool isNextScammer = Random.Range(0, 100) <= 40;
				GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
				DateTime nextScammerDayTime = gameTimeModel.GetDay(gameTimeModel.Day + Random.Range(3, 7))
					.AddMinutes(Random.Range(20, 100));
				
				DateTime nextNonScammerDayTime = gameTimeModel.GetDay(gameTimeModel.Day + Random.Range(7, 10))
					.AddMinutes(Random.Range(20, 100));

				this.GetSystem<GameEventSystem>().AddEvent(new FoodBorrowEvent(
					new TimeRange(nextScammerDayTime, nextScammerDayTime.AddMinutes(20)),
					true, 1));
				
				this.GetSystem<GameEventSystem>().AddEvent(new FoodBorrowEvent(
					new TimeRange(nextNonScammerDayTime, nextNonScammerDayTime.AddMinutes(20)),
					false, 1));
			}
		}

		private void GenerateNewBodies(bool isScammer) {
			BodyModel bodyModel = this.GetModel<BodyModel>();
			SuspectModel suspectModel = this.GetModel<SuspectModel>();

			if (isScammer) {
				BodyInfo scammer = BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, false, 0,
					new NormalKnockBehavior(3, Random.Range(5, 10), null, "Knock_BorrowFood"),
					bodyModel.AvailableBodyPartIndices,
					40);
			
				bodyModel.AddToManagedBodyInfos(scammer);
				foodBorrowModel.CurrentScammerId = scammer.ID;
				suspectModel.AddSuspect(scammer, "<size=200%>Food Scammer</size>\nDo not give them food!", GoodsInfo.GetGoodsInfo(new BulletGoods(), 2));
			}
			else {
				BodyInfo nonScammer = BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, false, 0,
					new NormalKnockBehavior(3, Random.Range(5, 10), null, "Knock_BorrowFood"),
					bodyModel.AvailableBodyPartIndices,
					40);
				bodyModel.AddToManagedBodyInfos(nonScammer);
				foodBorrowModel.CurrentNonScammerId = nonScammer.ID;
			}
			
			
			
		}
	}
}