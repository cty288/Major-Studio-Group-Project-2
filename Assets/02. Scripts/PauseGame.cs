using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework.Architecture;
using Crosstales.RTVoice;
using MikroFramework;
using MikroFramework.AudioKit;
using MikroFramework.TimeSystem;
using UnityEngine.SceneManagement;

public struct OnPauseAudioSet {
    
}

public struct OnPause {
    public bool isPause;
}
public class PauseGame : MonoBehaviour, ICanSendEvent {
    protected static bool isPause = false;

    public static bool IsPause => isPause;

    protected GameObject PausePanel;
    protected GameObject PauseBtn;
    [SerializeField] protected Sprite Spr_Pause;
    [SerializeField] protected Sprite Spr_Resume;

    protected Button continueButton;
    protected Button mainMenuButton;

    //Audio Setting
    protected GameObject audioPanel;
    protected Button audioSettings;
    protected Collider2D audioSettingsBaseCollider;
    protected Collider2D audioSettingsRangeCollider;

    protected Slider MasterSlider;
    protected Slider SFXSlider;
    protected Slider RadioSlider;
    protected Slider VoicesSlider;

    


    private void Awake() {
        PausePanel = transform.Find("Panel").gameObject;
        PauseBtn = transform.Find("PauseButton").gameObject;
        
        PauseBtn.GetComponent<Button>().onClick.AddListener(OnPauseButtonClicked);
        isPause = false;
        PausePanel.transform.Find("BG").GetComponent<Button>().onClick.AddListener(OnPausePanelClicked);
        continueButton = PausePanel.transform.Find("ContinueButton").GetComponent<Button>();
        mainMenuButton = PausePanel.transform.Find("QuitButton").GetComponent<Button>();
        
        continueButton.onClick.AddListener(OnContinueButtonClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);

        audioPanel = PausePanel.transform.Find("AudioPanel").gameObject;
        audioSettings = PausePanel.transform.Find("AudioButton").GetComponent<Button>();
        Collider2D[] colliders = audioSettings.GetComponents<Collider2D>();
        audioSettingsBaseCollider = colliders[0];
        audioSettingsRangeCollider = colliders[1];
        //audioSettings.onClick.AddListener(activateAudioPanel);

        //bars
        MasterSlider = audioPanel.transform.Find("Master").GetComponent<Slider>();
        SFXSlider = audioPanel.transform.Find("SFX").GetComponent<Slider>();
        RadioSlider = audioPanel.transform.Find("Radio").GetComponent<Slider>();
        VoicesSlider = audioPanel.transform.Find("Voices").GetComponent<Slider>();
        
        MasterSlider.onValueChanged.AddListener(OnMasterSliderValueChanged);
        SFXSlider.onValueChanged.AddListener(OnSFXSliderValueChanged);
        RadioSlider.onValueChanged.AddListener(OnRadioSliderValueChanged);
        VoicesSlider.onValueChanged.AddListener(OnVoicesSliderValueChanged);
        
    }



    private void Start()
    {
        audioPanel.SetActive(false);
        this.Delay(0.3f, () => {
            MasterSlider.SetValueWithoutNotify( AudioSystem.Singleton.MasterVolume);
            SFXSlider.SetValueWithoutNotify(AudioSystem.Singleton.SoundVolume);
            RadioSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Radio_Volume", 1f));
            VoicesSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Voices_Volume", 1f));
        });
        

    }
    
    private void OnMasterSliderValueChanged(float value) {
        AudioSystem.Singleton.MasterVolume = value;
    }
    
    private void OnSFXSliderValueChanged(float value) {
        AudioSystem.Singleton.SoundVolume = value;
    }
    
    private void OnRadioSliderValueChanged(float value) {
        PlayerPrefs.SetFloat("Radio_Volume", value);
       
    }
    
    private void OnVoicesSliderValueChanged(float value) {
        PlayerPrefs.SetFloat("Voices_Volume", value);
        
    }

    private void Update()
    {
        AudioSettingsPanelControl();
        //Read Slider Value
        /*
        Master_Volume = MasterSlider.value;
        SFX_Volume = SFXSlider.value;
        Radio_Volume = RadioSlider.value;
        Voices_Volume = VoicesSlider.value;*/
        //TO DO: Pass Them into System
    }

    private bool isMouseInAudioSettingsBaseColliderBefore = false;
    private void AudioSettingsPanelControl() {
        Vector2 mousePos = Input.mousePosition;
        
        bool isMouseInAudioSettingsBaseCollider = audioSettingsBaseCollider.OverlapPoint(mousePos);
        if (isMouseInAudioSettingsBaseCollider) {
            audioPanel.SetActive(true);
        }
        else {
            if (isMouseInAudioSettingsBaseColliderBefore) {
                bool isMouseInAudioSettingsRangeCollider = audioSettingsRangeCollider.OverlapPoint(mousePos);
                if (!isMouseInAudioSettingsRangeCollider) {
                    audioPanel.SetActive(false);
                }
                else {
                    audioPanel.SetActive(true);
                    return;
                }
            }
            else {
                audioPanel.SetActive(false);
            }
        }
        
        
        isMouseInAudioSettingsBaseColliderBefore = isMouseInAudioSettingsBaseCollider;
    }
    

    private void OnMainMenuButtonClicked() {
        BackToOpening();
    }
    
    protected void BackToOpening() {
        Time.timeScale = 1;
       DieCanvas.BackToMenu();
    }
    private void OnContinueButtonClicked() {
        Btn_Pause();
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
            this.SendEvent<OnPause>(new OnPause(){isPause = true});
            Time.timeScale = 0;
            PauseBtn.GetComponent<Image>().sprite = Spr_Resume;
            Crosstales.RTVoice.Speaker.Instance.Pause();
        }
        else
        {
            Time.timeScale = 1;
            PauseBtn.GetComponent<Image>().sprite = Spr_Pause;
            this.SendEvent<OnPauseAudioSet>();
            Crosstales.RTVoice.Speaker.Instance.UnPause();
            this.SendEvent<OnPause>(new OnPause(){isPause = false});
        }
        PausePanel.SetActive(isPause);
    }

    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
