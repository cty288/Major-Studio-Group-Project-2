using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework.Architecture;
using Crosstales.RTVoice;
using MikroFramework.TimeSystem;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;
using TMPro;

public class MainMenuSetting : MonoBehaviour {
    protected static bool isPause = false;

    public static bool IsPause => isPause;

    protected GameObject PausePanel;
    protected GameObject PauseBtn;

    protected Button continueButton;
    protected Button screenButton;

    //Audio Setting
    protected GameObject audioPanel;
    protected Button audioSettings;

    protected Slider MasterSlider;
    protected Slider SFXSlider;
    protected Slider RadioSlider;
    protected Slider VoicesSlider;

    [SerializeField] public float Master_Volume;
    [SerializeField] public float SFX_Volume;
    [SerializeField] public float Radio_Volume;
    [SerializeField] public float Voices_Volume;

    //Screen Size Setting
    protected GameObject ScreenSizeDropdown;
    public TMP_Dropdown ScreenSizeSelections;
    [SerializeField] public int Screen_Type=0;
    protected Button fullScreen;
    protected TMP_Text fullScreenText;
    protected int isFullScreen = 0;


    private void Awake() {
        PausePanel = transform.Find("Panel").gameObject;
        PauseBtn = GameObject.Find("SettingsGameButton").gameObject;
        ScreenSizeDropdown = PausePanel.transform.Find("ScreenSizeDrop").gameObject;
        ScreenSizeSelections = ScreenSizeDropdown.GetComponent<TMP_Dropdown>();

        PauseBtn.GetComponent<Button>().onClick.AddListener(OnPauseButtonClicked);
        isPause = false;
        PausePanel.transform.Find("BG").GetComponent<Button>().onClick.AddListener(OnPausePanelClicked);
        continueButton = PausePanel.transform.Find("ContinueButton").GetComponent<Button>();
        screenButton = PausePanel.transform.Find("ScreenButton").GetComponent<Button>();
        fullScreen = ScreenSizeDropdown.transform.Find("Btn_FullScreen").GetComponent<Button>();
        fullScreenText = ScreenSizeDropdown.transform.Find("Btn_FullScreen").transform.Find("Text (TMP)").GetComponent<TMP_Text>();


        continueButton.onClick.AddListener(OnContinueButtonClicked);
        screenButton.onClick.AddListener(ScreenSize);
        fullScreen.onClick.AddListener(FullScreen);

        audioPanel = PausePanel.transform.Find("AudioPanel").gameObject;
        audioSettings = PausePanel.transform.Find("AudioButton").GetComponent<Button>();
        audioSettings.onClick.AddListener(activateAudioPanel);

        //bars
        MasterSlider = audioPanel.transform.Find("Master").GetComponent<Slider>();
        SFXSlider = audioPanel.transform.Find("SFX").GetComponent<Slider>();
        RadioSlider = audioPanel.transform.Find("Radio").GetComponent<Slider>();
        VoicesSlider = audioPanel.transform.Find("Voices").GetComponent<Slider>();

        //on value change
        MasterSlider.onValueChanged.AddListener(delegate { SaveMaster(); });
        SFXSlider.onValueChanged.AddListener(delegate { SaveSFX(); });
        RadioSlider.onValueChanged.AddListener(delegate { SaveRadio(); });
        VoicesSlider.onValueChanged.AddListener(delegate { SaveVoice(); });

        //Add Screensize Selections
        addScreenSelections();
    }

    private void Start()
    {
        audioPanel.SetActive(false);
        ScreenSizeDropdown.SetActive(false);

        //read audio value
        if (PlayerPrefs.HasKey("Master_Volume")) { Master_Volume = PlayerPrefs.GetFloat("Master_Volume"); }
        if (PlayerPrefs.HasKey("SFX_Volume")) { Master_Volume = PlayerPrefs.GetFloat("SFX_Volume"); }
        if (PlayerPrefs.HasKey("Radio_Volume")) { Master_Volume = PlayerPrefs.GetFloat("Radio_Volume"); }
        if (PlayerPrefs.HasKey("Voices_Volume")) { Master_Volume = PlayerPrefs.GetFloat("Voices_Volume"); }
        if (PlayerPrefs.HasKey("Voices_Volume")) { Master_Volume = PlayerPrefs.GetFloat("Voices_Volume"); }
        //Screen Size
        if (PlayerPrefs.HasKey("isFullScreen")) { Screen_Type = PlayerPrefs.GetInt("isFullScreen"); }
        ScreenSizeSelections.value = Screen_Type;
        setFullScreen();
        ScreenSizeSelections.RefreshShownValue();
    }

