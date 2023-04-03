using _02._Scripts.Radio.RadioScheduling.GhostStory;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine.Audio;

namespace _02._Scripts.Radio.RadioScheduling {
	public class GhostStoryRadioEvent: ScheduledRadioEvent<RadioDialogueContent> {
		
		[field: ES3Serializable]
		protected RadioDialogueContent radioContent { get; set; }

		public GhostStoryRadioEvent(TimeRange startTimeRange, RadioDialogueContent content) :
			base(startTimeRange, content,
				RadioChannel.FM92) {
			
		}
		
		public GhostStoryRadioEvent():base() {
			
		}



		protected override RadioDialogueContent GetRadioContent() {
			return radioContent;
		}

		protected override void SetRadioContent(RadioDialogueContent radioContent) {
			this.radioContent = radioContent;
		}

		protected override void OnRadioStart() {
			
		}

		protected override void OnPlayedWhenRadioOff() {
			
		}

		[field: ES3Serializable]
		protected override RadioProgramType ProgramType { get; set; } = RadioProgramType.GhostStory;
		[field: ES3Serializable]
		protected override bool DayEndAfterFinish { get; set; } = true;
		protected override ScheduledRadioEvent<RadioDialogueContent> OnGetNextRadioProgramMessage(TimeRange nextTimeRange, bool playSuccess) {
			if (!playSuccess) {
				return new GhostStoryRadioEvent(nextTimeRange, radioContent);
			}
			return null;
		}
	}
}