using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using _02._Scripts.Notebook;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NotebookViewController : AbstractCanHaveDroppableItems, IPointerClickHandler, ICanHaveDroppableItems{
    [SerializeField] private OpenableUIPanel panel;
    private MouseHoverOutline mouseHoverOutline;
    [SerializeField] private TMP_Text droppingText;
    protected GameTimeModel gameTimeModel;

    protected override void Awake() {
        base.Awake();
        mouseHoverOutline = GetComponent<MouseHoverOutline>();
        gameTimeModel = this.GetModel<GameTimeModel>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        panel.Show(0.5f);
    }

    
    public override void OnEnter(IDroppable content) {
        mouseHoverOutline.StartHovering(true);
        droppingText.gameObject.SetActive(true);
        NotebookPanel notebookPanel = panel as NotebookPanel;
        droppingText.text = "Add to notebook";
       
    }

    public override void OnExit(IDroppable content) {
        mouseHoverOutline.StopHovering();
        droppingText.gameObject.SetActive(false);
    }

    public override void OnDrop(IDroppable content) {
        NotebookPanel notebookPanel = panel as NotebookPanel;

        notebookPanel.AddContent(gameTimeModel.CurrentTime.Value, content.GetDroppableInfo(), !panel.IsShow);
        
        droppingText.gameObject.SetActive(false);
    }
}
