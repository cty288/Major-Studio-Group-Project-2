using System;
using DG.Tweening;
using MikroFramework;
using UnityEngine;

namespace _02._Scripts.Radio {
	public class RadioMusicContentPlayer: RadioContentPlayer {
		
		protected AudioSource audioSource;

		private void Awake() {
			audioSource = GetComponent<AudioSource>();
		}

		public override void Mute(bool mute) {
			audioSource.mute = mute;
		}

		public override bool IsPlaying() {
			return audioSource.isPlaying;
		}

		public override void Stop() {
			audioSource.DOFade(0, 0.5f).OnComplete(() => {
				audioSource.Stop();
			});
		}

		public override void Play(IRadioContent content, Action<RadioContentPlayer> onFinish, bool isMuted) {
			RadioMusicContent musicContent = content as RadioMusicContent;
			audioSource.clip = RadioContentPlayerFactory.Singleton.GetMusicSource(musicContent.MusicIndexInPlayList);
			audioSource.mute = isMuted;
			audioSource.Play();
			this.Delay(audioSource.clip.length, () => {
				onFinish(this);
			});
		}

		public override void SetVolume(float relativeVolume, bool isLoud, bool isInstant) {
			float loudVolume = 1f;
			float notLoudVolume = 0.5f;
			
			
			if (!isInstant) {
				if (!isLoud) {
					audioSource.DOFade(notLoudVolume * relativeVolume, 1f);
					//speaker.AudioMixer.DOSetFloat("volume", notLoudVolume, 1f);
				}
				else {
					audioSource.DOFade(loudVolume * relativeVolume, 1f);
				}
			}
			else {
				if (!isLoud) {
					audioSource.volume = notLoudVolume * relativeVolume;
				}
				else {
					audioSource.volume = loudVolume * relativeVolume;
				}
			}
		}
	}
}