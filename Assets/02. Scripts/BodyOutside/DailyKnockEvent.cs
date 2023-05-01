using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.ChoiceSystem;
using _02._Scripts.FPSEnding;
using _02._Scripts.GameTime;
using _02._Scripts.SurvivalGuide;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.TimeSystem;
using UnityEngine;
using Random = UnityEngine.Random;


namespace _02._Scripts.BodyOutside {
	public class DailyKnockEvent : BodyGenerationEvent {
		
		protected BodyGenerationSystem bodyGenerationSystem;
		protected DateTime knockWaitTimeUntil = DateTime.MaxValue;
		protected SurvivalGuideModel survivalGuideModel;
		public DailyKnockEvent(TimeRange startTimeRange) : base(startTimeRange, null, 1) {
			bodyGenerationSystem = this.GetSystem<BodyGenerationSystem>(system => {
				bodyGenerationSystem = system;
			});
			survivalGuideModel = this.GetModel<SurvivalGuideModel>();
		}

		public DailyKnockEvent() : base() {
			bodyGenerationSystem = this.GetSystem<BodyGenerationSystem>(system => {
				bodyGenerationSystem = system;
			});
			survivalGuideModel = this.GetModel<SurvivalGuideModel>();
		}

		public override void OnStart() {
			base.OnStart();
			bodyInfo = bodyGenerationSystem.GetAlienOrDeliverBody();
		}

		public override EventState OnUpdate() {
			DateTime currentTime = gameTimeManager.CurrentTime.Value;

			if (bodyInfo == null) {
				return EventState.End;
			}
			
			if (this.GetModel<GameStateModel>().IsDoorOpened && !interacted) {
				return EventState.Missed;
			}

			if (bodyInfo.IsAlien && this.GetModel<MonsterMotherModel>().MotherHealth.Value <= 0) {
				return EventState.End;
			}
			if (currentTime.Hour == gameTimeManager.NightTimeStart && currentTime.Minute <= 20) {
				
				return EventState.End;
			}
			
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
						BodyModel bodyModel = this.GetModel<BodyModel>();
						//bodyModel.ConsecutiveNonAlianSpawnCount = Mathf.Max(0, bodyModel.ConsecutiveNonAlianSpawnCount - 1);
						if (bodyInfo.IsAlien) {
							bodyModel.ConsecutiveNonAlianSpawnCount = 0;
						}
						else {
							bodyModel.ConsecutiveNonAlianSpawnCount++;
						}
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
				AudioSystem.Singleton.Play2DSound("door_open");
				this.GetSystem<ITimeSystem>().AddDelayTask(0.5f, () => {
					if (bodyInfo.IsAlien) {
						LoadCanvas.Singleton.ShowImage(0, 0f);
						List<string> messages = new List<string>() {
							"goOD dAy sIR. buT iT'S yOuR tiME!",
							"hI, Hi! iT IS yOur tiMe!",
							"I nEeD yOU cLotHEs!",
							"YOur bRaIN iS MiNE!",
							"YOuR TimE IS oVeR!"
						};
						speaker.Speak(messages[Random.Range(0, messages.Count)],
							AudioMixerList.Singleton.AudioMixerGroups[4], "???", 1f, OnAlienClickedOutside);
					}
					else {
						int foodCount = Random.Range(1, 3);
						this.GetModel<PlayerResourceModel>().AddFood(foodCount);
						LoadCanvas.Singleton.ShowImage(1, 0.2f);
						List<string> messages = new List<string>() {
							$"Delivery service! I got {foodCount} can{((foodCount > 1) ? "s" : "")} of food for you!",
							$"Here {(foodCount > 1 ? "are" : "is")} your food delivery! {foodCount} can{((foodCount > 1) ? "s" : "")} of food!",
							$"Hey, I brought you {foodCount} can{((foodCount > 1) ? "s" : "")} of food! Take care!",
						};

						IVoiceTag voiceTag = bodyInfo.VoiceTag;

						string additionalMessage = "";
						//additionalMessage += SendSurvivalGuideAndGetPhrases();

						speaker.Speak(messages[Random.Range(0, messages.Count)] + additionalMessage,
							bodyInfo.VoiceTag.VoiceGroup,
							"Deliver", 1, OnDelivererClickedOutside,
							voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
					}
				});
				return () => onClickPeepholeSpeakEnd;
			}

		private string SendSurvivalGuideAndGetPhrases() {
			if (survivalGuideModel.ReceivedSurvivalGuideBefore.Value) {
				return "";
			}
			survivalGuideModel.ReceivedSurvivalGuideBefore.Value = true;
			return
				" By the way, we are also sending out survival guide. It is written by the Dorcha government recently, and it could be very beneficial for you in this world filled with danger. Please take it and read it carefully. It's essential to know how to survive in this world full of monsters.";
		}


		private void OnDelivererClickedOutside(Speaker speaker) {
	       
	       // this.SendEvent<OnShowFood>();
	        timeSystem.AddDelayTask(1f, () => {
	            onClickPeepholeSpeakEnd = true;
	            LoadCanvas.Singleton.HideImage(0.5f);
	        });
	    }

	    protected void OnAlienClickedOutside(Speaker speaker) {
		    DogModel dogModel = this.GetModel<DogModel>();
		   


		    bool hasBulletAndGun = playerResourceModel.HasEnoughResource<BulletGoods>(1) &&
		                           playerResourceModel.HasEnoughResource<GunResource>(1);

		    bool hasDog = dogModel.HaveDog && dogModel.isDogAlive && !dogModel.SentDogBack;
	        if (hasDog && !hasBulletAndGun) {
	           SacrificeDog();
	           
	        } else if (hasBulletAndGun && !hasDog) {
	           GunKill();
	        }else if (hasBulletAndGun && hasDog) {
		        SelectGunOrDog();
	        }else {
	            LoadCanvas.Singleton.HideImage(1f);
	            DieCanvas.Singleton.Show("You are killed by the monster!", 2);
	            this.GetModel<GameStateModel>().GameState.Value = GameState.End;
	            this.GetSystem<BodyGenerationSystem>().StopCurrentBody();
	        }
	    }

	    private void SelectGunOrDog() {
		    ChoiceSystem.ChoiceSystem choiceSystem = this.GetSystem<ChoiceSystem.ChoiceSystem>();
		    choiceSystem.StartChoiceGroup(new ChoiceGroup(ChoiceType.Outside,
			    new ChoiceOption("- Use the gun", OnSelectUseGun),
			    new ChoiceOption("- Call the dog for help", OnSelectUseDog)));
	    }

	    private void OnSelectUseDog(ChoiceOption obj) {
		    SacrificeDog();
	    }

	    private void OnSelectUseGun(ChoiceOption obj) {
		    GunKill();
	    }

	    protected virtual void SacrificeDog() {
		    DogSystem dogSystem = this.GetSystem<DogSystem>();
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
	    }

	    protected void GunKill() {
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
	    }

	    public override void OnMissed() {
		    GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
		    DateTime nextTime = gameTimeModel.CurrentTime.Value.AddMinutes(Random.Range(10, 20));
		    gameEventSystem.AddEvent(new DailyKnockEvent(new TimeRange(nextTime, nextTime.AddMinutes(20))));
	    }
	    
	    public override void OnEventEnd() {
		    //bodyGenerationSystem.SpawnAlienOrDeliverBody();
		    GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
		    DateTime nextTime = gameTimeModel.CurrentTime.Value.AddMinutes(Random.Range(20, 50));
		    gameEventSystem.AddEvent(new DailyKnockEvent(new TimeRange(nextTime, nextTime.AddMinutes(20))));
	    }
	}
}