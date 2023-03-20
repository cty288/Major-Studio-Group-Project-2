using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.UI;

public class NotebookHint : MonoBehaviour
{
    public NotebookHintBG hint;
    private void Awake()
    {
        hint.isActive = false;
        GetComponent<Button>().onClick.AddListener(OnHintClicked);
    }

    public void OnHintClicked()
    {
        hint.gameObject.SetActive(true);
        hint.isActive = true;
    }
}
