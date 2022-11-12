using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Event;
using TMPro;
using UnityEngine;

public class TimeIndicator : MonoBehaviour {
    private TMP_Text text;

    private void Awake() {
        text = GetComponent<TMP_Text>();

        GameTimeManager.Singleton.CurrentTime.RegisterWithInitValue(OnTimeChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnTimeChanged(DateTime arg1, DateTime time) {
        text.text = $"{time.Hour:D2}:{time.Minute:D2}";
    }
}
