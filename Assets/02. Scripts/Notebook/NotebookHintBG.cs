using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookHintBG : MonoBehaviour
{
    public bool isActive; 
    public void Awake()
    {
        isActive = false;
        GetComponent<Button>().onClick.AddListener(CloseHint);
    }

    private void CloseHint()
    {
        isActive = false;
        gameObject.SetActive(false);
    }
}
