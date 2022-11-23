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
    public static NotebookPanel notebookPanel;
    private GameObject panel;
    private Button backButton;
    private NotebookWritePage writePage;
    public NotebookPage notebookPage;

    private void Awake()
    {
        notebookPanel = this;
        panel = transform.Find("Panel").gameObject;
        backButton = panel.transform.Find("BackButton").GetComponent<Button>();
        writePage = panel.transform.Find("NotebookWritePage").GetComponent<NotebookWritePage>();
        backButton.onClick.AddListener(OnBackButtonClicked);
    }
    public override void OnDayEnd()
    {
        Hide();
    }
    
    private void OnBackButtonClicked() {
        Hide();
    }    
    public override void Show()
    {
        panel.gameObject.SetActive(true);
    }

    public override void Hide() {
        panel.gameObject.GetComponent<Animator>().CrossFade("Stop", 0.5f);
        this.Delay(0.5f, () => {
            panel.gameObject.SetActive(false);
        });
    }

    //生成page
    public void CeratePage()
    {
        NotebookPage np = Instantiate(notebookPage).GetComponent<NotebookPage>();
        np.pageContentText = writePage.inputField.text;
    }
}
