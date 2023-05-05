using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.BindableProperty;
using MikroFramework.Event;
using UnityEngine;

public class VoiceAudioControl : AbstractMikroController<MainGame> {
	public BindableProperty<float> GlobalVolume { get; private set; } = new BindableProperty<float>(1);
	private void Awake() {
		this.RegisterEvent<OnPauseAudioSet>(OnPauseAudioSet).UnRegisterWhenGameObjectDestroyed(gameObject);
	}

	private void Start() {
		this.Delay(0.2f, UpdateAudioVolume);
	}

	private void OnPauseAudioSet(OnPauseAudioSet e) {
		UpdateAudioVolume();
	}
	
	private void UpdateAudioVolume() {
		GlobalVolume.Value = Mathf.Max(0.01f, AudioSystem.Singleton.MasterVolume * PlayerPrefs.GetFloat("Voices_Volume", 1f));
	}
}
