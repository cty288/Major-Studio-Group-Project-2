using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;

public class PlaceDescriptionPanel : AbstractMikroController<MainGame> {
    private TMP_Text descriptionText;

    private void Awake() {
        descriptionText = GetComponentInChildren<TMP_Text>(true);
        ShowDescription("", "");
    }
    
    
    public void ShowDescription(string placeName, string description) {
        if (!String.IsNullOrEmpty(placeName) && !String.IsNullOrEmpty(description)) {
            descriptionText.text = $"Travel To\n{placeName}\n\n\"{description}\"";
        }
    }
}
