using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Singletons;
using MikroFramework.TimeSystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public struct EndingAnimation {
    public List<Sprite> sprites;
}

public class DieCanvas : MonoMikroSingleton<DieCanvas>, IController {
    //private Button restartButton;
    private Button backToMenuButton;
    private Button restartTodayButton;
    private Button statsButton;
    private Image endingBG;
    [SerializeField]
    private List<EndingAnimation> endingAnimations;
    private StatsPanel statsPanel;
    private Image bgStrip;

    private Animator animator;
    private void Awake() {
        //restartButton = transform.Find("Panel/OptionGroup/RestartButton").GetComponent<Button>();
        restartTodayButton = transform.Find("Panel/OptionGroup/RestartFromToday").GetComponent<Button>();
        backToMenuButton = transform.Find("Panel/OptionGroup/BackToMenu").GetComponent<Button>();
       // restartButton.onClick.AddListener(OnRestartButtonClicked);
        restartTodayButton.onClick.AddListener(OnRestartTodayButtonClicked);
        backToMenuButton.onClick.AddListener(OnBackToMenuButtonClicked);
        endingBG = transform.Find("Panel/EndingBG").GetComponent<Image>();
        animator = transform.Find("Panel").GetComponent<Animator>();
        bgStrip = transform.Find("Panel/BGStrip").GetComponent<Image>();
        statsButton = transform.Find("Panel/OptionGroup/Stats").GetComponent<Button>();
        statsButton.onClick.AddListener(OnStatsButtonClicked);
        statsPanel = transform.Find("Panel/Panel_Stats").GetComponent<StatsPanel>();
    }

    private void OnStatsButtonClicked() {
        statsPanel.TurnOnStatsPanel();
    }


    private void OnBackToMenuButtonClicked() {
        BackToMenu();
    }

    private void OnRestartTodayButtonClicked() {
        BackToOpening();
    }

    private void OnRestartButtonClicked() {
        MainGame.ClearSave();
        BackToOpening();
    }

    public static void BackToOpening() {
        Architecture<MainGame>.ResetArchitecture();
        
        //MainGame.Interface.ResetArchitecture();
        DeleteUpdaters();

        SceneManager.LoadScene("Opening");
    }

    private static void DeleteUpdaters() {
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
    }
    public static void BackToMenu() {
        Architecture<MainGame>.ResetArchitecture();
        
        //MainGame.Interface.ResetArchitecture();
        DeleteUpdaters();
        
        SceneManager.LoadScene("Menu");
    }

    public void Show(string dieReason, int endingAnimIndex, bool isPrologue = false) {
        
        Show("You Died!", dieReason, endingAnimIndex, isPrologue);
    }

    public void Show(string title, string dieReason, int endingAnimIndex, bool isPrologue = false, bool showRestart = true) {
        Awake();
        animator.enabled = true;
        transform.Find("Panel").gameObject.SetActive(true);
        transform.Find("Panel/DieReason").GetComponent<TMP_Text>().text = dieReason;
        transform.Find("Panel/DieText").GetComponent<TMP_Text>().text = title;
        //restartButton.gameObject.SetActive(!isPrologue);
        statsButton.gameObject.SetActive(!isPrologue);
        backToMenuButton.gameObject.SetActive(!isPrologue);
        restartTodayButton.gameObject.SetActive(!isPrologue && showRestart);
        endingBG.gameObject.SetActive(endingAnimIndex >= 0 && !isPrologue);
        if (!isPrologue) {
            bgStrip.DOFade(1f, 1f);
        }
        this.Delay(1f, () => {
            animator.enabled = false;
        });
        if (endingAnimIndex >= 0 && !isPrologue) {
            EndingAnimation endingAnimation = endingAnimations[endingAnimIndex];
            endingBG.sprite = endingAnimation.sprites[0];
            this.Delay(2f, () => {
                PlayBGAnim(endingAnimation, 1);
            });
           // StartCoroutine(Fade());

        }
    }



    private void PlayBGAnim(EndingAnimation endingAnimation, int startIndex) {
        if(startIndex >= endingAnimation.sprites.Count) {
            return;
        }
        endingBG.DOFade(0, 0.5f).OnComplete(() => {
            endingBG.sprite = endingAnimation.sprites[startIndex];
            endingBG.DOFade(1, 0.5f).OnComplete(() => {
                if (startIndex < endingAnimation.sprites.Count - 1) {
                    this.Delay(1f, () => {
                        PlayBGAnim(endingAnimation, startIndex + 1);
                    });
                }
            });
        });
    }

    public void Hide() {
        transform.Find("Panel").gameObject.SetActive(false);
    }
    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
