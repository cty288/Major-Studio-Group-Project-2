using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Singletons;
using TMPro;
using UnityEngine;

public class TopScreenHintText : MonoMikroSingleton<TopScreenHintText> {
    private TMP_Text text;

    private void Awake() {
        text = GetComponent<TMP_Text>();
        Hide();
    }



    public void Show(string text) {
        this.text.text = text;
      
    }

    public void Hide() {
        this.text.text = "";
    }
}
