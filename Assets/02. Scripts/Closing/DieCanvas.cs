using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DieCanvas : MonoMikroSingleton<DieCanvas>, IController {
    private Button restartButton;

    private void Awake() {
        restartButton = transform.Find("Panel/RestartButton").GetComponent<Button>();
        restartButton.onClick.AddListener(OnRestartButtonClicked);
    }

    private void OnRestartButtonClicked() {
        (MainGame.Interface as MainGame).ClearSave();
        Architecture<MainGame>.ResetArchitecture();
        
        //MainGame.Interface.ResetArchitecture();
        SceneManager.LoadScene("Opening");
    }

    public void Show(string dieReason, bool isPrologue = false) {
        
        Show("You Died!", dieReason, isPrologue);
    }

    public void Show(string title, string dieReason, bool isPrologue = false) {
        transform.Find("Panel").gameObject.SetActive(true);
        transform.Find("Panel/DieReason").GetComponent<TMP_Text>().text = dieReason;
        transform.Find("Panel/DieText").GetComponent<TMP_Text>().text = title;
        restartButton.gameObject.SetActive(!isPrologue);
    }
    
    public void Hide() {
        transform.Find("Panel").gameObject.SetActive(false);
    }
    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
