using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.ChoiceSystem;
using _02._Scripts.FPSEnding;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;


namespace _02._Scripts.CultEnding {
	public class CultKnockEvent : BodyGenerationEvent {
		[field: ES3Serializable] private int refuseTime = 0;

		protected MonsterMotherModel _monsterMotherModel;
		public CultKnockEvent(TimeRange startTimeRange, BodyInfo bodyInfo, int refuseTime): base(startTimeRange, bodyInfo, 1) {
			this.refuseTime = refuseTime;
			Debug.Log("CultKnockEvent start time: " + startTimeRange.StartTime);
			_monsterMotherModel = this.GetModel<MonsterMotherModel>();
		}

		public CultKnockEvent(): base() {
			_monsterMotherModel = this.GetModel<MonsterMotherModel>();
		}

		public override void OnMissed() {
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime evTime = gameTimeModel.CurrentTime.Value.AddMinutes(Random.Range(20, 50));
			gameEventSystem.AddEvent(new CultKnockEvent(new TimeRange(evTime, evTime.AddMinutes(20)),
				CultEndingSystem.GetCultBodyInfo(), refuseTime));
		}

		protected override Func<bool> OnOpen() {
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			string message = "";

			if (refuseTime == 0) {
				message =
					"Have you heard the call of the Xenovore, my friend? It is the only true path to salvation, to transcendence. " +
					"Join us, the <color=yellow>Shadow Sect</color>, and embrace the transformation.";
			}
			else {
				message =
					"Hello once again, surely you must see by now, there is nothing to be gained from this meaningly struggle to subsist." +
					"We are so close to reaching the <color=yellow>Xenovore</color>, and in her embrace we will find rebirth.";
			}
			
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			speaker.Speak(message,
				bodyInfo.VoiceTag.VoiceGroup,
				"Cultist", 1, OnGreetingEnd,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
			return () => onClickPeepholeSpeakEnd;
		}

		protected override void OnNotOpen() {
			base.OnNotOpen();
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime evTime = gameTimeModel.GetDay(gameTimeModel.Day + Random.Range(1, 4))
				.AddMinutes(Random.Range(20, 100));
			gameEventSystem.AddEvent(new CultKnockEvent(new TimeRange(evTime, evTime.AddMinutes(20)),
				CultEndingSystem.GetCultBodyInfo(), refuseTime));
		}

		private void OnGreetingEnd(Speaker obj) {
			this.GetSystem<ChoiceSystem.ChoiceSystem>().StartChoiceGroup(new ChoiceGroup(ChoiceType.Outside,
				new ChoiceOption("- \"Why should I join you?\"", OnRespond),
				new ChoiceOption("- [Join the Shadow Sect] ", OnJoin), new  ChoiceOption("- [Refuse]", OnRefuse)));
		}

		private void OnRespond(ChoiceOption option) {
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			string message =
				"The benefits of joining our sect are immeasurable, my friend. You will have taken the first step towards true liberation. " +
				"As Shadow Sect members, <color=yellow>we will each seek out the Xenovore and share any information we uncover about our leader</color>. " +
				"Join us, and together we will rise above the tyranny of the government and the mundane limitations of humanity!";
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			speaker.Speak(message,
				bodyInfo.VoiceTag.VoiceGroup,
				"Cultist", 1, OnPlayerMakeChoice,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
			
		}

		private void OnPlayerMakeChoice(Speaker obj) {
			this.GetSystem<ChoiceSystem.ChoiceSystem>().StartChoiceGroup(new ChoiceGroup(ChoiceType.Outside,
				new ChoiceOption("- [Join the Shadow Sect] ", OnJoin), new ChoiceOption("- [Refuse]", OnRefuse)));
		}

		private void OnRefuse(ChoiceOption obj) {
			refuseTime++;
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			string message =
				"Well...That is disappointing. You have been brainwashed by the government. Perhaps you will see the truth next time.";

			if (refuseTime >= 3) {
				message = "If you're not willing to see the truth, then we cannot help you. " +
				          "We'll leave you to your blindness and continue our search for the one true Xenovore. Farewell, lost soul.";
			}
			
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			speaker.Speak(message,
				bodyInfo.VoiceTag.VoiceGroup,
				"Cultist", 1, OnEndConversation,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);

			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime evTime = gameTimeModel.GetDay(gameTimeModel.Day + Random.Range(2, 4))
				.AddMinutes(Random.Range(20, 100));
			if (refuseTime >= 3) {
				evTime = gameTimeModel.GetDay(gameTimeModel.Day + 1);
				SpawnFirstMonsterMother();
				ConstructCultdeadNews(evTime, gameTimeModel.Day + 1);
				return;
			}
			
			gameEventSystem.AddEvent(new CultKnockEvent(new TimeRange(evTime, evTime.AddMinutes(20)),
				CultEndingSystem.GetCultBodyInfo(), refuseTime));
		}

		private void ConstructCultdeadNews(DateTime evTime, int day) {
			ImportantNewsTextInfo content = this.GetModel<ImportantNewsTextModel>().GetInfo("CultDieNews");
			List<string> listOfInfo = this.GetModel<CultEndingModel>().PopContents(3);
			string info = "\n";
			for (int i = 1; i <= listOfInfo.Count; i++) {
				info += i + ". " + "<color=red>" + listOfInfo[i - 1] + "</color>";
				if (i != listOfInfo.Count) {
					info += "\n\n";
				}
			}

			content.Content += info;
			ImportantNewspaperModel model = this.GetModel<ImportantNewspaperModel>();
			int issue = model.GetIssueForNews(day, evTime);
			model.AddPageToNewspaper(issue, content, 0);
			Debug.Log("Cult dead news will be published on day " + day + " issue " + issue);
		}

		private void OnJoin(ChoiceOption obj) {
			Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
			string message = "You've made the wise decision. Welcome to the Shadow Sect, brother! " +
			                 "From this moment on, you're one of us. <color=yellow>We'll keep you updated on any information we discover about Xenovore</color>," +
			                 " and together we'll work towards our ultimate goal.";
			
			IVoiceTag voiceTag = bodyInfo.VoiceTag;
			speaker.Speak(message,
				bodyInfo.VoiceTag.VoiceGroup,
				"Cultist", 1, OnEndConversation,
				voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
			
			
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			
			
			SpawnFirstMonsterMother();
			
			DateTime nextTime = gameTimeModel.GetDay(gameTimeModel.Day + Random.Range(2, 4));
			gameEventSystem.AddEvent(new CultistLetterSpawnEvent(new TimeRange(nextTime)));

			DateTime newsTime = nextTime.AddDays(2);
			ImportantNewspaperModel importantNewspaperModel = this.GetModel<ImportantNewspaperModel>();
			int issue = importantNewspaperModel.GetIssueForNews(gameTimeModel.GetDay(newsTime), newsTime);
			importantNewspaperModel.AddPageToNewspaper(issue, 
				this.GetModel<ImportantNewsTextModel>().GetInfo("PoliceSearchMother"),0); 
			Debug.Log("Police search mother issue: " + issue);
		}

		private void SpawnFirstMonsterMother() {
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime nextStartTime = gameTimeModel.GetDay(gameTimeModel.Day + Random.Range(3, 5));
			
			
			nextStartTime = new DateTime(nextStartTime.Year, nextStartTime.Month, nextStartTime.Day, 23,
				Random.Range(0, 39), 0);
			if (!_monsterMotherModel.MonsterMotherSpawned) {
				MonsterMotherModel monsterMotherModel = this.GetModel<MonsterMotherModel>();
				BodyModel bodyModel = this.GetModel<BodyModel>();
				if (!bodyModel.IsInAllBodyTimeInfos(monsterMotherModel.MotherBodyTimeInfo.BodyInfo)) {
					bodyModel.AddToAllBodyTimeInfos(monsterMotherModel.MotherBodyTimeInfo);
					_monsterMotherModel.MonsterMotherSpawned = true;
					this.GetSystem<GameEventSystem>().AddEvent(new MonsterMotherSpawnEvent(
						new TimeRange(nextStartTime, nextStartTime.AddMinutes(20)),
						_monsterMotherModel.MotherBodyTimeInfo.BodyInfo, 1));
				}
				
			}
		}

		private void OnEndConversation(Speaker obj) {
			onClickPeepholeSpeakEnd = true;
		}

		public override void OnEventEnd() {
			
		}
	}
}