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
    private NotebookWritePage notebookWritePage;

    protected override void Awake() {
        base.Awake();
        panel = transform.Find("Panel").gameObject;
        notebookWritePage = GetComponentInChildren<NotebookWritePage>(true);
        panel.gameObject.SetActive(true);
        this.Delay(0.1f, () => {
            panel.gameObject.SetActive(false);
        });
    }
    public override void OnDayEnd()
    {
        Hide(0.5f);
    }
    
    private void OnBackButtonClicked() {
        Hide(0.5f);
    }    
    public override void OnShow(float time) {
        panel.gameObject.SetActive(true);
        if (GetComponentInChildren<TMP_SelectionCaret>(true)) {
            GetComponentInChildren<TMP_SelectionCaret>(true).raycastTarget = true;
        }
    }

    public override void OnHide(float time) {
        panel.gameObject.GetComponent<Animator>().CrossFade("Stop", time);
        if (GetComponentInChildren<TMP_SelectionCaret>(true))
        {
            GetComponentInChildren<TMP_SelectionCaret>(true).raycastTarget = false;
        }

        this.Delay(time, () => {
            panel.gameObject.SetActive(false);
        });
    }

    public void TryWriteText(string text) {
        notebookWritePage.TryWriteNewLine(text);
    }


    public bool CanWriteNewLine() {
        return notebookWritePage.CanAddNewLine();
    }
    //生成page
    
}
