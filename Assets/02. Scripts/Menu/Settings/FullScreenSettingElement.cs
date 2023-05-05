using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FullScreenSettingElement : MenuSettingsElement {
	private Toggle offToggle;
	private Toggle onToggle;
	private ToggleGroup toggleGroup;

	private void Awake() {
		toggleGroup = GetComponent<ToggleGroup>();
		offToggle = transform.Find("SelectionElements/OffToggle").GetComponent<Toggle>();
		onToggle = transform.Find("SelectionElements/OnToggle").GetComponent<Toggle>();
		
		onToggle.onValueChanged.AddListener(OnToggleValueChanged);
		
	}

	private void OnToggleValueChanged(bool isOn) {
		Screen.fullScreen = isOn;
		PlayerPrefs.SetInt("FullScreen", isOn ? 1 : 0);
	}

	public override void OnRefresh() {
		bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
		onToggle.SetIsOnWithoutNotify(isFullScreen);
		offToggle.SetIsOnWithoutNotify(!isFullScreen);
	}
}
