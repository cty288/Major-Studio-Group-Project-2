using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Radio;
using _02._Scripts.Radio.RadioScheduling;
using Crosstales.RTVoice.Model.Enum;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class MusicRadio : ScheduledRadioEvent<RadioMusicContent> {
    [field: ES3Serializable]
    protected override bool DayEndAfterFinish { get; set; } = false;
    
    public MusicRadio(TimeRange startTimeRange, int musicIndex) :
        base(startTimeRange, new RadioMusicContent(musicIndex), RadioChannel.FM104 ) {
        
    }

    public MusicRadio() : base() {
       // if (radioContent.mixer == null) {
            //this.radioContent.mixer = AudioMixerList.Singleton.AudioMixerGroups[1];
        //}
        //}
    }

    [field: SerializeField] protected override RadioProgramType ProgramType { get; set; } = RadioProgramType.Music;

    [field: ES3Serializable]
    public override float TriggerChance { get; } = 1f;
   

    protected override ScheduledRadioEvent<RadioMusicContent> OnGetNextRadioProgramMessage(TimeRange nextTimeRange, bool playSuccess) {
        return new MusicRadio(nextTimeRange, RadioContentPlayerFactory.Singleton.GetMusicSourceIndex());
    }

  

    [field: ES3Serializable]
    protected RadioMusicContent radioContent { get; set; }

    protected override RadioMusicContent GetRadioContent() {
        return radioContent;
    }
    protected override void SetRadioContent(RadioMusicContent radioContent) {
        this.radioContent = radioContent;
    }

    protected override void OnRadioStart() {
        
    }

    protected override void OnPlayedWhenRadioOff() {
        
    }
}
