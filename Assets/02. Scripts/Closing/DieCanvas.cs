using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Singletons;
using MikroFramework.TimeSystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DieCanvas : MonoMikroSingleton<DieCanvas>, IController {
    private Button restartButton;
    private Button restartTodayButton;

    private void Awake() {
        restartButton = transform.Find("Panel/RestartButton").GetComponent<Button>();
        restartTodayButton = transform.Find("Panel/RestartFromToday").GetComponent<Button>();
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        restartTodayButton.onClick.AddListener(OnRestartTodayButtonClicked);
    }

    private void OnRestartTodayButtonClicked() {
        BackToOpening();
    }

    private void OnRestartButtonClicked() {
        MainGame.ClearSave();
        BackToOpening();
    }

    protected void BackToOpening() {
        Architecture<MainGame>.ResetArchitecture();
        
        //MainGame.Interface.ResetArchitecture();
        foreach (var updater in GameObject.FindObjectsOfType<TimeSystem.TimeSystemUpdate>()) {
            Destroy(updater.gameObject);
        }
        
        SceneManager.LoadScene("Opening");
    }

    public void Show(string dieReason, bool isPrologue = false) {
        
        Show("You Died!", dieReason, isPrologue);
    }

    public void Show(string title, string dieReason, bool isPrologue = false, bool showRestart = true) {
        transform.Find("Panel").gameObject.SetActive(true);
        transform.Find("Panel/DieReason").GetComponent<TMP_Text>().text = dieReason;
        transform.Find("Panel/DieText").GetComponent<TMP_Text>().text = title;
        restartButton.gameObject.SetActive(!isPrologue);
        restartTodayButton.gameObject.SetActive(!isPrologue && showRestart);
    }
    
    public void Hide() {
        transform.Find("Panel").gameObject.SetActive(false);
    }
    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
