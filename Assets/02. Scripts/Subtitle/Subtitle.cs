using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class Subtitle : MonoBehaviour {
    private TMP_Text text;
    

    private void Awake() {
        text = GetComponent<TMP_Text>();
    }
    
    public void OnSpeakStart(string s, string speakName) {
        text.text = $"{speakName}: {s}";
        if (String.IsNullOrEmpty(speakName)) {
            text.text = s;
        }
    }
    
    private void Update()
    {
      
    }

  
}
