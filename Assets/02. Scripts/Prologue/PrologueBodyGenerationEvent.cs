using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using UnityEngine;


namespace _02._Scripts.Prologue {
	public class PrologueBodyGenerationEvent: BodyGenerationEvent {
		public PrologueBodyGenerationEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float eventTriggerChance) : base(startTimeRange, bodyInfo, eventTriggerChance) {
		}
		
		public PrologueBodyGenerationEvent():base() {
			
		}
		public override EventState OnUpdate() {
			
			if (!started) {
				if (bodyGenerationModel.CurrentOutsideBody.Value != null) {
					OnNotOpen();
					return EventState.End;
				}
				started = true;
				knockDoorCheckCoroutine = CoroutineRunner.Singleton.StartCoroutine(KnockDoorCheck());
			}

			return (bodyGenerationModel.CurrentOutsideBody.Value == null && !bodyGenerationModel.CurrentOutsideBodyConversationFinishing) ? EventState.End : EventState.Running;
		}

		public override void OnMissed() {
			
		}

		protected override Func<bool> OnOpen() {
			onClickPeepholeSpeakEnd = false;
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			LoadCanvas.Singleton.ShowImage(0, 0.2f);
			List<string> messages = new List<string>() {
				"goOD dAy sIR. buT iT'S yOuR tiME!",
				"hI, Hi! iT IS yOur tiMe!",
				"YOuR TimE IS oVeR!"
			};
			speaker.Speak(messages[UnityEngine.Random.Range(0, messages.Count)], AudioMixerList.Singleton.AudioMixerGroups[4], "???", OnAlienClickedOutside);
			return () => onClickPeepholeSpeakEnd;
		}
		
		
		protected void OnAlienClickedOutside() {
			
			LoadCanvas.Singleton.HideImage(1f);
			//DieCanvas.Singleton.Show("", true);
			//this.GetModel<GameStateModel>().GameState.Value = GameState.End;
			this.GetSystem<BodyGenerationSystem>().StopCurrentBody();
			this.GetSystem<ITimeSystem>().AddDelayTask(2f, PrologueSpeak);

		}

		private void PrologueSpeak() {
			Speaker speaker = GameObject.Find("PrologueSpeaker").GetComponent<Speaker>();
			HotUpdateDataModel hotUpdateDataModel = this.GetModel<HotUpdateDataModel>();
			string content = hotUpdateDataModel.GetData("Opening").values[1];
			speaker.Speak(content, AudioMixerList.Singleton.AudioMixerGroups[5], "Radio", OnPrologueSpeakEnd, 1.2f);
		}

		private void OnPrologueSpeakEnd() {
			onClickPeepholeSpeakEnd = true;
			DieCanvas.Singleton.Show("","", true);
			this.GetSystem<ITimeSystem>().AddDelayTask(2.4f, () => {
				DieCanvas.Singleton.Hide();
			});
		}

		public override void OnEventEnd() {
			
		}
	}
}