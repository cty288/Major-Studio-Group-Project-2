using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningPanel : MonoBehaviour
{
    [SerializeField] private GameObject warn_panel;
    [SerializeField] private GameObject[] prompts;
    public void Show(int index)
    {
        warn_panel.SetActive(true);
        for (int i = 0; i < prompts.Length; i++)
        {
            if (i != index) prompts[i].SetActive(false);
            else prompts[i].SetActive(true);
        }
    }

    public void Hide()
    {
        warn_panel.SetActive(false);
    }
    
}