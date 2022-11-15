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
using Random = UnityEngine.Random;

public class NewspaperViewController : AbstractMikroController<MainGame>, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {
    private GameObject indicateCanvas;
    private GameObject dateCanvas;
    private List<SpriteRenderer> renderers = new List<SpriteRenderer>();
    private Bounds tableBounds;
    private DateTime pointerDownTime;
    private Newspaper news;
    private NewspaperSystem newspaperSystem;

    private SpriteRenderer selfRenderer;
    [SerializeField] private List<Sprite> sprites;
    private void Awake() {
        renderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();
     
        indicateCanvas = transform.Find("CanvasParent/Canvas").gameObject;
        dateCanvas = transform.Find("CanvasParent/DateCanvas").gameObject;
        indicateCanvas.SetActive(false);
        dateCanvas.SetActive(false);
        newspaperSystem = this.GetSystem<NewspaperSystem>();
        selfRenderer = GetComponent<SpriteRenderer>();

        Sprite sprite = sprites[Random.Range(0, sprites.Count)];
        selfRenderer.sprite = sprite;
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
        dateCanvas.GetComponent<Canvas>().sortingOrder = layer;
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
        if ((DateTime.Now - pointerDownTime).TotalSeconds < 0.2f) {
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

    //private bool mouseOnNewspaper = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        dateCanvas.SetActive(true);
        news.dateString = news.date.Month.ToString() + "/" + news.date.Day.ToString() + "'s Newspaper";
        dateCanvas.transform.GetChild(0).GetComponent<TMP_Text>().text = news.dateString;
        //mouseOnNewspaper = true;
    }

    
    public void OnPointerExit(PointerEventData eventData)
    {
        dateCanvas.SetActive(false);
        //mouseOnNewspaper = false;
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


