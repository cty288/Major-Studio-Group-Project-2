using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class NotebookPage : DraggableItems, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject pageCanvas;
    public string pageContentText;
    private List<SpriteRenderer> renderers = new List<SpriteRenderer>();
    private DateTime pointerDownTime;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();
        pageCanvas = transform.Find("CanvasParent/PageCanvas").gameObject;
        pageCanvas.SetActive(false);

    }

    public void OpenPage()
    {
        NotebookPagePanel.notebookPagePanel.OpenPageText(this);
    }
    
    public void OnPointerDown(PointerEventData eventData) {
        pointerDownTime = DateTime.Now;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if ((DateTime.Now - pointerDownTime).TotalSeconds < 0.2f) {
            OnClick();
        }
    }
    
    private void OnClick() {
        Debug.Log("OnClick");
        this.Delay(0.1f, () => {
            if (this) {
                OpenPage();
            }
        });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pageCanvas.SetActive(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        pageCanvas.SetActive(false);
    }

    public override void SetLayer(int layer)
    {
        foreach (var renderer in renderers) {
            renderer.sortingOrder = layer;
        }
        pageCanvas.GetComponent<Canvas>().sortingOrder = layer;
    }
}
