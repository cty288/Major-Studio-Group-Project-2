using _02._Scripts.Radio.RadioScheduling.GhostStory;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine.Audio;

namespace _02._Scripts.Radio.RadioScheduling {
	public class GhostStoryRadioEvent: ScheduledRadioEvent<RadioDialogueContent> {
		
		[field: ES3Serializable]
		protected RadioDialogueContent radioContent { get; set; }

		public GhostStoryRadioEvent(TimeRange startTimeRange) :
			base(startTimeRange, null,
				RadioChannel.FM92) {
			
		}
		
		public GhostStoryRadioEvent():base() {
			
		}

		public override void OnStart() {
			base.OnStart();
			GhostStoryModel ghostStoryModel = this.GetModel<GhostStoryModel>();
			SetRadioContent(ghostStoryModel.GetRandomContent());
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
			return null;
		}
	}
}