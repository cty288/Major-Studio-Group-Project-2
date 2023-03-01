using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.GameTime;
using _02._Scripts.Notebook;
using DG.Tweening;
using UnityEngine;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class NotebookPanel : OpenableUIPanel, ICanHaveDroppableItems {
    
    private GameObject panel;
    protected NotebookModel notebookModel;
    protected Collider2D leftPageSpace;
    protected Collider2D rightPageSpace;
    [SerializeField]
    protected Bounds leftPageBounds;
    [SerializeField]
    protected Bounds rightPageBounds;
    protected RectTransform contentPanel;
   
    private Dictionary<Image, float> imageAlpha = new Dictionary<Image, float>();
    protected TMP_Text dateText;
    protected Collider2D selfCollider;
    protected GameTimeModel gameTimeModel;
    
    protected Button lastPageButton;
    protected Button nextPageButton;
    
    [SerializeField] private TMP_Text droppingText;
    [SerializeField] private GameObject writtenTextPrefab;

    protected NotebookWrittenText currentWritingText = null;
    protected override void Awake() {
        base.Awake();
        panel = transform.Find("Panel").gameObject;
        notebookModel = this.GetModel<NotebookModel>();
        leftPageSpace = panel.transform.Find("NotebookContent/Left").GetComponent<Collider2D>();
        rightPageSpace = panel.transform.Find("NotebookContent/Right").GetComponent<Collider2D>();
        contentPanel = panel.transform.Find("NotebookContent/MainContentPanel").GetComponent<RectTransform>();
        dateText = panel.transform.Find("NotebookContent/DateText").GetComponent<TMP_Text>();

        lastPageButton = panel.transform.Find("LastPage").GetComponent<Button>();
        nextPageButton = panel.transform.Find("NextPage").GetComponent<Button>();
        
        this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
        selfCollider = GetComponent<Collider2D>();
        gameTimeModel = this.GetModel<GameTimeModel>();
        panel.gameObject.SetActive(true);
        this.Delay(0.1f, () => {
            panel.gameObject.SetActive(false);
        });
        selfCollider.enabled = false;
        
        var images = GetComponentsInChildren<Image>(true).ToList();
        foreach (var image in images) {
            imageAlpha.Add(image, image.color.a);
        }
        
        lastPageButton.onClick.AddListener(OnLastPageButtonClicked);
        nextPageButton.onClick.AddListener(OnNextPageButtonClicked);
    }

    private void OnNextPageButtonClicked() {
        DateTime lastNoteBookOpenTime = notebookModel.LastOpened;
        if(lastNoteBookOpenTime == DateTime.MinValue) {
            lastNoteBookOpenTime = gameTimeModel.CurrentTime.Value.Date;
        }

        if (notebookModel.HasNextNotes(lastNoteBookOpenTime, out DateTime nextTime)) {
            DestroyAllContents();
            LoadContent(nextTime,false);
            notebookModel.UpdateLastOpened(nextTime);
        }
        
    }

    private void OnLastPageButtonClicked() {
        DateTime lastNoteBookOpenTime = notebookModel.LastOpened;
        if(lastNoteBookOpenTime == DateTime.MinValue) {
            lastNoteBookOpenTime = gameTimeModel.CurrentTime.Value.Date;
        }

        if (notebookModel.HasPreviousNotes(lastNoteBookOpenTime, out DateTime nextTime)) {
            DestroyAllContents();
            LoadContent(nextTime, false);
            notebookModel.UpdateLastOpened(nextTime);
        }
    }

    private void OnNewDay(OnNewDay e) {
        notebookModel.UpdateLastOpened(e.Date, true);
    }


    public override void OnDayEnd()
    {
        Hide(0.5f);
    }
    
    private void OnBackButtonClicked() {
        Hide(0.5f);
    }    
    public override void OnShow(float time) {
        DateTime lastNoteBookOpenTime = notebookModel.LastOpened;
        if(lastNoteBookOpenTime == DateTime.MinValue) {
            lastNoteBookOpenTime = gameTimeModel.CurrentTime.Value.Date;
        }
        
        
        LoadContent(lastNoteBookOpenTime, true);
        
        this.Delay(0.05f, () => {
            panel.gameObject.SetActive(true);

            var images = GetComponentsInChildren<Image>(true).ToList();
            var texts = GetComponentsInChildren<TMP_Text>(true).ToList();
            var rawImages = GetComponentsInChildren<RawImage>(true).ToList();
            ShowPanel(images, texts, rawImages, time);
            
            
        });
        
        selfCollider.enabled = true;
    }
    
    public void ShowPanel(List<Image> images, List<TMP_Text> texts, List<RawImage> rawImages, float time) {
        foreach (var image in images) {
            if (imageAlpha.ContainsKey(image)) {
                image.DOFade(imageAlpha[image], 0.5f);
            }else {
                image.DOFade(1, time);
            }
        }
        
        foreach (var text in texts) {
            text.DOFade(1, time);
        }
        
        foreach (var rawImage in rawImages) {
            rawImage.DOFade(1, time);
        }
    }
    
    public void HidePanel(List<Image> images, List<TMP_Text> texts, List<RawImage> rawImages, float time, bool isInstant = false) {
        if (isInstant) {
            foreach (var image in images) {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            }
            foreach (var text in texts) {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            }
            foreach (var rawImage in rawImages) {
                rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, 0);
            }
        }
        else {
            foreach (var image in images) {
                image.DOFade(0, time);
            }
        
            foreach (var text in texts) {
                text.DOFade(0, time);
            }
        
            foreach (var rawImage in rawImages) {
                rawImage.DOFade(0, time);
            }
        }
       
    }
    
    

    public override void OnHide(float time) {
        var images = GetComponentsInChildren<Image>(true).ToList();
        var texts = GetComponentsInChildren<TMP_Text>(true).ToList();
        var rawImages = GetComponentsInChildren<RawImage>(true).ToList();
        HidePanel(images, texts, rawImages, time);

        this.Delay(time, () => {
            DestroyAllContents();
            panel.gameObject.SetActive(false);
        });
        selfCollider.enabled = false;
    }

    private void DestroyAllContents() {
        foreach (Transform child in contentPanel) {
            Destroy(child.gameObject);
        }
    }

    
    private Vector3 lastMousePosition;
    protected override void Update() {
        base.Update();
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0);
            lastMousePosition = mousePos;
        }
        
        if (Input.GetMouseButtonUp(0)) {
            if (currentWritingText == null) {
                Vector3 mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                mousePos = new Vector3(mousePos.x, mousePos.y, 0);

                bool mouseClickPanel = leftPageSpace.OverlapPoint(mousePos) || rightPageSpace.OverlapPoint(mousePos);
               
                if (mouseClickPanel && Vector3.Distance(lastMousePosition, mousePos) < 0.1f) {
                    CreateWritingText(mousePos);
                }
                
            }
           
        }
    }

    private void CreateWritingText(Vector2 mousePos) {
        DateTime lastNoteBookOpenTime = notebookModel.LastOpened;
        if(lastNoteBookOpenTime == DateTime.MinValue) {
            lastNoteBookOpenTime = gameTimeModel.CurrentTime.Value.Date;
        }

        
        NotebookWrittenTextDroppableInfo droppableInfo = new NotebookWrittenTextDroppableInfo(writtenTextPrefab);
        NotebookWrittenText droppedUIObjectViewController =
            droppableInfo.GetContentUIObject(leftPageSpace.GetComponent<RectTransform>()) as NotebookWrittenText;
        
        //need to update content.bounds
        AddContent(lastNoteBookOpenTime, mousePos, droppedUIObjectViewController.GetExtent(), droppableInfo,
            droppedUIObjectViewController, false);
        
        currentWritingText = droppedUIObjectViewController;
        currentWritingText.OnClickOutside += OnClickOutside;
    }

    private void OnClickOutside(string str) {

        currentWritingText.DroppableInfo.Bounds =
            new Bounds(currentWritingText.transform.position, currentWritingText.GetExtent());
        currentWritingText.DisableInteractable();
        currentWritingText.OnClickOutside -= OnClickOutside;
        StartCoroutine(DelayedUpdateCurrentWritingText(str));
    }
    
    private IEnumerator DelayedUpdateCurrentWritingText(string str) {
        yield return null;
         
        if (String.IsNullOrEmpty(str)) {
            notebookModel.RemoveNotes(currentWritingText.DroppableInfo);
            Destroy(currentWritingText.gameObject);
        }
        
        
        currentWritingText = null;
    }

    public void LoadContent(DateTime date, bool hide) {
        List<DroppableInfo> infos = notebookModel.GetNotes(date);
        if(infos!=null && infos.Count>0){
            foreach (DroppableInfo content in infos) {
                DroppedUIObjectViewController droppedUIObjectViewController =
                    content.GetContentUIObject(leftPageSpace.GetComponent<RectTransform>());
               StartCoroutine(PlaceContent(droppedUIObjectViewController, content, hide));
            }
        }
        
        dateText.text = CalenderViewController.GetMonthAbbreviation(date.Month) + " " +
                        date.Day;

        lastPageButton.gameObject.SetActive(notebookModel.HasPreviousNotes(date, out var dates));
        nextPageButton.gameObject.SetActive(notebookModel.HasNextNotes(date, out dates));
    }

    private Vector2 FindAvailableSpace(DroppableInfo content, Vector2 extent, bool isLeft, DateTime date) {
        Vector2 uiExtent = extent;
        List<DroppableInfo> allContents = notebookModel.GetNotes(date);

        Bounds targetBounds = isLeft ? leftPageBounds : rightPageBounds;
        
        //use DroppableInfo.Extent and DroppableInfo.Position to calculate the available space withing targetBounds
        //return the position of the place where the content can be dropped
        
        Vector2 tryPos = new Vector2(Random.Range(targetBounds.min.x + uiExtent.x, targetBounds.max.x - uiExtent.x),
            Random.Range(targetBounds.min.y + uiExtent.y, targetBounds.max.y - uiExtent.y));
        
        
        int tryCount = 0;
        while (IsOverlap(tryPos, uiExtent, allContents)) {
            tryPos = new Vector2(Random.Range(targetBounds.min.x + uiExtent.x, targetBounds.max.x - uiExtent.x),
                Random.Range(targetBounds.min.y + uiExtent.y, targetBounds.max.y - uiExtent.y));
            tryCount++;
            if (tryCount > 100) {
                return tryPos;
            }
        }

        return tryPos;
    }
    
    private bool IsOverlap(Vector3 pos, Vector3 extent, List<DroppableInfo> allContents) {
        foreach (DroppableInfo content in allContents) {
            if (IsOverlap(new Bounds(pos,extent), content.Bounds)) {
                return true;
            }
        }

        return false;
    }
    private bool IsOverlap(Bounds bounds1, Bounds bounds2) {
        return bounds1.min.x < bounds2.max.x && bounds1.max.x > bounds2.min.x &&
               bounds1.min.y < bounds2.max.y && bounds1.max.y > bounds2.min.y;
    }


    private IEnumerator PlaceContent(DroppedUIObjectViewController droppedUIObjectViewController,DroppableInfo content, bool hide) {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(droppedUIObjectViewController.transform.GetComponent<RectTransform>());
        droppedUIObjectViewController.DroppableInfo = content;
        
        droppedUIObjectViewController.transform.SetParent(contentPanel);
        droppedUIObjectViewController.transform.SetAsLastSibling();
        RectTransform rectTransform = droppedUIObjectViewController.GetComponent<RectTransform>();
        droppedUIObjectViewController.Bounds = new[] {rightPageBounds, leftPageBounds};


        rectTransform.position = content.Bounds.center;
        rectTransform.localScale = Vector3.one;

        if (hide) {
            var images = droppedUIObjectViewController.GetComponentsInChildren<Image>(true).ToList();
            var texts = droppedUIObjectViewController.GetComponentsInChildren<TMP_Text>(true).ToList();
            var rawImages = droppedUIObjectViewController.GetComponentsInChildren<RawImage>(true).ToList();
            HidePanel(images, texts, rawImages, 0, true);
        }
       


    }
    
    
    private IEnumerator AddContent(DateTime day, DroppableInfo content,
        DroppedUIObjectViewController droppedUIObjectViewController, bool leftOrRight, bool hide) {
        
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(droppedUIObjectViewController.transform.GetComponent<RectTransform>());
        

        Vector2 extent = droppedUIObjectViewController.GetExtent();
        //convert extent to world space
        extent = contentPanel.TransformVector(extent);
        
        Vector2 placedPos = FindAvailableSpace(content, extent, leftOrRight, day);
        Debug.Log("placedPos: " + placedPos);

        AddContent(day, placedPos, extent, content, droppedUIObjectViewController, hide);
    }

    private void AddContent(DateTime day, Vector2 pos, Vector2 extent, DroppableInfo content,
        DroppedUIObjectViewController droppedUIObjectViewController, bool hide) {
        droppedUIObjectViewController.DroppableInfo = content;
        
        content.Bounds = new Bounds(pos, extent);
        
        notebookModel.AddNote(content, day);

        if (hide) {
            Destroy(droppedUIObjectViewController.gameObject);
        }
        else {
            StartCoroutine(PlaceContent(droppedUIObjectViewController, content, false));
        }

    }

    public void AddContent(DateTime day, DroppableInfo content, bool hide) {
        bool leftOrRight = content.IsDefaultLeftPage;
        DroppedUIObjectViewController droppedUIObjectViewController = content.GetContentUIObject(leftOrRight
            ? leftPageSpace.GetComponent<RectTransform>()
            : rightPageSpace.GetComponent<RectTransform>());

        CoroutineRunner.Singleton.StartCoroutine(AddContent(day, content, droppedUIObjectViewController, leftOrRight, hide));

        if (hide) {
            notebookModel.UpdateLastOpened(gameTimeModel.CurrentTime.Value);
        }
        
    }


    public override bool AdditionMouseExitCheck() {
        return DroppedUIObjectViewController.CurrentDroppingItem == null &&
               String.IsNullOrEmpty(SubtitleHightlightedTextDragger.CurrentDraggedText);
    }

    public void OnEnter(IDroppable content) {
        droppingText.gameObject.SetActive(true);
        droppingText.text = "Add to the Notebook";
    }

    public void OnExit(IDroppable content) {
        droppingText.gameObject.SetActive(false);
    }

    public void OnDrop(IDroppable content) {
        DateTime lastNoteBookOpenTime = notebookModel.LastOpened;
        if(lastNoteBookOpenTime == DateTime.MinValue) {
            lastNoteBookOpenTime = gameTimeModel.CurrentTime.Value.Date;
        }
        
        droppingText.gameObject.SetActive(false);
        AddContent(lastNoteBookOpenTime, content.GetDroppableInfo(), !IsShow);
    }
}
