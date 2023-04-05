using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RadioHintBtn : MonoBehaviour
{
    public RadioHint radioHint;
    public GameObject hintBG;

    public void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            hintBG.SetActive(true);
            Image[] imgs = hintBG.GetComponentsInChildren<Image>(true);
            TMP_Text[] texts = hintBG.GetComponentsInChildren<TMP_Text>(true);


            foreach (Image img in imgs) {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
                img.DOFade(0.7f, 0.5f);
            }

            foreach (TMP_Text text in texts) {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
                text.DOFade(1, 0.5f);
            }
            radioHint.Show();
        });
    }

}