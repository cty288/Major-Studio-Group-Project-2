using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour {
    private Button newGameButton;
    private Button quitGameButton;
    private Button loadGameButton;

    private void Awake() {
        newGameButton = transform.Find("NewGameButton").GetComponent<Button>();
        quitGameButton = transform.Find("QuitGameButton").GetComponent<Button>();
        loadGameButton = transform.Find("LoadGameButton").GetComponent<Button>();
        
        newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        quitGameButton.onClick.AddListener(OnQuitGameButtonClicked);
        loadGameButton.onClick.AddListener(OnLoadGameButtonClicked);
        
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
        
        SceneManager.LoadScene("Opening");
    }
}
