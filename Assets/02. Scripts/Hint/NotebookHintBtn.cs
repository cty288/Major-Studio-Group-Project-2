using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class NotebookHintBtn : MonoBehaviour
{
    public NotebookHint notebookHint;
   // public GameObject hintBG;

    public void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            //hintBG.SetActive(true);
            if (notebookHint.IsShow) {
                notebookHint.Hide();
            }
            else {
                notebookHint.Show();
            }
            //notebookHint.Show();
        });
    }

}