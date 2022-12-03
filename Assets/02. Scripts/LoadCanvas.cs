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
    private void Awake() {
        gameTimeManager = this.GetSystem<GameTimeManager>();
        bg = transform.Find("BG").GetComponent<Image>();
        message = transform.Find("Message").GetComponent<TMP_Text>();
        this.GetModel<GameSceneModel>().GameScene.RegisterOnValueChaned(OnGameSceneChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
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
                BackButton.Singleton.Hide();
            }
            else {
                BackButton.Singleton.Show();
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

    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
