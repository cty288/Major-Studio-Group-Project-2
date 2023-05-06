using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using UnityEngine;


namespace _02._Scripts.Prologue {
	public struct OnIntroSceneHide {
		
	}

	public struct OnIntroSceneShowImage {
		public int ImageIndex;
	}
	public class PrologueBodyGenerationEvent: BodyGenerationEvent {

		internal class PrologueSpeakInfo {
			public string Content;
			public int ImageIndex;
			
			public PrologueSpeakInfo(string content, int imageIndex) {
				Content = content;
				ImageIndex = imageIndex;
			}
		}

		[field: ES3Serializable]
		protected override bool freezeTimeWhenOpenDoor { get; set; } = false;

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
			speaker.Speak(messages[UnityEngine.Random.Range(0, messages.Count)], AudioMixerList.Singleton.AudioMixerGroups[4], "???", 1f, OnAlienClickedOutside);
			return () => onClickPeepholeSpeakEnd;
		}
		
		
		protected void OnAlienClickedOutside(Speaker speaker) {
			
			LoadCanvas.Singleton.HideImage(1f);
			//DieCanvas.Singleton.Show("", true);
			//this.GetModel<GameStateModel>().GameState.Value = GameState.End;
			this.GetSystem<BodyGenerationSystem>().StopCurrentBody();
			this.GetSystem<ITimeSystem>().AddDelayTask(2f, PrologueSpeak);

		}

		private void PrologueSpeak() {
			Speaker speaker = GameObject.Find("PrologueSpeaker").GetComponent<Speaker>();
			//HotUpdateDataModel hotUpdateDataModel = this.GetModel<HotUpdateDataModel>();
			//string content = hotUpdateDataModel.GetData("Opening").values[Application.isEditor ? 0 : 1];
			List<PrologueSpeakInfo> speakInfos = ConstructPrologueSpeakInfo();
			PrologueDoSpeak(speakInfos, 0, speaker);
			
		}

		private void PrologueDoSpeak(List<PrologueSpeakInfo> infos, int index, Speaker speaker) {
			if(index >= infos.Count) {
				this.SendEvent<OnIntroSceneHide>();
				OnPrologueSpeakEnd(speaker);
				return;
			}
			this.SendEvent<OnIntroSceneShowImage>(new OnIntroSceneShowImage() {
				ImageIndex = infos[index].ImageIndex
			});
			this.GetSystem<ITimeSystem>().AddDelayTask(0.1f, () => {
				speaker.Speak(infos[index].Content, AudioMixerList.Singleton.AudioMixerGroups[5], "Radio", 1f,
					(speaker) => {
						PrologueDoSpeak(infos, index + 1, speaker);
					}, 1.1f);
			});

		}


		private List<PrologueSpeakInfo> ConstructPrologueSpeakInfo() {
			List<PrologueSpeakInfo> prologueSpeakInfos = new List<PrologueSpeakInfo>();
			prologueSpeakInfos.Add(new PrologueSpeakInfo(
				"Attention, citizens of Dorcha.There have been reports circulating about some  <color=yellow>mysterious creatures </color> that have been attempting to  <color=yellow>disguise itself as humans </color> and infiltrate our neighborhoods.",
				0));

			prologueSpeakInfos.Add(new PrologueSpeakInfo(
				"We do not know the creature's targets yet, but I assure you we are taking swift action against these infiltration operations.",
				1));

			prologueSpeakInfos.Add(new PrologueSpeakInfo(
				"The creature has the ability to <color=yellow>transform and mimic human behavior</color>.", 2));

			prologueSpeakInfos.Add(new PrologueSpeakInfo(
				"To protect our town and its people, we have locked down the area until we can fully eliminate any potential threats.",
				3));

			prologueSpeakInfos.Add(new PrologueSpeakInfo(
				"It is essential that you stay informed by <color=yellow>listening to the radio and reading the newspaper</color> delivered to your doorstep. Stay safe, and stay vigilant.",
				4));
			
			return prologueSpeakInfos;

		}

		private void OnPrologueSpeakEnd(Speaker speaker) {
			onClickPeepholeSpeakEnd = true;
			DieCanvas.Singleton.Show("","", -1, true);
			this.GetSystem<ITimeSystem>().AddDelayTask(2.7f, () => {
				DieCanvas.Singleton.Hide();
			});
		}

		public override void OnEventEnd() {
			
		}
	}
}