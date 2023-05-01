using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using MikroFramework.AudioKit;

public class RadioHintBtn : MonoBehaviour
{
    public RadioHint radioHint;
    //[SerializeField] private AudioClip ui_click;
    public void Awake() {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            AudioSystem.Singleton.Play2DSound("UI_Click");
            if(radioHint.IsShowing) {
                radioHint.Hide();
            } else {
                radioHint.Show();
            }
        });
    }

}