using System;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.GameTime;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using Random = UnityEngine.Random;

namespace _02._Scripts.CultEnding {
	public class CultEndingSystem: AbstractSystem {
		protected ImportantNewspaperModel importantNewsModel;
		protected ImportantNewsTextModel importantNewsTextModel;
		protected override void OnInit() {
			this.RegisterEvent<OnNewDay>(OnNewDay);
			importantNewsModel = this.GetModel<ImportantNewspaperModel>();
			importantNewsTextModel = this.GetModel<ImportantNewsTextModel>();
		}

		private void OnNewDay(OnNewDay e) {
			if (e.Day == 0) {
				this.GetModel<CultEndingModel>().InitModel();
				
				int motherNews1Day = Random.Range(4, 7);
				GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
				DateTime date = gameTimeModel.GetDay(motherNews1Day);


				int issue1Num = 2; //importantNewsModel.GetIssueForNews(motherNews1Day, date);
				importantNewsModel.AddPageToNewspaper(issue1Num,
					importantNewsTextModel.GetInfo("MotherNews_1"));

				importantNewsModel.AddPageToNewspaper(issue1Num + 1,
					importantNewsTextModel.GetInfo("MotherNews_2"));
				
				//add cult
				DateTime evTime = date.AddDays(3).AddMinutes(Random.Range(20, 80));
				this.GetSystem<GameEventSystem>().AddEvent(new CultKnockEvent(
					new TimeRange(evTime, evTime.AddMinutes(20)),
					GetCultBodyInfo(), 0));
			}
		}

		public static BodyInfo GetCultBodyInfo() {
			BodyPartPrefabInfo cultHead = AlienBodyPartCollections.Singleton.SpecialBodyPartPrefabs
				.HeightSubCollections[6].ShadowBodyPartPrefabs
				.HumanTraitPartsPrefabs[0].GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo(0);


			BodyInfo bodyInfo = BodyInfo.GetBodyInfo(null, null, cultHead, HeightType.Tall, new VoiceTag(
				AudioMixerList.Singleton.AudioMixerGroups[14], 1f, Gender.MALE), new NormalKnockBehavior(4, 8,
				null, "Knock_Cult"), BodyPartDisplayType.Shadow, false);

			return bodyInfo;
		}
	}
}