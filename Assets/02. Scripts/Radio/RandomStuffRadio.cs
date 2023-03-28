using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Radio.RadioScheduling;
using Crosstales.RTVoice.Model.Enum;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class RandomStuffRadio : ScheduledRadioEvent<RadioTextContent> {
    [field: ES3Serializable]
    protected override bool DayEndAfterFinish { get; set; } = true;
    
    
    public RandomStuffRadio(TimeRange startTimeRange, RadioTextMessageInfo textMessageInfo) :
        base(startTimeRange, new RadioTextContent(textMessageInfo.Content, textMessageInfo.SpeakSpeed, textMessageInfo.Gender, AudioMixerList.Singleton.AudioMixerGroups[textMessageInfo.MixerIndex]), RadioChannel.FM100 ) {
        if (radioContent.mixer == null) {
            radioContent.mixer = AudioMixerList.Singleton.AudioMixerGroups[1];
        }

        ProgramType = textMessageInfo.ProgramType;
        channel = textMessageInfo.Channel;
    }

    public RandomStuffRadio() : base() {
       // if (radioContent.mixer == null) {
            //this.radioContent.mixer = AudioMixerList.Singleton.AudioMixerGroups[1];
        //}
        //}
    }

    [field: SerializeField]
    protected override RadioProgramType ProgramType { get; set; }

    [field: ES3Serializable]
    public override float TriggerChance { get; } = 1f;
   

    protected override ScheduledRadioEvent<RadioTextContent> OnGetNextRadioProgramMessage(TimeRange nextTimeRange, bool playSuccess) {
        return new RandomStuffRadio(nextTimeRange, RadioRandomStuff.Singleton.GetNextRandomRadio(ProgramType));
    }

  

    [field: ES3Serializable]
    protected RadioTextContent radioContent { get; set; }

    protected override RadioTextContent GetRadioContent() {
        return radioContent;
    }
    protected override void SetRadioContent(RadioTextContent radioContent) {
        this.radioContent = radioContent;
    }

    protected override void OnRadioStart() {
        
    }

    protected override void OnPlayedWhenRadioOff() {
        
    }
}
