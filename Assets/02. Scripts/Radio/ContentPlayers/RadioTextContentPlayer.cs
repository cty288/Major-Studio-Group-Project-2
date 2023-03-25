using System;
using DG.Tweening;
using UnityEngine;

namespace _02._Scripts.Radio {
	public class RadioTextContentPlayer : RadioContentPlayer {
		protected Speaker speaker;

		private void Awake() {
			speaker = GetComponent<Speaker>();
			speaker.subtitile = GameObject.Find("RadioSubtitle").GetComponent<Subtitle>();
		}

		public override void Mute(bool mute) {
			speaker.Mute(mute);
		}

		public override bool IsPlaying() {
			return speaker.IsSpeaking;
		}

		public override void Stop() {
			speaker.Stop();
		}

		public override void Play(IRadioContent content, Action<RadioContentPlayer> onFinish) {
			RadioTextContent textContent = content as RadioTextContent;
			speaker.Speak(textContent.speakContent, textContent.mixer, "Radio", 1, (speaker) => {
				onFinish?.Invoke(this);
			}, textContent.speakRate, 1f, textContent.speakGender);
		}

		public override void SetVolume(float relativeVolume, bool isLoud, bool isInstant) {
			float loudVolume = -20 * (1 / relativeVolume);
			float notLoudVolume = -45 * (1 / relativeVolume);
			
			
			if (!isInstant) {
				if (!isLoud) {
					speaker.AudioMixer.DOSetFloat("volume", notLoudVolume, 1f);
				}
				else {
					speaker.AudioMixer.DOSetFloat("volume", loudVolume, 1f);
				}
			}
			else {
				if (!isLoud) {
					speaker.AudioMixer.SetFloat("volume", notLoudVolume);
				}
				else {
					speaker.AudioMixer.SetFloat("volume", loudVolume);
				}
			}
		}
	}
}