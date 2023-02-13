﻿using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.AudioKit;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;


namespace _02._Scripts.AlienInfos.Tags.Base.KnockBehavior {
	public class NormalKnockBehavior: AbstractKnockBehavior {
		
		private Speaker currentSpeaker = null;
		AudioSource knockAudioSource = null;
		public NormalKnockBehavior(float knockDoorTimeInterval, float knockTime, List<string> overrideDoorKnockingPhrases) : base(knockDoorTimeInterval, knockTime, overrideDoorKnockingPhrases) {
		}

		public override string name { get; } = "Knock_random";
		public override IEnumerator OnKnockDoor(Speaker speaker, IVoiceTag voiceTag) {
			currentSpeaker = speaker;
			AudioMixerGroup mixer = AudioMixerList.Singleton.AlienVoiceGroups[voiceTag.VoiceIndex];
			for (int i = 0; i < KnockTime; i++) {
				string clipName = $"knock_{Random.Range(1, 8)}";
				knockAudioSource = AudioSystem.Singleton.Play2DSound(clipName, 1, false);
				yield return new WaitForSeconds(knockAudioSource.clip.length);

				bool speak = Random.Range(0, 100) <= 30;
				bool speakFinished = false;
				if(doorKnockingPhrases!=null && doorKnockingPhrases.Count>0 && speak) {
					speaker.Speak(doorKnockingPhrases[Random.Range(0, doorKnockingPhrases.Count)],
						mixer, "???", () => { speakFinished = true;}, voiceTag.VoiceSpeed, 1, voiceTag.VoiceType);
					
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
			currentSpeaker.Stop();
			knockAudioSource.Stop();
		}
	}
}