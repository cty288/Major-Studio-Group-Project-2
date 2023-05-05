using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceVolumeSettingsElement : MenuSettingsElement
{
    protected Slider slider;

    private void Awake() {
        slider = GetComponentInChildren<Slider>();
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value) {
        PlayerPrefs.SetFloat("Voices_Volume", value);
    }

    public override void OnRefresh() {
        slider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Voices_Volume", 1f));
    }
}
