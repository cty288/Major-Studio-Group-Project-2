using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RadioHintBtn : MonoBehaviour
{
    public RadioHint radioHint;
   

    public void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            if(radioHint.IsShowing) {
                radioHint.Hide();
            } else {
                radioHint.Show();
            }
        });
    }

}