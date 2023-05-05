using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.AudioKit;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour {
    protected Button button;
    [SerializeField] protected AudioClip clickSound;
    private void Awake() {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        AudioSystem.Singleton.Play2DSound(clickSound);
    }
}
