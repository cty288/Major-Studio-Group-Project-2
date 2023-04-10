using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework.Architecture;
using Crosstales.RTVoice;

public class PauseGame : MonoBehaviour {
    protected static bool isPause = false;

    public static bool IsPause => isPause;

    protected GameObject PausePanel;
    protected GameObject PauseBtn;
    [SerializeField] protected Sprite Spr_Pause;
    [SerializeField] protected Sprite Spr_Resume;

    private void Awake() {
        PausePanel = transform.Find("Panel").gameObject;
        PauseBtn = transform.Find("PauseButton").gameObject;
        
        PauseBtn.GetComponent<Button>().onClick.AddListener(OnPauseButtonClicked);
        isPause = false;
        PausePanel.transform.Find("BG").GetComponent<Button>().onClick.AddListener(OnPausePanelClicked);
    }

    private void OnPausePanelClicked() {
        if (isPause) {
            Btn_Pause();
        }
    }

    private void OnPauseButtonClicked() {
        Btn_Pause();
    }
    
    

   

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

  
    public void Btn_Pause()
    {
        isPause = !isPause;
        if (isPause)
        {
            Time.timeScale = 0;
            PauseBtn.GetComponent<Image>().sprite = Spr_Resume;
            Crosstales.RTVoice.Speaker.Instance.Pause();
        }
        else
        {
            Time.timeScale = 1;
            PauseBtn.GetComponent<Image>().sprite = Spr_Pause;
            Crosstales.RTVoice.Speaker.Instance.UnPause();
        }
        PausePanel.SetActive(isPause);
    }
}
