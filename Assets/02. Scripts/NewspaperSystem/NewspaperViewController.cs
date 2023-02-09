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

public struct OnNewspaperThrown {
    public Newspaper Newspaper;
}
public class NewspaperViewController : DraggableItems, IPointerEnterHandler, IPointerExitHandler, ICanSendEvent {
    private GameObject indicateCanvas;
    private GameObject dateCanvas;
    private List<SpriteRenderer> renderers = new List<SpriteRenderer>();
   

    private Newspaper news;
    private NewspaperSystem newspaperSystem;

    private SpriteRenderer selfRenderer;
    [SerializeField] private List<Sprite> sprites;
    
    
    protected override void Awake() {
        base.Awake();
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

    public override void SetLayer(int layer) {
        foreach (var renderer in renderers) {
            renderer.sortingOrder = layer;
        }
        indicateCanvas.GetComponent<Canvas>().sortingOrder = layer;
        dateCanvas.GetComponent<Canvas>().sortingOrder = 1000;
    }

    public void SetContent(Newspaper news) {
        this.news = news;
    }


  

    protected override void OnClick() {
        Debug.Log("OnClick");
        this.Delay(0.1f, () => {
            if (this) {
                this.SendCommand<OpenNewspaperUIPanelCommand>(new OpenNewspaperUIPanelCommand(news, true));
            }
        });
    }

    public override void OnThrownToRubbishBin() {
        this.SendEvent<OnNewspaperThrown>(new OnNewspaperThrown() {
            Newspaper = news
        });
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


