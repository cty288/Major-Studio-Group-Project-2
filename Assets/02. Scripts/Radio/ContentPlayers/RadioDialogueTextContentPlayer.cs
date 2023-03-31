using System;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.ResKit;
using UnityEngine;

namespace _02._Scripts.Radio {
	public class RadioDialogueTextContentPlayer : RadioContentPlayer {
		protected Speaker speaker;
		protected GameObject spawnedSubtitle;
		protected RadioDialogueContent radioDialogueContent;
		protected int currentDialogueIndex = 0;
		private Action<RadioContentPlayer> onFinish;
		private bool isMuted = false;
		private bool isPlaying = false;
		private void Awake() {
			speaker = GetComponent<Speaker>();
			ResLoader resLoader = this.GetUtility<ResLoader>();
			Transform subtitleSpawnPos = GameObject.Find("RadioSubtitleSpawnPosition").transform;
			GameObject subtitlePrefab = resLoader.LoadSync<GameObject>("general", "RadioSubtitle");
			spawnedSubtitle = Instantiate(subtitlePrefab, subtitleSpawnPos);
			speaker.subtitile = spawnedSubtitle.GetComponent<Subtitle>();
		}

		public override void Mute(bool mute) {
			isMuted = mute;
			speaker.Mute(mute);
		}

		private void OnDestroy() {
			if (spawnedSubtitle) {
				Destroy(spawnedSubtitle);
			}
		}

		public override bool IsPlaying() {
			return isPlaying;
		}

		public override void Stop() {
			speaker.Stop();
			isPlaying = false;
		}

		public override void Play(IRadioContent content, Action<RadioContentPlayer> onFinish, bool isMuted) {
			radioDialogueContent = content as RadioDialogueContent;
			currentDialogueIndex = 0;
			this.onFinish = onFinish;
			this.isMuted = isMuted;
			StartSpeak(currentDialogueIndex);
			
		}

		private void StartSpeak(int i) {
			if(i>=radioDialogueContent.TextContents.Count) {
				onFinish?.Invoke(this);
				isPlaying = false;
				return;
			}
			isPlaying = true;
			this.Delay(0.1f, () => {
				RadioTextContent textContent = radioDialogueContent.TextContents[i];
				speaker.Speak(textContent.speakContent, textContent.mixer, textContent.DisplayName, 1, (speaker) => {
					StartSpeak(i + 1);
				}, textContent.speakRate, 1f, textContent.speakGender);
				
				Mute(isMuted);
			});
			
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