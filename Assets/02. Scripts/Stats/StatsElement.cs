using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StatsElement : MonoBehaviour {
   protected TMP_Text nameText;
   protected TMP_Text valueText;

   private void Awake() {
      nameText = transform.Find("Name").GetComponent<TMP_Text>();
      valueText = transform.Find("Value").GetComponent<TMP_Text>();
   }
   
   public void SetData(string name, object value) {
      Awake();
      nameText.text = name;
      valueText.text = value.ToString();
   }
   
   public void Show(float delay) {
      nameText.DOFade(1, delay);
      valueText.DOFade(1, delay);
   }
   
   public void Stop(float delay) {
      nameText.DOKill();
      valueText.DOKill();
      nameText.DOFade(0, delay);
      valueText.DOFade(0, delay).OnComplete(() => {
         Destroy(gameObject);
      });
   }
   
   
}
