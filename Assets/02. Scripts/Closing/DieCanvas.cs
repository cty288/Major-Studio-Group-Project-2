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
    private Image BG;
    private Image endingBG;
    [SerializeField]
    private List<EndingAnimation> endingAnimations;
    private int endingIndex;
    private TMP_Text dieReasonText;
    private Animator animator;
    private void Awake() {
        //restartButton = transform.Find("Panel/OptionGroup/RestartButton").GetComponent<Button>();
        restartTodayButton = transform.Find("Panel/OptionGroup/RestartFromToday").GetComponent<Button>();
        backToMenuButton = transform.Find("Panel/OptionGroup/BackToMenu").GetComponent<Button>();
       // restartButton.onClick.AddListener(OnRestartButtonClicked);
        restartTodayButton.onClick.AddListener(OnRestartTodayButtonClicked);
        backToMenuButton.onClick.AddListener(OnBackToMenuButtonClicked);
        BG = transform.Find("Panel/BG").GetComponent<Image>();
        endingBG = transform.Find("Panel/EndingBG").GetComponent<Image>();
        animator = transform.Find("Panel").GetComponent<Animator>();
        dieReasonText= transform.Find("Panel/DieReason").GetComponent<TMP_Text>();
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
        endingIndex = endingAnimIndex;
        transform.Find("Panel").gameObject.SetActive(true);
        dieReasonText.text = dieReason;
        transform.Find("Panel/DieText").GetComponent<TMP_Text>().text = title;
        //restartButton.gameObject.SetActive(!isPrologue);
        backToMenuButton.gameObject.SetActive(!isPrologue);
        restartTodayButton.gameObject.SetActive(!isPrologue && showRestart);
        endingBG.gameObject.SetActive(endingAnimIndex >= 0 && !isPrologue);
        endingBG.color = Color.white;
        if (endingAnimIndex == 0)
        {
            BG.color = Color.white;
            dieReasonText.color = Color.black;
        }
        else
        {
            BG.color = Color.black;
            dieReasonText.color = Color.white;
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
                    PlayBGAnim(endingAnimation, startIndex + 1);
                }
                if ((startIndex == endingAnimation.sprites.Count - 1)&& endingIndex!=0)
                {
                    endingBG.DOColor(new Color(0.4F, 0, 0, 1), 0.5f);
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
