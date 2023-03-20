using System;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;

namespace _02._Scripts.FashionCatalog {
	public class FashionCatalogSystem: AbstractSystem {
		private FashionCatalogModel model;
		private GameTimeModel gameTimeModel;
		protected override void OnInit() {
			this.RegisterEvent<BodyPartIndicesUpdateInfo>(OnBodyPartIndicesUpdate);
			model = this.GetModel<FashionCatalogModel>();
			gameTimeModel = this.GetModel<GameTimeModel>();
		}

		private void OnBodyPartIndicesUpdate(BodyPartIndicesUpdateInfo e) {
			if (gameTimeModel.CurrentTime.Value.DayOfWeek == DayOfWeek.Monday) {
				e.Time = gameTimeModel.CurrentTime.Value.Date;
				model.AddBodyPartIndicesUpdateInfo(e);
			}
		}
	}
}