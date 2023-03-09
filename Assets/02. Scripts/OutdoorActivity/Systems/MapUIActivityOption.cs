using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapUIActivityOption : AbstractMikroController<MainGame> {
	private Transform panel;
	
	private TMP_Text title;
	private TMP_Text description;
	private IActivity activity;

	public IActivity Activity => activity;
	private Toggle toggle;

	public bool IsEnabled = false;
	private void Awake() {
		panel = transform.Find("Content");
		title = panel.Find("Title").GetComponent<TMP_Text>();
		description = panel.Find("Description").GetComponent<TMP_Text>();
		toggle = GetComponentInChildren<Toggle>(true);
	}



	public void Enable(IActivity activity) {
		this.activity = activity;
		panel.gameObject.SetActive(true);
		this.title.text = activity.DisplayName;
		this.description.text = activity.Description;
		toggle.isOn = false;
		IsEnabled = true;
	}

	public void Hide() {
		panel.gameObject.SetActive(false);
		toggle.isOn = false;
		IsEnabled = false;
		activity = null;
	}
}
