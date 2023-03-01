using System;
using System.Collections;
using System.Collections.Generic;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.Audio;

public struct OnRadioEnd {

}

public struct OnRadioStart {
    public string speakContent;
    public float speakRate;
    public Gender speakGender;
    public AudioMixerGroup mixer;
}
public abstract class RadioEvent : GameEvent, ICanGetModel, ICanSendEvent {
    [field: ES3Serializable]
    public override GameEventType GameEventType { get; } = GameEventType.Radio_News;
    
    protected RadioModel radioModel;

    [ES3Serializable]
    protected string speakContent;
    [ES3Serializable]
    protected float speakRate;
    [ES3Serializable]
    protected Gender speakGender;
    [ES3Serializable]
    protected AudioMixerGroup mixer;
    [ES3Serializable]
    protected RadioChannel channel;

    protected bool started = false;
    protected bool ended = false;
    
    protected RadioEvent(TimeRange startTimeRange, string speakContent, float speakRate, Gender speakGender, AudioMixerGroup mixer,
        RadioChannel channel) : base(startTimeRange) {
        radioModel = this.GetModel<RadioModel>();
        gameStateModel = this.GetModel<GameStateModel>();
        
        this.speakContent = speakContent;
        this.speakRate = speakRate;
        this.speakGender = speakGender;
        this.mixer = mixer;
        this.channel = channel;
    }

    public RadioEvent(): base() {
        radioModel = this.GetModel<RadioModel>();
        gameStateModel = this.GetModel<GameStateModel>();
    }

    public override void OnStart() {
       radioModel.CurrentChannel.RegisterOnValueChaned(OnRadioChannelChanged);
    }

   

    public override EventState OnUpdate() {
        if (radioModel.CurrentChannel != channel && !started) {
            return EventState.Missed;
        }
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        if ((currentTime.Hour == 23 && currentTime.Minute >= 50) || gameStateModel.GameState.Value == GameState.End) {
            //EndRadio();
            //return EventState.End;
        }
        

        if (!started) {
            started = true;
            this.SendEvent<OnRadioStart>(new OnRadioStart() {
                speakContent = speakContent,
                speakRate = speakRate,
                speakGender = speakGender,
                mixer = mixer
            });
            OnRadioStart();
        }

        return (radioModel.IsSpeaking && !ended) ? EventState.Running : EventState.End;
    }

    protected abstract void OnRadioStart();

    protected void EndRadio() {
        ended = true;
        this.SendEvent<OnRadioEnd>();
        radioModel.CurrentChannel.UnRegisterOnValueChanged(OnRadioChannelChanged);
    }

    private void OnRadioChannelChanged(RadioChannel channel) {
        if(this.channel != channel) {
            EndRadio();
        }
    }
}


