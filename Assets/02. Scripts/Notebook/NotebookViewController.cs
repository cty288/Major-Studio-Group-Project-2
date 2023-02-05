using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NotebookViewController : AbstractMikroController<MainGame>, IPointerClickHandler, ICanHaveDroppedTexts{
    [SerializeField] private OpenableUIPanel panel;
    private MouseHoverOutline mouseHoverOutline;
    [SerializeField] private TMP_Text droppingText;

    private void Awake() {
        mouseHoverOutline = GetComponent<MouseHoverOutline>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        panel.Show(0.5f);
    }

    public void OnEnter(string text) {
        mouseHoverOutline.StartHovering(true);
        droppingText.gameObject.SetActive(true);
        NotebookPanel notebookPanel = panel as NotebookPanel;
        if (notebookPanel.CanWriteNewLine()) {
            droppingText.text = "Add to notebook";
        }
        else {
            droppingText.text = "<color=red>The current page is full!</color>";
        }
    }

    public void OnExit(string text) {
       mouseHoverOutline.StopHovering();
       droppingText.gameObject.SetActive(false);
    }

    public void OnDrop(string text) {
        NotebookPanel notebookPanel = panel as NotebookPanel;
        if (notebookPanel.CanWriteNewLine()) {
            notebookPanel.TryWriteText(text);
        }
        droppingText.gameObject.SetActive(false);
    }
}
