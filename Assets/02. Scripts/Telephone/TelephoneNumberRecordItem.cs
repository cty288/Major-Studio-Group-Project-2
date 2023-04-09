using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TelephoneNumberRecordItem : MonoBehaviour {
   private TMP_Text nameText;
   private TMP_Text numberText;
   
   public void OnInit(string name, string number) {
      nameText = transform.Find("Name").GetComponent<TMP_Text>();
      numberText = transform.Find("Number").GetComponent<TMP_Text>();

      nameText.text = name + ":";
      numberText.text = number;
   }
}
