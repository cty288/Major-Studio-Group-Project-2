using Remotion.Linq.Clauses;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class OptionsPanel : MonoBehaviour
{
    protected static bool optOn = false;
    public static bool OptOn=> optOn;

    protected GameObject optionsPanel;
    protected GameObject optionsBtn;
    protected GameObject audioPanel;

    protected Button screenSize;
    protected Button audioSettings;
    protected Button exitToMenu;
    protected Button resumeBtn;

    protected Slider MasterSlider;
    protected Slider SFXSlider;
    protected Slider RadioSlider;
    protected Slider VoicesSlider;

    protected bool isFullScreen = false;
    protected TextMeshProUGUI Text_isFullScreen;

    [SerializeField] public float Master_Volume;
    [SerializeField] public float SFX_Volume;
    [SerializeField] public float Radio_Volume;
    [SerializeField] public float Voices_Volume;

    //[SerializeField] protected bool AudioPandlOn = false;

    private void Awake()
    {
        //Find obj
        optionsPanel = transform.Find("Panel").gameObject;
        optionsBtn = transform.Find("OptionButton").gameObject;
        audioPanel = optionsPanel.transform.Find("AudioPanel").gameObject;

        //find buttons
        GameObject btn = optionsPanel.transform.Find("Btn").gameObject;
        screenSize = btn.transform.Find("Btn_ScreenSize").GetComponent<Button>();
        audioSettings = btn.transform.Find("Btn_Audio").GetComponent<Button>();
        exitToMenu = btn.transform.Find("Btn_Exit").GetComponent<Button>();
        resumeBtn = btn.transform.Find("Btn_Resume").GetComponent<Button>();

        //add On click event
        optionsBtn.GetComponent<Button>().onClick.AddListener(Btn_Options);
        screenSize.onClick.AddListener(fullScreen);
        audioSettings.onClick.AddListener(activateAudioPanel);
        exitToMenu.onClick.AddListener(BackToOpening);
        resumeBtn.onClick.AddListener(Resume);

        //bars
        MasterSlider = audioPanel.transform.Find("Master").GetComponent<Slider>();
        SFXSlider = audioPanel.transform.Find("SFX").GetComponent<Slider>();
        RadioSlider = audioPanel.transform.Find("Radio").GetComponent<Slider>();
        VoicesSlider = audioPanel.transform.Find("Voices").GetComponent<Slider>();

        //Text
        Text_isFullScreen = btn.transform.Find("Btn_ScreenSize").gameObject.transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        audioPanel.SetActive(false);
    }

    private void Update()
    {
        //Read Slider Value
        Master_Volume = MasterSlider.value;
        SFX_Volume= SFXSlider.value;
        Radio_Volume = RadioSlider.value;
        Voices_Volume = VoicesSlider.value;
        //TO DO: Pass Them into System
    }
    private void Btn_Options()
    {
        optOn = !optOn;
        optionsPanel.SetActive(optOn);
    }

    private void Resume()
    {
        optOn = false;
        optionsPanel.SetActive(optOn);
    }
    private void fullScreen()
    {
        audioPanel.SetActive(false);
        isFullScreen = !isFullScreen;
        if(isFullScreen)
        {
            Text_isFullScreen.SetText("Full Screen: ON");
            Screen.fullScreen = true;
        }
        else
        {
            Text_isFullScreen.SetText("Full Screen: OFF");
            Screen.fullScreen = false;
        }

    }
    private void activateAudioPanel()
    {
        if (audioPanel.activeSelf)
        {
            audioPanel.SetActive(false);
        }
        else
        {
            audioPanel.SetActive(true);
        }
    }
    private void BackToOpening()
    {
        audioPanel.SetActive(false);
        DieCanvas.BackToMenu();
    }
}
