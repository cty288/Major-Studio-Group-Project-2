using _02._Scripts.GameTime;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;

namespace _02._Scripts.Radio.RadioScheduling.DorchaDaily {
	public class DorchaDailySystem: AbstractSystem {
		
		private RadioSchedulingModel radioSchedulingModel;
		//private DayOfWeek importantNewsPaperDay = 0;
		private GameTimeModel gameTimeModel;
		private ImportantNewspaperModel importantNewspaperModel;
		protected override void OnInit() {
			this.RegisterEvent<OnNewDay>(OnNewDay);
			gameTimeModel = this.GetModel<GameTimeModel>();
			radioSchedulingModel = this.GetModel<RadioSchedulingModel>();
			importantNewspaperModel = this.GetModel<ImportantNewspaperModel>();
		}

		private void OnNewDay(OnNewDay e) {
			if ((e.Day - importantNewspaperModel.NewspaperStartDay)%7==0) {
				DorchaDailyProgram(gameTimeModel.Week);
			}
		}


		private void DorchaDailyProgram(int week) {
			HotUpdateDataModel hotUpdateDataModel = this.GetModel<HotUpdateDataModel>();
			HotUpdateData data = hotUpdateDataModel.GetData($"DorchaDaily_{week}");
			if (data != null) {
				TimeRange timeRange1 = radioSchedulingModel.AddToScheduled(
					new RadioScheduleInfo(new RadioProgramNameInfo(RadioProgramType.DorchaDaily, "Dorcha Daily"),
						RadioChannel.FM92, null, 60),
					gameTimeModel.CurrentTime.Value.AddDays(Random.Range(1, 3)), gameTimeModel.NightTimeStart);

				if (timeRange1 == null) {
					return;
				}
				Gender speakGender = Gender.MALE;
				AudioMixerGroup mixer = AudioMixerList.Singleton.AudioMixerGroups[7];
				float speakRate = Random.Range(1f, 1.5f);
				GameEventSystem gameEventSystem = this.GetSystem<GameEventSystem>();
				gameEventSystem.AddEvent(new DorchaDailyRadioEvent(timeRange1, data.values[0], speakRate, speakGender,
					mixer));
				
				TimeRange timeRange2 = radioSchedulingModel.AddToScheduled(
					new RadioScheduleInfo(new RadioProgramNameInfo(RadioProgramType.DorchaDaily, "Dorcha Daily (Replayed)"),
						RadioChannel.FM92, null, 60),
					gameTimeModel.CurrentTime.Value.AddDays(Random.Range(4, 7)), gameTimeModel.NightTimeStart);

				if (timeRange2 == null) {
					return;
				}
				
				gameEventSystem.AddEvent(new DorchaDailyRadioEvent(timeRange2, data.values[0], speakRate, speakGender,
					mixer));
			}
		}
	}
}