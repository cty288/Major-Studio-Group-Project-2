using System;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.ResKit;
using UnityEngine;

namespace _02._Scripts.Radio {
	public class RadioTextContentPlayer : RadioContentPlayer {
		protected Speaker speaker;
		protected GameObject spawnedSubtitle;
		private void Awake() {
			speaker = GetComponent<Speaker>();
			ResLoader resLoader = this.GetUtility<ResLoader>();
			Transform subtitleSpawnPos = GameObject.Find("RadioSubtitleSpawnPosition").transform;
			GameObject subtitlePrefab = resLoader.LoadSync<GameObject>("general", "RadioSubtitle");
			spawnedSubtitle = Instantiate(subtitlePrefab, subtitleSpawnPos);
			speaker.subtitile = spawnedSubtitle.GetComponent<Subtitle>();
		}

		public override void Mute(bool mute) {
			speaker.Mute(mute);
		}

		private void OnDestroy() {
			if (spawnedSubtitle) {
				Destroy(spawnedSubtitle);
			}
		}

		public override bool IsPlaying() {
			return speaker.IsSpeaking;
		}

		public override void Stop() {
			speaker.Stop(true);
		}

		public override void Play(IRadioContent content, Action<RadioContentPlayer> onFinish, bool isMuted) {
			RadioTextContent textContent = content as RadioTextContent;
			speaker.Speak(textContent.speakContent, textContent.mixer, textContent.DisplayName, 1, (speaker) => {
				onFinish?.Invoke(this);
			}, textContent.speakRate, 1f, textContent.speakGender);
			Mute(isMuted);
		}

		public override void SetVolume(float relativeVolume, bool isLoud, bool isInstant) {
			float loudVolume = -17 * (1 / relativeVolume);
			float notLoudVolume = -37 * (1 / relativeVolume);
			
			
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