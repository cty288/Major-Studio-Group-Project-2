using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewspaperUIPanel : OpenableUIPanel {
  
    private GameObject panel;
    private TMP_Text dateText;

    private Button backButton;

    [SerializeField] private List<RawImage> imageContainers = new List<RawImage>();
    private List<GameObject> savedSpawnedImages = new List<GameObject>();
    private Newspaper lastNewspaper = null;
    [SerializeField] private List<Image> symbolImages = new List<Image>();
    [SerializeField] private List<Sprite> symbols = new List<Sprite>();

    protected Newspaper news;
    public override void OnDayEnd() {
        Hide();
    }

    protected override void Awake() {
        base.Awake();
        panel = transform.Find("Panel").gameObject;
        backButton = panel.transform.Find("BackButton").GetComponent<Button>();
        dateText = panel.transform.Find("DateText").GetComponent<TMP_Text>();
        backButton.onClick.AddListener(OnBackButtonClicked);
        this.RegisterEvent<OnNewspaperUIPanelOpened>(OnNewspaperUIPanelOpened)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
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
        Hide();
    }    

    public void Show(Newspaper news) {
        dateText.text = news.dateString;
        panel.gameObject.SetActive(true);

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
                hintText.text = "This person is dead.";
            }
            else {
                symbolImages[i].sprite = symbols[1];
                hintText.text = "We are not sure if they are dead or not.";
            }

            
            
        }


        lastNewspaper = news;
    }

    public override void Show() {
        Show(news);
    }

    public override void Hide() {
        panel.gameObject.GetComponent<Animator>().CrossFade("Stop", 0.5f);
        this.Delay(0.5f, () => {
            panel.gameObject.SetActive(false);
        });
    }
}
