using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadCanvas : MonoMikroSingleton<LoadCanvas>, IController {
    private GameTimeManager gameTimeManager;
    private Image bg;
    private TMP_Text message;

    [SerializeField] private List<Camera> scenesToCameras = new List<Camera>();
    [SerializeField] private List<Sprite> imageSprites = new List<Sprite>();
    private Image image;
    private OutdoorActivityModel outdoorActivityModel;
    private void Awake() {
        gameTimeManager = this.GetSystem<GameTimeManager>();
        bg = transform.Find("BG").GetComponent<Image>();
        message = transform.Find("Message").GetComponent<TMP_Text>();
        outdoorActivityModel = this.GetModel<OutdoorActivityModel>();
        this.GetModel<GameSceneModel>().GameScene.RegisterOnValueChaned(OnGameSceneChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnEndOfOutdoorDayTimeEvent>(OnEndOfOutdoorDayTime).UnRegisterWhenGameObjectDestroyed(gameObject);
        image = transform.Find("Image").GetComponent<Image>();
    }

    private void OnEndOfOutdoorDayTime(OnEndOfOutdoorDayTimeEvent e) {
        bool loadFinished = false;
        Func<bool> loadCondition = () => loadFinished;

        string hint;
        if (outdoorActivityModel.IsOutdoor) {
            hint = "It's time to go home...";
        }
        else {
            //indoor, then since it's the end of the day, it's better not to go out
            hint = "It's dark outside, it's better to stay at home...";
        }

        LoadUntil(() => {
            ShowMessage(hint);
            this.Delay(3f, HideMessage);
            this.Delay(4f, () => {
                loadFinished = true;
            });
            return () => gameTimeManager.CurrentTime.Value.Hour >= gameTimeManager.NightTimeStart;
        }, () => {
            StopLoad(null);
        });

        e.OnEndOfDayTimeEventList.Add(loadCondition);

    }

    private void OnGameSceneChanged(GameScene scene) {
        Load(0.2f, () => {
            int index = (int) scene;
            for (int i = 0; i < scenesToCameras.Count; i++) {
                if (i == index) {
                    scenesToCameras[i].gameObject.SetActive(true);
                }
                else {
                    scenesToCameras[i].gameObject.SetActive(false);
                }
            }

            if (scene == GameScene.MainGame) {
                //BackButton.Singleton.Hide();
            }
            else {
               // BackButton.Singleton.Show();
            }
           
        }, null);
    }

    public void Load(float loadTime, Action onScreenBlack, Action onScreenRecover) {
        bg.raycastTarget = true;
        gameTimeManager.LockDayEnd.Retain();
        bg.DOFade(1, 0.5f).OnComplete(() => {
            onScreenBlack?.Invoke();
            this.Delay(loadTime, () => {
                bg.raycastTarget = false;
                onScreenRecover?.Invoke();
            });
            bg.DOFade(0, 0.5f).OnComplete(() => {
                gameTimeManager.LockDayEnd.Release();
            }).SetDelay(loadTime);
        });
    }

    public void Load(Action onScreenBlack) {
        bg.raycastTarget = true;
        gameTimeManager.LockDayEnd.Retain();
        bg.DOFade(1, 0.5f).OnComplete(() => {
            onScreenBlack?.Invoke();
        });
    }

    public void StopLoad(Action onScreenRecover)
    {
        bg.raycastTarget = false;
        gameTimeManager.LockDayEnd.Release();
        bg.DOFade(0, 0.5f).OnComplete(() =>
        {
            onScreenRecover?.Invoke();
        });
    }



    public void LoadUntil(Func<Func<bool>> onScreenBlack, Action onScreenRecover) {
        gameTimeManager.LockDayEnd.Retain();
        bg.raycastTarget = true;
        bg.DOFade(1, 0.5f).OnComplete(() => {
            Func<bool> condition = onScreenBlack?.Invoke();

            UntilAction action = UntilAction.Allocate(condition);

            action.OnEndedCallback += () => {
                onScreenRecover?.Invoke();
                bg.raycastTarget = false;
                gameTimeManager.LockDayEnd.Release();
            };
            
            action.Execute();
        });
    }

    public void ShowMessage(string message) {
        this.message.text = message;
        this.message.DOFade(1, 1f);
    }

    public void HideMessage() {
        this.message.DOFade(0, 1f);
    }

    public void ShowImage(int imageIndex, float duration) {
        image.sprite = imageSprites[imageIndex];
        image.DOFade(1, duration);
    }

    public void HideImage(float duration) {
        image.DOFade(0, duration);
    }

    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
