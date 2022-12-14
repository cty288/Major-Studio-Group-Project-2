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
    public override GameEventType GameEventType { get; } = GameEventType.Radio;
    
    protected RadioModel radioModel;

    protected string speakContent;
    protected float speakRate;
    protected Gender speakGender;
    protected AudioMixerGroup mixer;

    protected bool started = false;
    protected RadioEvent(TimeRange startTimeRange, string speakContent, float speakRate, Gender speakGender, AudioMixerGroup mixer) : base(startTimeRange) {
        radioModel = this.GetModel<RadioModel>();
        gameStateModel = this.GetModel<GameStateModel>();
        this.speakContent = speakContent;
        this.speakRate = speakRate;
        this.speakGender = speakGender;
        this.mixer = mixer;
    }

    public override void OnStart() {
       
    }

    public override EventState OnUpdate() {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        if ((currentTime.Hour == 23 && currentTime.Minute >= 50) || gameStateModel.GameState.Value == GameState.End) {
            EndRadio();
            return EventState.End;
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

        return radioModel.IsSpeaking ? EventState.Running : EventState.End;
    }

    protected abstract void OnRadioStart();

    protected void EndRadio() {
        this.SendEvent<OnRadioEnd>();
    }
}


