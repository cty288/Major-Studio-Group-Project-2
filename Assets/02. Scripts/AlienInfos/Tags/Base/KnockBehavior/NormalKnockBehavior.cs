﻿using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;


namespace _02._Scripts.AlienInfos.Tags.Base.KnockBehavior {
	public struct OnKnockOutsideAudioPlayed {
		public bool isAlien;
	}
	public class NormalKnockBehavior: AbstractKnockBehavior, ICanSendEvent {
		
		private Speaker currentSpeaker = null;
		AudioSource knockAudioSource = null;
		public NormalKnockBehavior(float knockDoorTimeInterval, float knockTime, List<string> overrideDoorKnockingPhrases, string overrideTagName = "") : base(knockDoorTimeInterval, knockTime, overrideDoorKnockingPhrases, overrideTagName) {
		}
		
		public NormalKnockBehavior(): base(){}

		[field: ES3Serializable]
		public override string TagName { get; protected set; }= "Knock_random";
		public override IEnumerator OnKnockDoor(Speaker speaker, IVoiceTag voiceTag, bool isAlien) {
			currentSpeaker = speaker;
			AudioMixerGroup mixer = voiceTag.VoiceGroup;
			for (int i = 0; i < KnockTime; i++) {
				string clipName = $"knock_{Random.Range(1, 8)}";
				knockAudioSource = AudioSystem.Singleton.Play2DSound(clipName, 1, false);
				
				yield return new WaitForSeconds(knockAudioSource.clip.length);
				
				this.SendEvent<OnKnockOutsideAudioPlayed>(new OnKnockOutsideAudioPlayed() {
					isAlien = isAlien
				});

				bool speak = Random.Range(0, 100) <= 30;
				bool speakFinished = false;
				if(doorKnockingPhrases!=null && doorKnockingPhrases.Count>0 && speak) {
					this.SendEvent<OnOutsideBodyStartSpeak>();
					speaker.Speak(doorKnockingPhrases[Random.Range(0, doorKnockingPhrases.Count)],
						mixer, "???", 1, (speaker) => {
							this.SendEvent<OnOutsideBodyStopSpeak>();
							speakFinished = true;
						}, voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
					
					while (!speakFinished) {
						yield return null;
					}
				}
				
				yield return new WaitForSeconds(KnockDoorTimeInterval);
			}

		}

		public override IKnockBehavior GetOpposite() {
			return new NormalKnockBehavior(KnockDoorTimeInterval, KnockTime, null);
		}

		public override void OnStopKnock() {
			currentSpeaker.Stop(true);
			knockAudioSource.Stop();
			
		}
	}

	public struct OnOutsideBodyStartSpeak {
		
	}
	
	public struct OnOutsideBodyStopSpeak {
		
	}
}