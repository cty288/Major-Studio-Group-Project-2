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

public class NotebookPagePanel : OpenableUIPanel
{
    public static NotebookPagePanel notebookPagePanel;    
    public TMP_Text pageText;
    private GameObject panel;
    private Button backButton;
    public NotebookPage page;

    public override void OnDayEnd()
    {
        Hide();
    }
    
    private void OnBackButtonClicked() {
        Hide();
    }    

    private void Awake()
    {
        notebookPagePanel = this;
        panel = transform.Find("Panel").gameObject;
        backButton = panel.transform.Find("BackButton").GetComponent<Button>();
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    public void OpenPageText(NotebookPage page)
    {
        this.page = page;
        panel.gameObject.SetActive(true);
        pageText.text = page.pageContentText;
    }

    public override void Show()
    {
        OpenPageText(page);
    }

    public override void Hide() {
        panel.gameObject.GetComponent<Animator>().CrossFade("Stop", 0.5f);
        this.Delay(0.5f, () => {
            panel.gameObject.SetActive(false);
        });
    }
}
