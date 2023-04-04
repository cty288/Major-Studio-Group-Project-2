using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using UnityEngine;
using Random = UnityEngine.Random;


namespace _02._Scripts.BodyOutside {
	public class DailyKnockEvent : BodyGenerationEvent {
		
		protected BodyGenerationSystem bodyGenerationSystem;
		protected DateTime knockWaitTimeUntil = DateTime.MaxValue;
		public DailyKnockEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float eventTriggerChance) : base(startTimeRange, bodyInfo, eventTriggerChance) {
			bodyGenerationSystem = this.GetSystem<BodyGenerationSystem>(system => {
				bodyGenerationSystem = system;
			});
		}

		public DailyKnockEvent() : base() {
			bodyGenerationSystem = this.GetSystem<BodyGenerationSystem>(system => {
				bodyGenerationSystem = system;
			});
		}
		public override EventState OnUpdate() {
			DateTime currentTime = gameTimeManager.CurrentTime.Value;
        
			if ((currentTime.Hour == 23 && currentTime.Minute >= 58 && !interacted) || gameStateModel.GameState.Value == GameState.End) {
				if (knockDoorCheckCoroutine != null) {
					CoroutineRunner.Singleton.StopCoroutine(knockDoorCheckCoroutine);
					knockDoorCheckCoroutine = null;
				}
            
				if (nestedKnockDoorCheckCoroutine != null) {
					CoroutineRunner.Singleton.StopCoroutine(nestedKnockDoorCheckCoroutine);
					nestedKnockDoorCheckCoroutine = null;
				}
				bodyGenerationModel.CurrentOutsideBody.Value = null;
				OnNotOpen();
				return EventState.End;
			}

			if (!started) {
				if (knockWaitTimeUntil == DateTime.MaxValue) {
					knockWaitTimeUntil = currentTime.AddMinutes(5);
				}
				else {
					if (bodyGenerationModel.CurrentOutsideBody.Value != null) {
						OnNotOpen();
						return EventState.End;
					}
					if (currentTime >= knockWaitTimeUntil) {
						started = true;
						knockDoorCheckCoroutine = CoroutineRunner.Singleton.StartCoroutine(KnockDoorCheck());
					}
				}
				
			}

			return (bodyGenerationModel.CurrentOutsideBody.Value == null && !bodyGenerationModel.CurrentOutsideBodyConversationFinishing
				&& currentTime >= knockWaitTimeUntil) ? EventState.End : EventState.Running;
		}
		protected override Func<bool> OnOpen() {
				onClickPeepholeSpeakEnd = false;
				Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
				if (bodyInfo.IsAlien) {
					LoadCanvas.Singleton.ShowImage(0, 0.2f);
					List<string> messages = new List<string>() {
						"goOD dAy sIR. buT iT'S yOuR tiME!",
						"hI, Hi! iT IS yOur tiMe!",
						"I nEeD yOU cLotHEs!",
						"YOur bRaIN iS MiNE!",
						"YOuR TimE IS oVeR!"
					};
					speaker.Speak(messages[Random.Range(0, messages.Count)], AudioMixerList.Singleton.AudioMixerGroups[4], "???", 1f,OnAlienClickedOutside);
				}
				else {
	            
					LoadCanvas.Singleton.ShowImage(1, 0.2f);
					List<string> messages = new List<string>() {
						"Delivery service! Take care!",
						"Here's the food for you today. Take care!",
						"Hey, I brought you some foods! Take care!"
					};

					IVoiceTag voiceTag = bodyInfo.VoiceTag;

					speaker.Speak(messages[Random.Range(0, messages.Count)],
						bodyInfo.VoiceTag.VoiceGroup,
						"Deliver", 1, OnDelivererClickedOutside,
						voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
				}
				return () => onClickPeepholeSpeakEnd;
			}
			
			
			
		private void OnDelivererClickedOutside(Speaker speaker) {
	        this.GetModel<PlayerResourceModel>().AddFood(Random.Range(1, 3));
	       // this.SendEvent<OnShowFood>();
	        timeSystem.AddDelayTask(1f, () => {
	            onClickPeepholeSpeakEnd = true;
	            LoadCanvas.Singleton.HideImage(0.5f);
	        });
	    }

	    protected void OnAlienClickedOutside(Speaker speaker) {
		    DogModel dogModel = this.GetModel<DogModel>();
		    DogSystem dogSystem = this.GetSystem<DogSystem>();
		    
	        if (dogModel.HaveDog && dogModel.isDogAlive && !dogModel.SentDogBack) {
	            float clipLength = AudioSystem.Singleton.Play2DSound("dogBark_4").clip.length;
	            timeSystem.AddDelayTask(clipLength, () => {
	                LoadCanvas.Singleton.HideImage(1f);
	                AudioSystem.Singleton.Play2DSound("dog_die");
	                LoadCanvas.Singleton.ShowMessage("Your friend is gone...");
	                dogSystem.KillDog();
	                timeSystem.AddDelayTask(2f, () => {
	                    LoadCanvas.Singleton.HideMessage();
	                    timeSystem.AddDelayTask(1f, () => {
	                        onClickPeepholeSpeakEnd = true;
	                    });                    
	                });
	            });
	        } else if (playerResourceModel.HasEnoughResource<BulletGoods>(1) && playerResourceModel.HasEnoughResource<GunResource>(1)) {
	            playerResourceModel.RemoveResource<BulletGoods>(1);
	            float clipLength = AudioSystem.Singleton.Play2DSound("gun_fire").clip.length;

	            timeSystem.AddDelayTask(1f, () => {
	                LoadCanvas.Singleton.HideImage(1f);
	                LoadCanvas.Singleton.ShowMessage("You shot the creature and it fleed.\n\nBullet - 1");
	                timeSystem.AddDelayTask(2f, () => {
	                    LoadCanvas.Singleton.HideMessage();
	                    timeSystem.AddDelayTask(1f, () => {
	                        onClickPeepholeSpeakEnd = true;
	                    });
	                });
	            });
	        }else {
	            LoadCanvas.Singleton.HideImage(1f);
	            DieCanvas.Singleton.Show("You are killed by the creature!");
	            this.GetModel<GameStateModel>().GameState.Value = GameState.End;
	            this.GetSystem<BodyGenerationSystem>().StopCurrentBody();
	        } 
	    }

	    public override void OnMissed() {
		    bodyGenerationSystem.SpawnAlienOrDeliverBody();
	    }
	    
	    public override void OnEventEnd() {
		    bodyGenerationSystem.SpawnAlienOrDeliverBody();
	    }
	}
}