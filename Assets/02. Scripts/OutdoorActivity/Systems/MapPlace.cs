using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapPlace : AbstractMikroController<MainGame> {
    [SerializeField] protected string placeName;
    protected OutdoorActivitySystem outdoorActivitySystem;
    protected Button button;
    protected GameObject newHintObject;
    protected TMP_Text placeNameText;
    
    public Action<string> OnPlaceClicked;

    private IPlace place;
    private void Awake() {
        outdoorActivitySystem = this.GetSystem<OutdoorActivitySystem>();
        button = transform.Find("Button").GetComponent<Button>();
        newHintObject = button.transform.Find("NewText").gameObject;
        placeNameText = button.transform.Find("PlaceName").GetComponent<TMP_Text>();
        place = outdoorActivitySystem.GetPlace(placeName);
        placeNameText.text = place.DisplayName;
        place.InteractedBefore.RegisterWithInitValue(OnInteractedBeforeChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
        place.RegisterOnPlaceAvailableChanged(OnPlaceAvailabilityChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
        button.onClick.AddListener(OnButtonClicked);
        button.gameObject.SetActive(place.IsAvailable);
    }

    private void OnPlaceAvailabilityChanged(IPlace place, bool available) {
        button.gameObject.SetActive(available);
    }

    private void OnButtonClicked() {
        place.InteractedBefore.Value = true;
        OnPlaceClicked?.Invoke(placeName);
    }

    private void OnInteractedBeforeChanged(bool interactedBefore) {
        newHintObject.SetActive(!interactedBefore);
    }
    
}
