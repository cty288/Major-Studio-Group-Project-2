using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class RadioHintBG : MonoBehaviour
{
    public RadioHint radioHint;
    private void Awake()
    {
        radioHint = GameObject.Find("Hint").GetComponent<RadioHint>();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            radioHint.Hide();
            this.gameObject.SetActive(false);
        });
    }
}
