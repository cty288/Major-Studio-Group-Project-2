﻿using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using UnityEngine;
using UnityEngine.Audio;

namespace _02._Scripts.Poster.PosterEvents {
	public class PosterKnockBehavior: AbstractKnockBehavior, ICanSendEvent {
		[field: ES3Serializable]
		public override string TagName { get; protected set; } = "Knock_Poster";
		
		public PosterKnockBehavior(): base(){}
		
		private Speaker currentSpeaker = null;
		AudioSource knockAudioSource = null;
		
		public PosterKnockBehavior(float knockDoorTimeInterval, float knockTime, List<string> overrideDoorKnockingPhrases): base(knockDoorTimeInterval, knockTime, overrideDoorKnockingPhrases){}
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
					speaker.Speak(doorKnockingPhrases[Random.Range(0, doorKnockingPhrases.Count)],
						mixer, "???", 1,(speaker) => { speakFinished = true;}, voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
					
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

		public override void OnHitByFlashlight(Speaker speaker, IVoiceTag voiceTag, bool isAlien) {
			if (!isAlien) {
				bool speak = Random.Range(0, 100) <= 40;
				if (speak) {
					List<string> replies = new List<string>();
					replies.Add("Hey! I'm not the monster! I send you poster!");
					replies.Add("Cut it off! I'm not the monster!");
					string reply = replies[UnityEngine.Random.Range(0, replies.Count)];

                    
					if (voiceTag != null) {
						speaker.Speak(reply, voiceTag.VoiceGroup,
							"",1f, null,voiceTag.VoiceSpeed,1f,
							voiceTag.VoiceType);
					}
					else {
						speaker.Speak(reply, null, "",1f, null,Random.Range(0.8f, 1.2f));
					}
				}
			}
		}
	}
	
	
}