using System.Collections;
using System.Collections.Generic;
using MikroFramework.AudioKit;
using UnityEngine;

namespace _02._Scripts.AlienInfos.Tags.Base.KnockBehavior {
	public class DogKnockBehavior: AbstractKnockBehavior {
		private AudioSource knockAudioSource = null;
		public DogKnockBehavior(float knockDoorTimeInterval, float knockTime, List<string> overrideDoorKnockingPhrases) : base(knockDoorTimeInterval, knockTime, overrideDoorKnockingPhrases) {
		}

		public DogKnockBehavior():base(){}
		[field: ES3Serializable]
		public override string TagName { get; protected set; }= "Knock_dog";

		public override IEnumerator OnKnockDoor(Speaker speaker, IVoiceTag voiceTag, bool isAlien) {
			
			for (int i = 0; i < KnockTime; i++) {
				string clipName =  $"dog_outside_door_{Random.Range(1, 4)}";
				knockAudioSource = AudioSystem.Singleton.Play2DSound(clipName, 1, false);
				yield return new WaitForSeconds(knockAudioSource.clip.length + KnockDoorTimeInterval);
			}
			
		}

		public override IKnockBehavior GetOpposite() {
			return this;
		}

		public override void OnStopKnock() {
			AudioSystem.Singleton.StopSound(knockAudioSource);
		}

		public override void OnHitByFlashlight(Speaker speaker, IVoiceTag voiceTag, bool isAlien) {
			
		}
	}
}