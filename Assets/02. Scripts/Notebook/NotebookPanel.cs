using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotebookPanel : OpenableUIPanel
{
    
    private GameObject panel;
    private Button backButton;
  
    protected override void Awake() {
        base.Awake();
        panel = transform.Find("Panel").gameObject;
        backButton = panel.transform.Find("BackButton").GetComponent<Button>();
        backButton.onClick.AddListener(OnBackButtonClicked);
    }
    public override void OnDayEnd()
    {
        Hide();
    }
    
    private void OnBackButtonClicked() {
        Hide();
    }    
    public override void Show() {
        panel.gameObject.SetActive(true);
        if (GetComponentInChildren<TMP_SelectionCaret>(true)) {
            GetComponentInChildren<TMP_SelectionCaret>(true).raycastTarget = true;
        }
    }

    public override void Hide() {
        panel.gameObject.GetComponent<Animator>().CrossFade("Stop", 0.5f);
        if (GetComponentInChildren<TMP_SelectionCaret>(true))
        {
            GetComponentInChildren<TMP_SelectionCaret>(true).raycastTarget = false;
        }

        this.Delay(0.5f, () => {
            panel.gameObject.SetActive(false);
        });
    }

    //生成page
    
}
