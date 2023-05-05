using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSettingsElement : MenuSettingsElement {
	private Toggle[] toggles;
	private ToggleGroup toggleGroup;
	[SerializeField]
	private Vector2Int[] resolutions;
	
	private void Awake() {
		toggleGroup = GetComponentInChildren<ToggleGroup>(true);
		toggles = transform.Find("SelectionElements").GetComponentsInChildren<Toggle>(true);
		
		//get the monitor's resolution
		Resolution currentResolution = Screen.currentResolution;
		resolutions[3] = new Vector2Int(currentResolution.width, currentResolution.height);

		foreach (Toggle toggle in toggles) {
			toggle.onValueChanged.AddListener(OnToggleValueChanged);
			
			TMP_Text text = toggle.transform.Find("Text").GetComponent<TMP_Text>();
			text.text = resolutions[toggle.transform.GetSiblingIndex()].x + " x " + resolutions[toggle.transform.GetSiblingIndex()].y;
		}
	}

	private void OnToggleValueChanged(bool value) {
		if (value) {
			Toggle activeToggle = toggleGroup.GetFirstActiveToggle();
			if (activeToggle != null) {
				int index = activeToggle.transform.GetSiblingIndex();
				Vector2Int resolution = resolutions[index];
				Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
			
				PlayerPrefs.SetInt("ResolutionIndex", index);
				PlayerPrefs.SetInt("ResolutionWidth", resolution.x);
				PlayerPrefs.SetInt("ResolutionHeight", resolution.y);
			}
		}
		
	}

	public override void OnRefresh() {
		int lastResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 3);
		toggles[lastResolutionIndex].SetIsOnWithoutNotify(true);
		
	}
}
