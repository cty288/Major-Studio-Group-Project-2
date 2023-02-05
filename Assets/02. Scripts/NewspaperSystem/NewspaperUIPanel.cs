using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewspaperUIPanel : OpenableUIPanel {
  
    private GameObject panel;
    private TMP_Text dateText;
    protected GameObject outOfDateText;
    
   

    [SerializeField] private List<RawImage> imageContainers = new List<RawImage>();
    private List<GameObject> savedSpawnedImages = new List<GameObject>();
    private Newspaper lastNewspaper = null;
    [SerializeField] private List<Image> symbolImages = new List<Image>();
    [SerializeField] private List<Sprite> symbols = new List<Sprite>();
    private GameTimeManager gameTimeManager;
    
    protected Newspaper news;

    protected Dictionary<Newspaper, GameObject> newspaperMarkers = new Dictionary<Newspaper, GameObject>();

    protected GameObject currentMarker = null;
    protected LineRenderer currentLineRenderer = null;
    [SerializeField] private GameObject lineRendererPrefab;
    
    protected Collider2D markerArea;
    protected Vector2 lastMarkerPosition;
    
    public override void OnDayEnd() {
        Hide(0.5f);
        
    }

    protected override void Awake() {
        base.Awake();
        panel = transform.Find("Panel").gameObject;
        //backButton = panel.transform.Find("BackButton").GetComponent<Button>();
        dateText = panel.transform.Find("DateText").GetComponent<TMP_Text>();
        //backButton.onClick.AddListener(OnBackButtonClicked);
        gameTimeManager = this.GetSystem<GameTimeManager>();
        outOfDateText = panel.transform.Find("OutOfDateText").gameObject;
        markerArea = panel.transform.Find("MarkerArea").GetComponent<Collider2D>();
        this.RegisterEvent<OnNewspaperUIPanelOpened>(OnNewspaperUIPanelOpened)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnNewspaperThrown>(OnNewspaperThrown)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnNewspaperThrown(OnNewspaperThrown e) {
        if(newspaperMarkers.ContainsKey(e.Newspaper)) {
            Destroy(newspaperMarkers[e.Newspaper]);
            newspaperMarkers.Remove(e.Newspaper);
        }
    }

    private void OnNewspaperUIPanelOpened(OnNewspaperUIPanelOpened e) {
        if (e.IsOpen) {
            news = e.Newspaper;
            Show(e.Newspaper);
        }
        else {
            //Hide();
        }
    }

    private void OnBackButtonClicked() {
        Hide(0.5f);
    }    

    public void Show(Newspaper news) {
        colliders.ForEach((collider2D => {
            collider2D.enabled = true;
        } ));
        currentMarker = null;
        
        outOfDateText.SetActive(gameTimeManager.CurrentTime.Value.Day - news.date.Day >= 3);
        
        dateText.text = news.dateString;
        panel.gameObject.SetActive(true);
        this.Delay(0.5f, () => {
            isShow = true;
        });
        AudioSystem.Singleton.Play2DSound("pick_up_newspaper");
        
        if (!newspaperMarkers.ContainsKey(news)) {
            LineRenderer marker = new GameObject("NewspaperMarker").AddComponent<LineRenderer>();
            marker.transform.SetParent(panel.transform);
            newspaperMarkers.Add(news, marker.gameObject);
           
        }

        lastMarkerPosition = Vector2.zero;
        currentMarker = newspaperMarkers[news];
        currentMarker.SetActive(true);
        
        
        if (lastNewspaper == news && savedSpawnedImages.Count > 0) {
            return;
        }

        foreach (GameObject image in savedSpawnedImages) {
            GameObject.Destroy(image.gameObject);
        }

        

        for (int i = 0; i < news.timeInfos.Count; i++) {
            BodyTimeInfo info = news.timeInfos[i];
            BodyInfo bodyInfo = info.BodyInfo;
            
            GameObject spawnedBody = AlienBody.BuildNewspaperAlienBody(bodyInfo, i);
            savedSpawnedImages.Add(spawnedBody);
            Camera camera = spawnedBody.GetComponentInChildren<Camera>();
            RenderTexture renderTexture = camera.targetTexture;
            imageContainers[i].texture = renderTexture;
            imageContainers[i].GetComponent<IHaveBodyInfo>().BodyInfo = bodyInfo;

            TMP_Text hintText = symbolImages[i].GetComponentInChildren<TMP_Text>(true);
            if (info.DayRemaining == 3) {
                symbolImages[i].sprite = symbols[0];
                hintText.text = "This person died today.";
            }
            else {
                symbolImages[i].sprite = symbols[1];
                hintText.text = "Not sure whether they are dead.";
            }

            imageContainers[i].GetComponent<BountyHuntingSelector>().SetHintText(GetShortDescription(bodyInfo));

        }


        lastNewspaper = news;
    }


    protected override void Update() {
        base.Update();
        if (panel.activeInHierarchy && currentMarker) {
            if (Input.GetMouseButtonDown(0)) {
                currentLineRenderer = Instantiate(lineRendererPrefab, currentMarker.transform).GetComponent<LineRenderer>();
            }
            
            if (Input.GetMouseButton(0)) {
                Vector2 mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                
                Debug.Log("MousePosWorld: " + mousePosWorld + " MarkerArea: " + markerArea.bounds + "MousePos: " + Input.mousePosition);
                if(Vector2.Distance(lastMarkerPosition, Input.mousePosition) > 1) {
                    //check if the mouse is in the marker area. The canvas is screen space camera
                    Vector3 mousePosWorldFixed = new Vector3(mousePosWorld.x, mousePosWorld.y,
                        markerArea.bounds.center.z);

                    if (markerArea.bounds.Contains(mousePosWorldFixed)) {
                        var positionCount = currentLineRenderer.positionCount;
                        positionCount++;
                        currentLineRenderer.positionCount = positionCount;
                        Vector3 targetPos = new Vector3(mousePosWorld.x, mousePosWorld.y, -15);
                        currentLineRenderer.SetPosition(positionCount - 1, targetPos);
                    }
                    
                    lastMarkerPosition = Input.mousePosition;
                    
                }
            }
        }
    }

    public string GetShortDescription(BodyInfo bodyInfo) {
        List<IAlienTag> tags = new List<IAlienTag>();
        
        bodyInfo.HeadInfoPrefab.Tags.ForEach((alienTag => {
            List<string> shortDescriptions = alienTag.GetShortDescriptions();
            if(shortDescriptions.Count > 0 && !string.IsNullOrEmpty(shortDescriptions[0])) {
                tags.Add(alienTag);
            }
        }));
        
        bodyInfo.MainBodyInfoPrefab.Tags.ForEach((alienTag => {
            List<string> shortDescriptions = alienTag.GetShortDescriptions();
            if(shortDescriptions.Count > 0 && !string.IsNullOrEmpty(shortDescriptions[0])) {
                tags.Add(alienTag);
            }
        }));
        
        
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < tags.Count; i++) {
            if (i == 2) {
                break;
            }
            if (i == 0) stringBuilder.Append(tags[i].GetShortDescriptions()[0]);
            else stringBuilder.Append("\n").Append(tags[i].GetShortDescriptions()[0]);
        }

        return stringBuilder.ToString();
    }
    public override void OnShow(float time) {
        Show(news);
    }

    public override void OnHide(float time) {
        panel.gameObject.GetComponent<Animator>().CrossFade("Stop", time);
        this.Delay(time, () => {
            panel.gameObject.SetActive(false);
        });
        AudioSystem.Singleton.Play2DSound("put_down_newspaper");
        if (currentMarker) {
            currentMarker.SetActive(false);
        }

        currentLineRenderer = null; 
        colliders.ForEach((collider2D => {
            collider2D.enabled = false;
        } ));
    }
}
