using Crosstales.RTVoice.Model.Enum;
using UnityEngine.Audio;

namespace _02._Scripts.Radio.RadioScheduling {
	public class DorchaDailyRadioEvent: ScheduledRadioEvent<RadioTextContent> {
		
		[field: ES3Serializable]
		protected RadioTextContent radioContent { get; set; }

		public DorchaDailyRadioEvent(TimeRange startTimeRange, string speakContent, float speakRate, Gender speakGender,
			AudioMixerGroup mixer) :
			base(startTimeRange, new RadioTextContent(speakContent, speakRate, speakGender, mixer),
				RadioChannel.FM92) {
			
		}
		
		public DorchaDailyRadioEvent():base() {
			
		}


		protected override RadioTextContent GetRadioContent() {
			return radioContent;
		}

		protected override void SetRadioContent(RadioTextContent radioContent) {
			this.radioContent = radioContent;
		}

		protected override void OnRadioStart() {
			
		}

		protected override void OnPlayedWhenRadioOff() {
			
		}

		[field: ES3Serializable]
		protected override RadioProgramType ProgramType { get; set; } = RadioProgramType.DorchaDaily;
		[field: ES3Serializable]
		protected override bool DayEndAfterFinish { get; set; } = true;
		protected override ScheduledRadioEvent<RadioTextContent> OnGetNextRadioProgramMessage(TimeRange nextTimeRange, bool playSuccess) {
			return null;
		}
	}
}