    private void Update()
    {
        //Read Slider Value
        Master_Volume = MasterSlider.value;
        SFX_Volume = SFXSlider.value;
        Radio_Volume = RadioSlider.value;
        Voices_Volume = VoicesSlider.value;
    }

    //transfer data to player prefab
    void SaveMaster()
    {
        PlayerPrefs.SetFloat("Master_Volume", Master_Volume);
        PlayerPrefs.Save();
    }

    void SaveSFX()
    {
        PlayerPrefs.SetFloat("SFX_Volume", SFX_Volume);
        PlayerPrefs.Save();
    }

    void SaveRadio()
    {
        PlayerPrefs.SetFloat("Radio_Volume", Radio_Volume);
        PlayerPrefs.Save();
    }
    void SaveVoice()
    {
        PlayerPrefs.SetFloat("Voices_Volume", Voices_Volume);
        PlayerPrefs.Save();
    }

    private void activateAudioPanel()
    {
        ScreenSizeDropdown.SetActive(false);
        if (audioPanel.activeSelf)
        {
            audioPanel.SetActive(false);
        }
        else
        {
            audioPanel.SetActive(true);
        }
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

  
    public void Btn_Pause()
    {
        isPause = !isPause;
        if (isPause)
        {
            Time.timeScale = 0;
            Crosstales.RTVoice.Speaker.Instance.Pause();
        }
        else
        {
            Time.timeScale = 1;
            Crosstales.RTVoice.Speaker.Instance.UnPause();
        }
        PausePanel.SetActive(isPause);
    }

    void ScreenSize()
    {
        audioPanel.SetActive(false);
        if (ScreenSizeDropdown.activeSelf)
        {
            ScreenSizeDropdown.SetActive(false);
        }
        else
        {
            ScreenSizeDropdown.SetActive(true);
        }
    }

    void addScreenSelections()
    {
        ScreenSizeSelections.onValueChanged.AddListener(delegate { changeScreenSize(); });
        ScreenSizeSelections.options.Add(new TMP_Dropdown.OptionData("1366x768"));
        ScreenSizeSelections.options.Add(new TMP_Dropdown.OptionData("1600x900"));
        ScreenSizeSelections.options.Add(new TMP_Dropdown.OptionData("1920x1080"));

        ScreenSizeSelections.value = 0;
        ScreenSizeSelections.RefreshShownValue();
        Debug.Log("Wait");
    }

    void changeScreenSize()
    {
        Screen_Type = ScreenSizeSelections.value;
        switch (ScreenSizeSelections.value)
        {
            case 0:
                // 1366x768
                Screen.SetResolution(1366, 768, false);
                break;
            case 1:
                // 1600x900
                Screen.SetResolution(1600, 900, false);
                break;
            case 2:
                // 1920x1080
                Screen.SetResolution(1920, 1080, false);
                break;

        }

        PlayerPrefs.SetInt("Screen_Type", Screen_Type);
        PlayerPrefs.Save();
    }

    void FullScreen()
    {
        if (isFullScreen == 0)
        {
            isFullScreen = 1;
            fullScreenText.text = "Full Screen: ON";
            Screen.fullScreen = true;
        }
        else
        {
            isFullScreen = 0;
            fullScreenText.text = "Full Screen: OFF";
            Screen.fullScreen = false;
        }
        PlayerPrefs.SetInt("Screen_Type", Screen_Type);
        PlayerPrefs.Save();
    }

    void setFullScreen()
    {
        if (isFullScreen == 1)
        {
            fullScreenText.text = "Full Screen: ON";
            Screen.fullScreen = true;
        }
        else
        {
            fullScreenText.text = "Full Screen: OFF";
            Screen.fullScreen = false;
        }
    }
}
