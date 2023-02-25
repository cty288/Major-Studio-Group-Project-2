using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class RadioChannalRange {
    public RadioChannel channel;
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
    
    [SerializeField]
    private List<RadioChannalRange> radioChannalRanges = new List<RadioChannalRange>();
    private Slider channelSlider;
    private RadioChannelSwitch radioChannelSwitch;
    [SerializeField] private float rotateSpeed = 0.02f;
    
    protected TMP_Text channelNameText;
    
    
    
    protected override void Awake() {
        base.Awake();
        images = GetComponentsInChildren<Image>(true).ToList();
        texts = GetComponentsInChildren<TMP_Text>(true).ToList();
        panel = transform.Find("Panel");
        radioModel = this.GetModel<RadioModel>();
        channelSlider = panel.Find("Radio/Slider").GetComponent<Slider>();
        radioChannelSwitch = panel.Find("Radio/RadioChannelSwitch").GetComponent<RadioChannelSwitch>();
        channelNameText = panel.Find("Radio/ChannelNameText").GetComponent<TMP_Text>();
        
        radioModel.InitializeChannelRanges(radioChannalRanges);
        
        
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

    private string GetChannelNameByChannel(RadioChannel channel) {
        switch (channel) {
            case RadioChannel.DeadNews:
                return "Dead Body Reports";
            case RadioChannel.GeneralNews:
                return "Daily News";
            case RadioChannel.None:
                return "";
        }

        return "";
    }

    private void OnRadioChannelChanged(RadioChannel channel) {
        string channelName = GetChannelNameByChannel(channel);
        channelNameText.DOKill();
        if(String.IsNullOrEmpty(channelName)) {
            channelNameText.DOFade(0, 0.5f);
        }
        else {
            channelNameText.text = "Current Channel: " + channelName;
            channelNameText.DOFade(1, 1f);
            channelNameText.DOFade(0, 1f).SetDelay(4f);
        }
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

    public override void OnShow(float time) {
        panel.gameObject.SetActive(true);
        images.ForEach((image => image.DOFade(imageAlpha[image], time)));
        texts.ForEach((text => text.DOFade(1, time)));
        string channelName = GetChannelNameByChannel(radioModel.CurrentChannel.Value);
        if (String.IsNullOrEmpty(channelName)) {
            channelNameText.text = "Channel Not Available";
        }
        else {
            channelNameText.text = "Current Channel: " + channelName;
        }
       
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
