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
					speaker.Speak(messages[Random.Range(0, messages.Count)], AudioMixerList.Singleton.AudioMixerGroups[4], "???", OnAlienClickedOutside);
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
						AudioMixerList.Singleton.AlienVoiceGroups[bodyInfo.VoiceTag.VoiceIndex],
						"Deliver", OnDelivererClickedOutside,
						voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
				}
				return () => onClickPeepholeSpeakEnd;
			}
			
			
			
		private void OnDelivererClickedOutside() {
	        this.GetModel<PlayerResourceModel>().AddFood(Random.Range(1, 3));
	       // this.SendEvent<OnShowFood>();
	        timeSystem.AddDelayTask(1f, () => {
	            onClickPeepholeSpeakEnd = true;
	            LoadCanvas.Singleton.HideImage(0.5f);
	        });
	    }

	    protected void OnAlienClickedOutside() {
	        DogSystem dogSystem = this.GetSystem<DogSystem>();
	        if (dogSystem.HaveDog && dogSystem.isDogAlive) {
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