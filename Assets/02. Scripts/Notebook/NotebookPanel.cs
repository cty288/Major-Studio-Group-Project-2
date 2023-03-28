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
    [SerializeField] private GameObject markerPrefab;

    protected NotebookWrittenText currentWritingText = null;
    protected  NotebookMarker currentMarker = null;
    protected NotebookWritePage notebookWritePage;
    protected RectTransform markerPanel;

    protected NotebookTool penTool;
    protected NotebookTool eraserTool;
    protected NotebookTool currentTool;
    protected TMP_Text controlHint;
    
    protected  Animator flipAnimator;
    protected override void Awake() {
        base.Awake();
        panel = transform.Find("Panel").gameObject;
        notebookModel = this.GetModel<NotebookModel>();
        leftPageSpace = panel.transform.Find("NotebookContent/Left").GetComponent<Collider2D>();
        rightPageSpace = panel.transform.Find("NotebookContent/Right").GetComponent<Collider2D>();
        contentPanel = panel.transform.Find("NotebookContent/MainContentPanel").GetComponent<RectTransform>();
        markerPanel = panel.transform.Find("NotebookContent/MarkerPanel").GetComponent<RectTransform>();
        dateText = panel.transform.Find("NotebookContent/DateText").GetComponent<TMP_Text>();

        penTool = panel.transform.Find("PenButton").GetComponent<NotebookTool>();
        eraserTool = panel.transform.Find("EraserButton").GetComponent<NotebookTool>();
        penTool.OnToolClicked += OnPenToolClicked;
        eraserTool.OnToolClicked += OnEraserToolClicked;
        
        flipAnimator = panel.transform.Find("NotebookAnimation").GetComponent<Animator>();
        
        lastPageButton = panel.transform.Find("LastPage").GetComponent<Button>();
        nextPageButton = panel.transform.Find("NextPage").GetComponent<Button>();
        controlHint = panel.transform.Find("ControlHint").GetComponent<TMP_Text>();
        
        
        
        this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
        selfCollider = GetComponent<Collider2D>();
        notebookWritePage = GetComponentInChildren<NotebookWritePage>(true);
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
        notebookWritePage.OnTear += OnTear;
        
        
    }

    private void OnPenToolClicked(NotebookTool tool) {
        currentTool = tool;
        eraserTool.Deselect();
        penTool.Select();
        controlHint.text = "Hold & Drag to Mark";
    }

    private void OnEraserToolClicked(NotebookTool tool) {
        currentTool = tool;
        penTool.Deselect();
        eraserTool.Select();
        controlHint.text = "Hold & Drag to Erase";
    }


   

    protected override void OnDestroy() {
        base.OnDestroy();
        lastPageButton.onClick.RemoveListener(OnLastPageButtonClicked);
        nextPageButton.onClick.RemoveListener(OnNextPageButtonClicked);
        notebookWritePage.OnTear -= OnTear;
        penTool.OnToolClicked -= OnPenToolClicked;
        eraserTool.OnToolClicked -= OnEraserToolClicked;
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
            flipAnimator.CrossFade("FlipRight", 0f);
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
            flipAnimator.CrossFade("FlipLeft", 0f);
        }
    }
    
    private void OnTear() {
        DateTime lastNoteBookOpenTime = notebookModel.LastOpened;
        if(lastNoteBookOpenTime == DateTime.MinValue) {
            lastNoteBookOpenTime = gameTimeModel.CurrentTime.Value.Date;
        }

        DeleteNotes(lastNoteBookOpenTime, true);

    }

    private void DeleteNotes(DateTime date, bool spawnTrash) {
        notebookModel.RemoveNotes(date, spawnTrash);
        DestroyAllContents();

        DateTime next = date;
        if (notebookModel.HasPreviousNotes(date, out DateTime nextTime)) {
            next = nextTime;
        }
        notebookModel.UpdateLastOpened(nextTime);
        LoadContent(next, false);
    }


    private void OnNewDay(OnNewDay e) {
        notebookModel.UpdateLastOpened(e.Date, true);
        if (gameTimeModel.Day == 1) {
            DateTime yesterday = e.Date.AddDays(-1).Date;
            DeleteNotes(yesterday, false);
            
            notebookModel.UpdateLastOpened(e.Date);
            LoadContent(e.Date, false);
        }
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
            OnPenToolClicked(penTool);
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

        if (currentMarker != null) {
            currentMarker.StopDraw();
            currentMarker = null;
        }
    }

    private void DestroyAllContents() {
        foreach (Transform child in contentPanel) {
            Destroy(child.gameObject);
        }

        foreach (Transform child in markerPanel) {
            Destroy(child.gameObject);
        }
    }

    
    private Vector3 lastMouseDownPosition;
    protected bool lastMouseDownInMarkerPanel = false;
    protected bool CheckMouseOnEmptySpace() {
       
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos = new Vector3(mousePos.x, mousePos.y, 0);
        
        //bool mouseClickPanel = leftPageSpace.OverlapPoint(mousePos) || rightPageSpace.OverlapPoint(mousePos);

        bool mouseClickElement = false;
        bool containLeftRightPage = false;
                
        Physics2D.OverlapCircleAll(mousePos, 0.1f).ToList().ForEach((collider) => {
            if (collider.gameObject == leftPageSpace.gameObject || collider.gameObject == rightPageSpace.gameObject) {
                containLeftRightPage = true;
            }

            if (collider.gameObject.CompareTag("NotebookElement")) {
                mouseClickElement = true;
            }
                    
        });



        if (!mouseClickElement  && containLeftRightPage) {
            return true;
        }
        return false;
    }
    
    protected override void Update() {
        base.Update();
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos = new Vector3(mousePos.x, mousePos.y, 0);
        
        
        if (Input.GetMouseButtonDown(0)) {
            lastMouseDownPosition = mousePos;
            lastMouseDownInMarkerPanel = CheckMouseOnEmptySpace();
        }

        if (Input.GetMouseButton(0)) {
            if (currentWritingText == null && !PhotoPanelUI.IsOpen ) {
                if (lastMouseDownInMarkerPanel && currentMarker == null && CheckMouseOnEmptySpace() && currentTool == penTool && Vector3.Distance(lastMouseDownPosition, mousePos) > 0.1f) {
                    CreateMarker(mousePos);
                }

                if (currentTool == eraserTool) {
                    EraseMarker(mousePos);
                }
            }
        }
        
        if (Input.GetMouseButtonUp(0)) {
            if (currentWritingText == null && !PhotoPanelUI.IsOpen && currentMarker == null && currentTool == penTool) { //check whether create new text
                if (CheckMouseOnEmptySpace() && Vector3.Distance(lastMouseDownPosition, mousePos) < 0.1f) {
                    CreateWritingText(mousePos);
                }
            }
            
            
            if (currentMarker != null) {
                currentMarker.StopDraw();
                currentMarker = null;
            }

        }
    }

    private void EraseMarker(Vector3 mousePos) {
        if (markerPanel.childCount <= 0) {
            return;
        }
        
        DateTime lastNoteBookOpenTime = notebookModel.LastOpened;
        if(lastNoteBookOpenTime == DateTime.MinValue) {
            lastNoteBookOpenTime = gameTimeModel.CurrentTime.Value.Date;
        }

        List<NotebookMarker> markers = markerPanel.GetComponentsInChildren<NotebookMarker>(true).ToList();
        //Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        if (markers != null) {
            foreach (NotebookMarker info in markers) {
                NotebookMarkerDroppableInfo droppableInfo = info.DroppableInfo as NotebookMarkerDroppableInfo;
                Vector2 mouseRelative = new Vector3(mousePos.x, mousePos.y, -6);
                mouseRelative = info.transform.InverseTransformPoint(mouseRelative);
                
                
                foreach (Vector3 markerPosition in droppableInfo.markerPositions) {
                    
                    Vector2 markerPos = new Vector2(markerPosition.x, markerPosition.y);
                   
                    
                    if (Vector3.Distance(markerPos, mouseRelative) < 10f) {
                        notebookModel.RemoveNote(lastNoteBookOpenTime, droppableInfo);
                        Destroy(info.gameObject);
                        break;
                    }
                }
            }
        }
    }

    private void CreateMarker(Vector3 mousePos) {
        DateTime lastNoteBookOpenTime = notebookModel.LastOpened;
        if(lastNoteBookOpenTime == DateTime.MinValue) {
            lastNoteBookOpenTime = gameTimeModel.CurrentTime.Value.Date;
        }

        Bounds markerBounds = leftPageBounds.Contains(mousePos) ? leftPageBounds : rightPageBounds;
        NotebookMarkerDroppableInfo droppableInfo = new NotebookMarkerDroppableInfo(markerPrefab, markerBounds);
        
        NotebookMarker droppedUIObjectViewController =
            droppableInfo.GetContentUIObject(markerPanel) as NotebookMarker;
        
        //need to update content.bounds
        AddContent(lastNoteBookOpenTime, mousePos, droppedUIObjectViewController.GetExtent(), droppableInfo,
            droppedUIObjectViewController, false, markerPanel);
        
        currentMarker = droppedUIObjectViewController;
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
            droppedUIObjectViewController, false, contentPanel);
        
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
                StartCoroutine(PlaceContent(droppedUIObjectViewController, content, hide,
                    content is NotebookMarkerDroppableInfo ? markerPanel : contentPanel));
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


    private IEnumerator PlaceContent(DroppedUIObjectViewController droppedUIObjectViewController,DroppableInfo content, bool hide, RectTransform parent) {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(droppedUIObjectViewController.transform.GetComponent<RectTransform>());
        droppedUIObjectViewController.DroppableInfo = content;
        
        droppedUIObjectViewController.transform.SetParent(parent);
        //set local z to 0
       
        
        droppedUIObjectViewController.transform.SetAsLastSibling();
        RectTransform rectTransform = droppedUIObjectViewController.GetComponent<RectTransform>();
        droppedUIObjectViewController.Bounds = new[] {rightPageBounds, leftPageBounds};


        rectTransform.position = content.Bounds.center;
        
        Vector3 localPos = droppedUIObjectViewController.transform.localPosition;
        droppedUIObjectViewController.transform.localPosition = new Vector3(localPos.x, localPos.y, 0);
        rectTransform.localScale = Vector3.one;

        if (hide) {
            var images = droppedUIObjectViewController.GetComponentsInChildren<Image>(true).ToList();
            var texts = droppedUIObjectViewController.GetComponentsInChildren<TMP_Text>(true).ToList();
            var rawImages = droppedUIObjectViewController.GetComponentsInChildren<RawImage>(true).ToList();
            HidePanel(images, texts, rawImages, 0, true);
        }
       


    }
    
    
    private IEnumerator AddContent(DateTime day, DroppableInfo content,
        DroppedUIObjectViewController droppedUIObjectViewController, bool leftOrRight, bool hide, RectTransform parent, Vector3 overridePos = default) {
        
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(droppedUIObjectViewController.transform.GetComponent<RectTransform>());
        

        Vector2 extent = droppedUIObjectViewController.GetExtent();
        //convert extent to world space
        extent = contentPanel.TransformVector(extent);

        overridePos = new Vector3(overridePos.x, overridePos.y, 0);
        if (overridePos != default) {
            //check if the overridePos is inside the bounds
            if(!leftPageBounds.Contains(overridePos) && !rightPageBounds.Contains(overridePos)) {
                overridePos = default;
            }
        }
        
        Vector2 placedPos =
            overridePos == default ? FindAvailableSpace(content, extent, leftOrRight, day) : overridePos;
        
        Debug.Log("placedPos: " + placedPos);

        AddContent(day, placedPos, extent, content, droppedUIObjectViewController, hide, parent);
    }

    private void AddContent(DateTime day, Vector2 pos, Vector2 extent, DroppableInfo content,
        DroppedUIObjectViewController droppedUIObjectViewController, bool hide, RectTransform parent) {
        droppedUIObjectViewController.DroppableInfo = content;
        
        content.Bounds = new Bounds(pos, extent);
        
        notebookModel.AddNote(content, day);

        if (hide) {
            Destroy(droppedUIObjectViewController.gameObject);
        }
        else {
            StartCoroutine(PlaceContent(droppedUIObjectViewController, content, false, parent));
        }

    }

    public void AddContent(DateTime day, DroppableInfo content, bool hide,  Vector2 overridePos = default, RectTransform parent = null) {
        bool leftOrRight = content.IsDefaultLeftPage;
        DroppedUIObjectViewController droppedUIObjectViewController = content.GetContentUIObject(leftOrRight
            ? leftPageSpace.GetComponent<RectTransform>()
            : rightPageSpace.GetComponent<RectTransform>());

        if (parent == null) {
            parent = content is NotebookMarkerDroppableInfo ? markerPanel : contentPanel;
        }
        CoroutineRunner.Singleton.StartCoroutine(AddContent(day, content, droppedUIObjectViewController, leftOrRight, hide, parent, overridePos));

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
        
        Vector2 pos = default;
        if (IsShow) {
            pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

       
        droppingText.gameObject.SetActive(false);
        AddContent(lastNoteBookOpenTime, content.GetDroppableInfo(), !IsShow, pos, contentPanel);
    }
}
