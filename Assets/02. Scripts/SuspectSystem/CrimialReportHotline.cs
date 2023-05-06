using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.ArmyEnding;
using _02._Scripts.ArmyEnding.InitialPhoneCalls;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.Dog;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace _02._Scripts.SuspectSystem {
	public class CrimialReportHotline: TelephoneContact {
		private AudioMixerGroup mixer;
		private PlayerControlModel playerControlModel;
		private Coroutine waitingForInteractionCoroutine;
		protected string officerName;
		
		public CrimialReportHotline(): base() {
			speaker = GameObject.Find("CriminalHotlineSpeaker").GetComponent<Speaker>();
			mixer = speaker.GetComponent<AudioSource>().outputAudioMixerGroup;
			playerControlModel = this.GetModel<PlayerControlModel>();
		}
		public override bool OnDealt() {
			return true;
		}

		protected override void OnStart() {
			List<string> welcomes = new List<string>();
			List<string> officierNames = new List<string>() {
				"Smith", "Ted", "James", "John", "Jack", "Bob", "Robert", "William", "David", "Richard", "Charles",
				"Joseph", "Thomas", "Christopher", "Daniel",
			};
			officerName = officierNames[Random.Range(0, officierNames.Count)];
			welcomes.Add(
				$"Thank you for calling the Criminal Report Hotline. My name is {officerName}. To help us better identify the criminal, could you please describe them in detail?");
			welcomes.Add(
				$"Thank you for calling the Crime Snitch Hotline. This is {officerName}. To help us smoke out the bad guys, " +
				$"could you please describe the criminal and report it to us?");
			speaker.Speak(welcomes[Random.Range(0, welcomes.Count)], mixer, $"Officer {officerName}", 1f, OnWelcomeEnd);
		}

		private void OnWelcomeEnd(Speaker obj) {
			StartSelect();
		}

		private void StartSelect() {
			playerControlModel.AddControlType(PlayerControlType.BountyHunting);
			TopScreenHintText.Singleton.Show(
				"Please select any information related to the unknown creatures to report to the bounty hunter.\n\nPossible information includes: figure outside / death report photos");
			this.RegisterEvent<OnBodyHuntingSelect>(OnBodyHuntingSelect);
			waitingForInteractionCoroutine = CoroutineRunner.Singleton.StartCoroutine(WaitingInteraction());
		}
		
		private IEnumerator WaitingInteraction() {
			
			yield return new WaitForSeconds(30);
			if (playerControlModel.HasControlType(PlayerControlType.BountyHunting)) {
				string noInfo =
					"Seems like you don't have any information about the criminal. I will end this call now. You are free to call us again if you have any information.";
				speaker.Speak(noInfo, mixer, $"Officer {officerName}", 1f, (speaker) => {
					this.GetSystem<TelephoneSystem>().HangUp(false);
				});
			}
		}
		
		
		private void OnBodyHuntingSelect(OnBodyHuntingSelect e) {
     
	        this.UnRegisterEvent<OnBodyHuntingSelect>(OnBodyHuntingSelect);
	        if (waitingForInteractionCoroutine != null) {
	            CoroutineRunner.Singleton.StopCoroutine(waitingForInteractionCoroutine);
	            waitingForInteractionCoroutine = null;
	        }

	        this.GetSystem<ITimeSystem>().AddDelayTask(0.05f, () => {
		        playerControlModel.RemoveControlType(PlayerControlType.BountyHunting);
	        });
	       
	        TopScreenHintText.Singleton.Hide();


	        if (e.bodyInfos.Count > 1) {
		        speaker.Speak(
			        "The photo you sent have multiple people in it. Please send us a photo with only one person in it.",
			        mixer, $"Officer {officerName}", 1f, OnWelcomeEnd);
		        return;
	        }
	        
	        if(e.bodyInfos.Count == 0) {
		        speaker.Speak("We can't get much valuable from your description. We suggest you to send us a clear photo.", mixer,
			        $"Officer {officerName}", 1f, OnWelcomeEnd);
		        return;
	        }
	        
	        
	        //remove this body from the manager no matter what
	        //List<BodyInfo> bodyInfos = e.bodyInfos;

	        GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
	        BodyModel bodyModel = this.GetModel<BodyModel>();
	        SuspectModel suspectModel = this.GetModel<SuspectModel>();

	        BodyInfo bodyInfo = e.bodyInfos[0];
	        List<string> welcomes = new List<string>();
	        if (!suspectModel.IsSuspect(bodyInfo.ID)) {
		        
		        welcomes.Add(
			        "Thank you for your report. We have no record of the person you described, but we appreciate your help in keeping our community safe.");
		        welcomes.Add(
			        "We will investigate your report, but we currently have no record of the person you described. Thank you for bringing this to our attention.");
		        welcomes.Add(
			        "We will keep an eye out for the person you reported, but unfortunately they are not in our records. Thank you for contacting us.");
		        welcomes.Add(
			        "Sorry, we're not sure who that is. But hey, at least you're not making up imaginary friends, right? We'll still investigate, though.");
		        
		        speaker.Speak(welcomes[Random.Range(0, welcomes.Count)], mixer,
			        $"Officer {officerName}", 1f, OnEndingSpeak);
		        return;
	        }
	        
	        
	        
	        DateTime current = gameTimeModel.CurrentTime.Value;
	        current = current.AddDays(1);
	        DateTime tomorrow = new DateTime(current.Year, current.Month, current.Day , gameTimeModel.NightTimeStart, 0, 0);
	        
	        welcomes.Add(
		        "Thank you for providing us with more details about the suspect. This information will be very helpful in our investigation. " +
		        "You will receive your reward tomorrow. Thank you for your help in making our community safer.");

	        welcomes.Add(
		        "Your report has been very helpful. With this additional information, we will have a better chance of apprehending the suspect. " +
		        "You will receive your reward tomorrow. Thank you for your help in making our community safer.");

	        SuspectInfo suspectInfo = suspectModel.GetSuspectInfo(bodyInfo.ID);
	        bodyModel.KillBodyInfo(bodyInfo);



	        speaker.Speak(welcomes[Random.Range(0, welcomes.Count)], mixer,
		        $"Officer {officerName}", 1f, OnEndingSpeak);
	        this.GetSystem<GameEventSystem>().AddEvent(new GoodsRewardEvent(new TimeRange(tomorrow), new List<GoodsInfo>(){suspectInfo.rewards},
		        "Thanks for your making our community safer!",
		        "Note from Dorcha Police Department"));
	        ArmyEndingInitialCallCheck();
		}
		
		private void ArmyEndingInitialCallCheck() {
			ArmyEndingModel armyEndingModel = this.GetModel<ArmyEndingModel>();
			if (!armyEndingModel.TriggeredStartPhoneCall) {
				armyEndingModel.TriggeredStartPhoneCall = true;

				GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
				DateTime nextTime = this.GetModel<GameTimeModel>().GetDay(gameTimeModel.Day + Random.Range(3, 5));
				nextTime = nextTime.AddMinutes(Random.Range(20, 60));
				


				this.GetSystem<GameEventSystem>().AddEvent(new PoliceArmyEndingInitialPhoneCall(
					new TimeRange(nextTime, nextTime.AddMinutes(60)),
					new PoliceArmyEndingInitialPhoneCallContact(mixer, officerName), 5));
			}
		}
		private void OnEndingSpeak(Speaker speaker) {
			EndConversation();
		}
		protected override void OnHangUp(bool hangUpByPlayer) {
			End();
		}

		protected override void OnEnd() {
			End();
		}
		
		private void End() {
			playerControlModel.RemoveControlType(PlayerControlType.BountyHunting);
			TopScreenHintText.Singleton.Hide();
			this.UnRegisterEvent<OnBodyHuntingSelect>(OnBodyHuntingSelect);
			if (waitingForInteractionCoroutine != null) {
				CoroutineRunner.Singleton.StopCoroutine(waitingForInteractionCoroutine);
				waitingForInteractionCoroutine = null;
			}
		}
	}
}