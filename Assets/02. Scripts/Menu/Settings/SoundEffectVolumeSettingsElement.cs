using System.Collections;
using System.Collections.Generic;
using MikroFramework.AudioKit;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectVolumeSettingsElement : MenuSettingsElement
{
    protected Slider slider;

    private void Awake() {
        slider = GetComponentInChildren<Slider>();
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value) {
        AudioSystem.Singleton.SoundVolume = value;
    }

    public override void OnRefresh() {
        slider.SetValueWithoutNotify(AudioSystem.Singleton.SoundVolume);
    }
}
