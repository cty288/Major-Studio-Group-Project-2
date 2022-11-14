using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewspaperUIPanel : AbstractMikroController<MainGame> {
  
    private GameObject panel;
    private TMP_Text dateText;

    private Button backButton;

    [SerializeField] private List<RawImage> imageContainers = new List<RawImage>();
    private List<GameObject> savedSpawnedImages = new List<GameObject>();
    private Newspaper lastNewspaper = null;

    private void Awake() {
        panel = transform.Find("Panel").gameObject;
        backButton = panel.transform.Find("BackButton").GetComponent<Button>();
        dateText = panel.transform.Find("DateText").GetComponent<TMP_Text>();
        backButton.onClick.AddListener(OnBackButtonClicked);
        this.RegisterEvent<OnNewspaperUIPanelOpened>(OnNewspaperUIPanelOpened)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnNewspaperUIPanelOpened(OnNewspaperUIPanelOpened e) {
        if (e.IsOpen) {
            Show(e.Newspaper);
        }
        else {
            Hide();
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

        int i = 0;
        foreach (BodyTimeInfo info in news.timeInfos) {
            BodyInfo bodyInfo = info.BodyInfo;
            GameObject spawnedBody = AlienBody.BuildNewspaperAlienBody(bodyInfo, i);
            savedSpawnedImages.Add(spawnedBody);
            Camera camera = spawnedBody.GetComponentInChildren<Camera>();
            RenderTexture renderTexture = camera.targetTexture;
            imageContainers[i].texture = renderTexture;
            i++;
        }

        lastNewspaper = news;
    }

    public void Hide() {
        panel.gameObject.GetComponent<Animator>().CrossFade("Stop", 0.5f);
        this.Delay(0.5f, () => {
            panel.gameObject.SetActive(false);
        });
    }
}
