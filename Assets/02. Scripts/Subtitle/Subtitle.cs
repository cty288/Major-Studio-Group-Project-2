using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Subtitle : MonoBehaviour {
    private TMP_Text text;

    private void Awake() {
        text = GetComponent<TMP_Text>();
    }
    
    public void OnSpeakStart(string s) {
        text.text = s;
    }
}
