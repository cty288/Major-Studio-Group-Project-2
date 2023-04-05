using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class NotebookHintBG : MonoBehaviour
{
    public NotebookHint notebookHint;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {   
            notebookHint.Hide();
            this.gameObject.SetActive(false);
        });
    }
}
