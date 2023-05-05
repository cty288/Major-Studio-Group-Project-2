using System.Collections;
using System.Collections.Generic;
using MikroFramework.AudioKit;
using UnityEngine;
using UnityEngine.UI;

public class RadioVolumeSettingsElement : MenuSettingsElement
{
    protected Slider slider;

    private void Awake() {
        slider = GetComponentInChildren<Slider>();
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value) {
        PlayerPrefs.SetFloat("Radio_Volume", value);
    }

    public override void OnRefresh() {
        slider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Radio_Volume", 1f));
    }
}
