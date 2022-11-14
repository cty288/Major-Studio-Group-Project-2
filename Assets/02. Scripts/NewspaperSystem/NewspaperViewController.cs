using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewspaperViewController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler {
    private GameObject indicateCanvas;
    private List<SpriteRenderer> renderers = new List<SpriteRenderer>();
    private Bounds tableBounds;
    private DateTime pointerDownTime;
    private void Awake() {
        renderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();
     
        indicateCanvas = transform.Find("CanvasParent/Canvas").gameObject;
        indicateCanvas.SetActive(false);
    }

    public void StartIndicateTodayNewspaper() {
        indicateCanvas.SetActive(true);
    }

    public void StopIndicateTodayNewspaper() {
        indicateCanvas.SetActive(false);
    }

    public void SetLayer(int layer) {
        foreach (var renderer in renderers) {
            renderer.sortingOrder = layer;
        }
        indicateCanvas.GetComponent<Canvas>().sortingOrder = layer;
    }

    public void SetContent(Newspaper news, Bounds tableBounds) {
        this.tableBounds = tableBounds;
    }

    public void OnDrag(PointerEventData eventData) {
        var pos = Camera.main.ScreenToWorldPoint(eventData.position);
        pos.z = 0;
        transform.position = pos;
    }

    public void OnPointerDown(PointerEventData eventData) {
        pointerDownTime = DateTime.Now;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if ((DateTime.Now - pointerDownTime).TotalSeconds < 0.3f) {
            OnClick();
        }
    }

    private void OnClick() {
        Debug.Log("OnClick");
    }

    private Vector2 dragStartPos;
    public void OnBeginDrag(PointerEventData eventData) {
        dragStartPos = transform.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        var pos = transform.position;
        if (!tableBounds.Contains(pos)) {
            transform.DOMove(dragStartPos, 0.5f);
        }
    }
}
