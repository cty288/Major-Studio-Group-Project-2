using System;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using UnityEngine;

namespace _02._Scripts.FashionCatalog {
	public class FashionCatalogSystem: AbstractSystem {
		private FashionCatalogModel model;
		private GameTimeModel gameTimeModel;
		private BodyModel bodyModel;
		protected override void OnInit() {
			this.RegisterEvent<BodyPartIndicesUpdateInfo>(OnBodyPartIndicesUpdate);
			this.RegisterEvent<OnNewDay>(OnNewDay);
			model = this.GetModel<FashionCatalogModel>();
			gameTimeModel = this.GetModel<GameTimeModel>();
			bodyModel = this.GetModel<BodyModel>();
		}

		private void OnNewDay(OnNewDay e) {
			if (e.Date.DayOfWeek == DayOfWeek.Sunday) {
				model.UpdateSpriteIndex();
			}
		}

		private void OnBodyPartIndicesUpdate(BodyPartIndicesUpdateInfo e) {
			if (gameTimeModel.CurrentTime.Value.DayOfWeek == DayOfWeek.Sunday) {
				
				e.Time = gameTimeModel.CurrentTime.Value.Date;
				model.AddBodyPartIndicesUpdateInfo(e);
				
			}
		}
	}
}