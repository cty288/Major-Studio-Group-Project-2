using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using UnityEngine;
using UnityEngine.UI;

public class RadioSwitch : AbstractMikroController<MainGame>
{
    [SerializeField] private AudioClip click;
    private Button button;
    private void Awake() {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        AudioSystem.Singleton.Play2DSound(click);
    }
}