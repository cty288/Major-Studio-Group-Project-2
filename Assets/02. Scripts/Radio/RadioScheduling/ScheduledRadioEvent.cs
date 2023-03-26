using System;
using _02._Scripts.Electricity;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using Random = UnityEngine.Random;

namespace _02._Scripts.Radio.RadioScheduling {
	public abstract class ScheduledRadioEvent<TRadioContent> : RadioEvent<TRadioContent> where TRadioContent: IRadioContent{
		
		protected abstract  RadioProgramType ProgramType { get; set; }
		protected RadioSchedulingModel radioschedulingModel;
		protected ScheduledRadioEvent(TimeRange startTimeRange, TRadioContent radioContent,
			RadioChannel channel) : base(startTimeRange, radioContent, channel) {
			radioschedulingModel = this.GetModel<RadioSchedulingModel>();
			
		}

		public ScheduledRadioEvent(): base() {
			radioschedulingModel = this.GetModel<RadioSchedulingModel>();
		}

		public override EventState OnUpdate() {
			DateTime currentTime = gameTimeManager.CurrentTime.Value;
			if (!radioschedulingModel.CheckIsProgramPlaying(currentTime, channel, ProgramType) && !started) {
				return EventState.Missed;
			}
			
			if (((currentTime.Hour==23 && currentTime.Minute>=55))) {
				if (!started) {
					return EventState.Missed;
				}

				if (radioModel.CurrentChannel.Value != channel || !electricityModel.HasElectricity() || !radioModel.IsOn) {
					this.SendEvent<OnRadioEnd>(new OnRadioEnd() {
						channel = channel
					});
					return EventState.End;
				}
			}

			if (radioModel.GetIsSpeaking(channel) && !started) {
				//AddNextRadioProgramMessage(false);
				return EventState.Missed;
			}


			if (!started && (!electricityModel.HasElectricity() || !radioModel.IsOn ||
			                 radioModel.CurrentChannel != channel)) {
				//still play the radio, but not the voice
				OnPlayedWhenRadioOff();
			}

			if (!started) {
				started = true;
				this.SendEvent<OnRadioProgramStart>(new OnRadioProgramStart() {
					radioContent = GetRadioContent(),
					channel = channel,
					// programType = programType
				});
				OnRadioStart();
			}

			if ((!radioModel.GetIsSpeaking(channel) || ended)&& !startDelayEnded) {
				startDelayEnded = true;
				delayEnded = false;
				this.GetSystem<ITimeSystem>().AddDelayTask(1f, () => {
					delayEnded = true;
				});
			} 
        
			if(delayEnded) {
				return EventState.End;
			}

			return EventState.Running;
		}

		public override float TriggerChance { get; } = 1;
		public override void OnEnd() {
			AddNextRadioProgramMessage(true);
		}

		public override void OnMissed() {
			AddNextRadioProgramMessage(false);
		}

		public void AddNextRadioProgramMessage(bool playSuccess) {
			DateTime currentTime = gameTimeManager.CurrentTime.Value;
			if (radioschedulingModel.CheckIsProgramPlaying(currentTime, channel, ProgramType)) {

				TimeRange nextTimeRange = new TimeRange(currentTime.AddMinutes(Random.Range(5, 10)));

				ScheduledRadioEvent<TRadioContent> nextRadioProgram = OnGetNextRadioProgramMessage(nextTimeRange, playSuccess);
				if (nextRadioProgram != null) {
					gameEventSystem.AddEvent(nextRadioProgram);
				}
			}
			else {
				DateTime nextTime = radioschedulingModel.FindNextAvailableTime(currentTime, channel, ProgramType);
				TimeRange nextTimeRange = null;
				if (nextTime != DateTime.MinValue) {
					nextTimeRange = new TimeRange(nextTime);
				}else {
					nextTimeRange = new TimeRange(currentTime.AddMinutes(Random.Range(10, 20)));
				}
				
				ScheduledRadioEvent<TRadioContent> nextRadioProgram = OnGetNextRadioProgramMessage(nextTimeRange, playSuccess);
				if (nextRadioProgram != null) {
					gameEventSystem.AddEvent(nextRadioProgram);
				}
			}
		}

		protected abstract ScheduledRadioEvent<TRadioContent> OnGetNextRadioProgramMessage(TimeRange nextTimeRange, bool playSuccess);
	}
}