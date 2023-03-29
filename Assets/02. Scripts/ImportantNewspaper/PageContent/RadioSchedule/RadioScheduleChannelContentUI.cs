using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Radio.RadioScheduling;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;

public class RadioScheduleChannelContentUI : AbstractMikroController<MainGame> {
    [SerializeField] private GameObject programContentPrefab;
    
    private TMP_Text channelNameText;
    private Transform programContentParent;
    private RadioModel radioModel;

    private void Awake() {
        channelNameText = transform.Find("ChannelName").GetComponent<TMP_Text>();
        programContentParent = transform.Find("ProgramList");
        radioModel = this.GetModel<RadioModel>();
    }

    public void SetContent(List<RadioScheduleInfo> channelContent, RadioChannel radioChannel, int index) {
        Awake();
        string channelName = radioModel.GetRadioChannelInfo(radioChannel).ChannelName;
        channelNameText.text = $"{index}. {channelName}:";

        foreach (RadioScheduleInfo scheduleInfo in channelContent) {
            RadioScheduleProgramContentUI programContentUI = Instantiate(programContentPrefab, programContentParent)
                .GetComponent<RadioScheduleProgramContentUI>();
            programContentUI.SetContent(scheduleInfo);
        }
    }
}
