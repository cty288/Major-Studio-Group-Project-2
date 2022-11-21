using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Singletons;
using UnityEngine;
using UnityEngine.UI;

public class LoadCanvas : MonoMikroSingleton<LoadCanvas>, IController {
    private GameTimeManager gameTimeManager;
    private Image bg;

    [SerializeField] private List<Camera> scenesToCameras = new List<Camera>();
    private void Awake() {
        gameTimeManager = this.GetSystem<GameTimeManager>();
        bg = transform.Find("BG").GetComponent<Image>();
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
        gameTimeManager.LockDayEnd.Retain();
        bg.DOFade(1, 0.5f).OnComplete(() => {
            onScreenBlack?.Invoke();
            bg.DOFade(0, 0.5f).OnComplete(() => {
                onScreenRecover?.Invoke();
                gameTimeManager.LockDayEnd.Release();
            }).SetDelay(loadTime);
        });
    }

    public void LoadUntil(Action onScreenBlack, Action onScreenRecover, Func<bool> condition) {
        gameTimeManager.LockDayEnd.Retain();
        
        bg.DOFade(1, 0.5f).OnComplete(() => {
            onScreenBlack?.Invoke();

            UntilAction action = UntilAction.Allocate(condition);

            action.OnEndedCallback += () => {
                onScreenRecover?.Invoke();
                gameTimeManager.LockDayEnd.Release();
            };
            
            action.Execute();
        });
    }

    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
