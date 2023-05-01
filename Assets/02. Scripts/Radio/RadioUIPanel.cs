using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.Electricity;
using _02._Scripts.Notebook;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using NHibernate.Mapping;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class RadioChannalRange {
    public float start;
    public float end;
}

public class RadioUIPanel : OpenableUIPanel
{
    private List<Image> images;
    private List<TMP_Text> texts;
    private Dictionary<Image, float> imageAlpha = new Dictionary<Image, float>();
    private Transform panel;
    
    private RadioModel radioModel;
    
    //[SerializeField]
    //private List<RadioChannalRange> radioChannalRanges = new List<RadioChannalRange>();

    [SerializeField] private List<RadioChannelInfo> radioChannelInfos = new List<RadioChannelInfo>();
    
    
    private Slider channelSlider;
    private RadioChannelSwitch radioChannelSwitch;
    [SerializeField] private float rotateSpeed = 0.02f;
    
    protected TMP_Text channelNameText;
    protected Button radioOffButton;
    protected TMP_Text radioOffButtonHint;
    protected ElectricityModel electricityModel;
    
    
    
    
    protected override void Awake() {
        base.Awake();
        images = GetComponentsInChildren<Image>(true).ToList();
        texts = GetComponentsInChildren<TMP_Text>(true).ToList();
        panel = transform.Find("Panel");
        radioModel = this.GetModel<RadioModel>();
        channelSlider = panel.Find("Radio/Slider").GetComponent<Slider>();
        radioChannelSwitch = panel.Find("Radio/RadioChannelSwitch").GetComponent<RadioChannelSwitch>();
        channelNameText = panel.Find("Radio/ChannelNameText").GetComponent<TMP_Text>();
        radioOffButton = panel.Find("Radio/RadioOffButton").GetComponent<Button>();
        radioOffButtonHint = radioOffButton.transform.Find("Hint").GetComponent<TMP_Text>();
        radioModel.InitializeChannelInfos(radioChannelInfos);
        electricityModel = this.GetModel<ElectricityModel>();
        radioModel.IsOn.RegisterWithInitValue(OnRadioIsOnChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
        
        this.RegisterEvent<OnRadioUnlocked>(OnRadioUnlocked).UnRegisterWhenGameObjectDestroyed(gameObject);
        radioOffButton.onClick.AddListener(OnRadioOffButtonClick);
        foreach (var image in images) {
            imageAlpha.Add(image, image.color.a);
        }
        Hide(0.5f);

        channelSlider.value = radioModel.CurrentProgress.Value;
        radioChannelSwitch.OnSwitch += OnSwitchRotate;

        radioModel.CurrentProgress.RegisterOnValueChaned(OnRadioProgressChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        
        radioModel.CurrentChannel.RegisterOnValueChaned(OnRadioChannelChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnRadioUnlocked(OnRadioUnlocked obj) {
        UpdateChannelNameText();
    }

    private void OnRadioOffButtonClick() {
        if (radioModel.IsOn) {
            radioModel.IsOn.Value = false;
        }
        else {
            radioModel.IsOn.Value = true;
        }

        AudioSystem.Singleton.Play2DSound("radio_button");
    }

    private void OnRadioIsOnChanged(bool isOn) {
        if(isOn){
            radioOffButtonHint.text = "Turn Off";
        }else{
            radioOffButtonHint.text = "Turn On";
        }

        UpdateChannelNameText(true);
    }

    private string GetChannelNameByChannel(RadioChannel channel) {
        RadioChannelInfo ch = radioModel.GetRadioChannelInfo(channel);
        if(ch == null){
            return "";
        }
        return ch.ChannelName;
    }

    private void OnRadioChannelChanged(RadioChannel channel) {
        string channelName = GetChannelNameByChannel(channel);
        channelNameText.DOKill();
        if(String.IsNullOrEmpty(channelName)) {
           // channelNameText.DOFade(0, 0.5f);
        }
        //else {
            UpdateChannelNameText(true);
        //}
    }

    private void OnRadioProgressChanged(float progress) {
        channelSlider.value = progress;
    }

    private void OnSwitchRotate(float rotate) {
        radioModel.ChangeProgress(rotateSpeed * rotate);
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        radioChannelSwitch.OnSwitch -= OnSwitchRotate;
    }

    public void UpdateChannelNameText(bool alsoAppear = false) {
        if (radioModel.IsOn && electricityModel.HasElectricity()) {
            string channelName = GetChannelNameByChannel(radioModel.CurrentChannel.Value);
            if (String.IsNullOrEmpty(channelName)) {
                channelNameText.text = "Channel Not Available";
            }
            else {
                channelNameText.text = channelName;
            }
        }else {
            if (!electricityModel.HasElectricity()) {
                channelNameText.text = "No Electricity";
            }
            else {
                channelNameText.text = "The Radio is Off";
            }
        }

        if (alsoAppear) {
            channelNameText.DOFade(1, 1f);
            channelNameText.DOFade(0, 1f).SetDelay(4f);
        }
    }

    public override void OnShow(float time) {
        panel.gameObject.SetActive(true);
        images.ForEach((image => {
            if(imageAlpha.ContainsKey(image)) {
                image.DOFade(imageAlpha[image], time);
            }
            else {
                image.DOFade(1, time);
            }
           
        }));
        texts.ForEach((text => text.DOFade(1, time)));
        
        
        UpdateChannelNameText();
        
       
        channelNameText.DOFade(0, 1f).SetDelay(3f);
    }

    public override void OnHide(float time) {
        channelNameText.DOKill();
        images.ForEach((image => image.DOFade(0, time)));
        texts.ForEach((text => text.DOFade(0, time)));
        this.Delay(time, () => {
            if (this) { {
                    panel.gameObject.SetActive(false);
                }
            }
        });
    }

    public override void OnDayEnd() {
        Hide(0.5f);
    }


   
}
