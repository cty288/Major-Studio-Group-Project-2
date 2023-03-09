using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndoorMapUIViewController : OpenableUIPanel {
	
	private List<Image> images;
	private List<TMP_Text> texts;
	private Dictionary<Image, float> imageAlpha = new Dictionary<Image, float>();
	protected Transform panel;
	
	
	protected OutdoorActivityModel outdoorActivityModel;
	protected OutdoorActivitySystem outdoorActivitySystem;
	private List<MapUIActivityOption> optionButtons = new List<MapUIActivityOption>();
	protected List<MapPlace> mapPlaceUIs;
	protected PlaceDescriptionPanel placeDescriptionPanel;
	
	protected IPlace lastSelectedPlace;
	protected override void Awake() {
		base.Awake();
		outdoorActivityModel = this.GetModel<OutdoorActivityModel>();
		panel = transform.Find("Panel");
		images = GetComponentsInChildren<Image>(true).ToList();
		texts = GetComponentsInChildren<TMP_Text>(true).ToList();
		mapPlaceUIs = GetComponentsInChildren<MapPlace>(true).ToList();
		placeDescriptionPanel = GetComponentInChildren<PlaceDescriptionPanel>(true);
		outdoorActivitySystem = this.GetSystem<OutdoorActivitySystem>();
		foreach (MapPlace mapPlace in mapPlaceUIs) {
			mapPlace.OnPlaceClicked += OnPlaceClicked;
		}
		
		foreach (var image in images) {
			imageAlpha.Add(image, image.color.a);
		}

		//Hide(0.5f);
		this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
		optionButtons = GetComponentsInChildren<MapUIActivityOption>(true).ToList();
	}

	private void OnDisable() {
		UnregisterLastPlace();
	}

	private void OnPlaceClicked(string placeName) {
		HideOptions();
		UnregisterLastPlace();
		IPlace place = outdoorActivitySystem.GetPlace(placeName);
		
		DescriptionData descriptionData = outdoorActivityModel.GetPlaceDescription(placeName);
		if(descriptionData!=null) {
			lastSelectedPlace = place;
			placeDescriptionPanel.ShowDescription(descriptionData.DisplayName, descriptionData.Description);
			List<IActivity> activities = outdoorActivitySystem.GetPlace(placeName).GetEnabledActivities();
			for (int i = 0; i < activities.Count; i++) {
				if (i >= optionButtons.Count) {
					break;
				}
				optionButtons[i].Enable(activities[i]);
			}

			place.RegisterOnActivityAvailableChanged(OnPlaceActivityAvailableChanged);
		}
	}

	private void UnregisterLastPlace() {
		if (lastSelectedPlace != null) {
			lastSelectedPlace.UnRegisterOnActivityAvailableChanged(OnPlaceActivityAvailableChanged);
			lastSelectedPlace = null;
		}
	}

	private void OnPlaceActivityAvailableChanged(IPlace place, IActivity activity, bool isAvailable) {
		if (isAvailable) {
			for (int i = 0; i < optionButtons.Count; i++) {
				MapUIActivityOption optionButton = optionButtons[i];
				if (!optionButton.IsEnabled) {
					optionButton.Enable(activity);
					break;
				}
			}
		}else {
			foreach (MapUIActivityOption mapUIActivityOption in optionButtons) {
				if (mapUIActivityOption.Activity.Name == activity.Name) {
					mapUIActivityOption.Hide();
					break;
				}
			}
		}
		
	}

	private void HideOptions() {
		foreach (MapUIActivityOption mapUIActivityOption in optionButtons) {
			mapUIActivityOption.Hide();
		}
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		foreach (MapPlace mapPlace in mapPlaceUIs) {
			mapPlace.OnPlaceClicked -= OnPlaceClicked;
		}
	}

	private void OnNewDay(OnNewDay e) {
		if (outdoorActivityModel.HasMap.Value) {
			Show(0.5f);
		}
	}


	public override void OnShow(float time) {
		panel.gameObject.SetActive(true);
		images.ForEach((image => image.DOFade(imageAlpha[image], time)));
		texts.ForEach((text => text.DOFade(1, time)));
	}

	public override void OnHide(float time) {
		images.ForEach((image => image.DOFade(0, time)));
		texts.ForEach((text => text.DOFade(0, time)));
		this.Delay(time, () => {
			if (this) { {
					panel.gameObject.SetActive(false);
				}
			}
		});
	}

	public override void OnDayEnd() {
		
	}
}
