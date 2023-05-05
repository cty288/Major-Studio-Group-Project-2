using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.TimeSystem;
using MikroFramework.UIKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuPanel : AbstractPanelContainer {
    private Button newGameButton;
    private Button quitGameButton;
    private Button loadGameButton;
    private Button settingButton;

    private void Awake() {
        this.Delay(0.3f, () => { AudioSystem.Singleton.Initialize(null); });
        bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        newGameButton = transform.Find("ButtonPanels/NewGameButton").GetComponent<Button>();
        quitGameButton = transform.Find("QuitGameButton").GetComponent<Button>();
        loadGameButton = transform.Find("ButtonPanels/LoadGameButton").GetComponent<Button>();
        settingButton = transform.Find("ButtonPanels/SettingsButton").GetComponent<Button>();
        
        newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        quitGameButton.onClick.AddListener(OnQuitGameButtonClicked);
        loadGameButton.onClick.AddListener(OnLoadGameButtonClicked);
        settingButton.onClick.AddListener(OnSettingButtonClicked);
        
        Screen.fullScreen = isFullScreen;
        
        Resolution currentResolution = Screen.currentResolution;
        int resolutionWidth = PlayerPrefs.GetInt("ResolutionWidth", currentResolution.width);
        int resolutionHeight = PlayerPrefs.GetInt("ResolutionHeight", currentResolution.height);
        Screen.SetResolution(resolutionWidth, resolutionHeight, isFullScreen);
        
    }

    private void OnSettingButtonClicked() {
        UIManager.Singleton.Open<MainMenuSetting>(Parent, null);
    }

    private void Start() {
        loadGameButton.gameObject.SetActive(ES3.FileExists("saveFile.es3"));
        
    }

    private void OnLoadGameButtonClicked() {
        ToOpening();
    }

    private void OnQuitGameButtonClicked() {
        Application.Quit();
    }

    private void OnNewGameButtonClicked() {
        ClearDataAndStart();
    }
    
    private void ClearDataAndStart() {
       MainGame.ClearSave();
        ToOpening();
    }

    protected void ToOpening() {
        //Architecture<MainGame>.ResetArchitecture();
        
        //MainGame.Interface.ResetArchitecture();
        foreach (var updater in GameObject.FindObjectsOfType<TimeSystem.TimeSystemUpdate>()) {
            Destroy(updater.gameObject);
        }
        
        foreach (var updater in GameObject.FindObjectsOfType<ElectricitySystemUpdater>()) {
            Destroy(updater.gameObject);
        }
        foreach (var updater in GameObject.FindObjectsOfType<GameEventSystemUpdater>()) {
            Destroy(updater.gameObject);
        }
        
        foreach (var updater in GameObject.FindObjectsOfType<TelephoneSystemUpdater>()) {
            Destroy(updater.gameObject);
        }
        SceneManager.LoadScene("Opening");
    }

    public override void OnInit() {
        
    }

    public override void OnOpen(UIMsg msg) {
        
    }

    public override void OnClosed() {
        
    }
}
