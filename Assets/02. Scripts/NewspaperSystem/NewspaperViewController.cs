using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewspaperViewController : AbstractMikroController<MainGame>, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler {
    private GameObject indicateCanvas;
    private List<SpriteRenderer> renderers = new List<SpriteRenderer>();
    private Bounds tableBounds;
    private DateTime pointerDownTime;
    private Newspaper news;
    private NewspaperSystem newspaperSystem;
    private void Awake() {
        renderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();
     
        indicateCanvas = transform.Find("CanvasParent/Canvas").gameObject;
        indicateCanvas.SetActive(false);
        newspaperSystem = this.GetSystem<NewspaperSystem>();
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
        this.news = news;
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
        if ((DateTime.Now - pointerDownTime).TotalSeconds < 0.1f) {
            OnClick();
        }
    }

    private void OnClick() {
        Debug.Log("OnClick");
        this.Delay(0.1f, () => {
            if (this) {
                this.SendCommand<OpenNewspaperUIPanelCommand>(new OpenNewspaperUIPanelCommand(news, true));
            }
        });
    }

    private Vector2 dragStartPos;
    public void OnBeginDrag(PointerEventData eventData) {
        dragStartPos = transform.position;
        newspaperSystem.CurrentHoldingNewspaper = this;
    }

    public void OnEndDrag(PointerEventData eventData) {
        var pos = transform.position;
        if (!tableBounds.Contains(pos)) {
            transform.DOMove(dragStartPos, 0.5f);
        }

        newspaperSystem.CurrentHoldingNewspaper = null;
    }
}

public class OpenNewspaperUIPanelCommand : AbstractCommand<OpenNewspaperUIPanelCommand> {
    private Newspaper newspaper;
    private bool isOpen;
    public OpenNewspaperUIPanelCommand(){}
    public OpenNewspaperUIPanelCommand(Newspaper news, bool isOpen) {
        this.newspaper = news;
        this.isOpen = isOpen;
    }
    protected override void OnExecute() {
        this.SendEvent<OnNewspaperUIPanelOpened>(
            new OnNewspaperUIPanelOpened() {Newspaper = newspaper, IsOpen = isOpen});
    }
}

public struct OnNewspaperUIPanelOpened {
    public Newspaper Newspaper;
    public bool IsOpen;
}


