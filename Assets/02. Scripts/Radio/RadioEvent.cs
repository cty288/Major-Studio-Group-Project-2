using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Electricity;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.TimeSystem;
using UnityEngine;
using UnityEngine.Audio;

struct OnRadioEnd {
    public RadioChannel channel;
}



public struct OnRadioProgramStart {
    public IRadioContent radioContent;
    public RadioChannel channel;
   // public RadioProgramType programType;
}
public abstract class RadioEvent<TRadioContent> : GameEvent, ICanGetModel, ICanSendEvent  where TRadioContent : IRadioContent {
    [field: ES3Serializable]
    public override GameEventType GameEventType { get; } = GameEventType.Radio;
    
    protected RadioModel radioModel;

    
    protected abstract TRadioContent GetRadioContent();
    protected abstract void SetRadioContent(TRadioContent radioContent);
    
   // [ES3Serializable]
    //protected RadioProgramType programType;
    
    [ES3Serializable]
    protected RadioChannel channel;

    protected bool started = false;
    protected bool ended = false;
    protected bool delayEnded = false;
    protected bool startDelayEnded = false;
    protected ElectricityModel electricityModel;

    [field: ES3Serializable]
    public override bool CanStartWithSameType { get; } = true;

    protected RadioEvent(TimeRange startTimeRange, TRadioContent radioContent,
        RadioChannel channel) : base(startTimeRange) {
        radioModel = this.GetModel<RadioModel>();
        gameStateModel = this.GetModel<GameStateModel>();
        electricityModel = this.GetModel<ElectricityModel>();

        SetRadioContent(radioContent);
       // this.programType = programType;
        this.channel = channel;
    }

    public RadioEvent(): base() {
        radioModel = this.GetModel<RadioModel>();
        gameStateModel = this.GetModel<GameStateModel>();
        electricityModel = this.GetModel<ElectricityModel>();
    }

    public override void OnStart() {
     //  radioModel.CurrentChannel.RegisterOnValueChaned(OnRadioChannelChanged);
       if (channel == RadioChannel.AllChannels) {
           channel = radioModel.CurrentChannel.Value;
       }
    }

   

    public override EventState OnUpdate() {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        if (((currentTime.Hour==23 && currentTime.Minute>=55))) {
            if (!started) {
                return EventState.Missed;
            }

            if (radioModel.CurrentChannel.Value != channel || !electricityModel.HasElectricity() || !radioModel.IsOn) {
                this.SendEvent<OnRadioEnd>(new OnRadioEnd() {
                    channel = channel
                });
                return EventState.End;
            }
        }

        if (radioModel.GetIsSpeaking(channel) && !started) {
            return EventState.Missed;
        }


        if (!started && (!electricityModel.HasElectricity() || !radioModel.IsOn ||
                         radioModel.CurrentChannel != channel)) {
            //still play the radio, but not the voice
            OnPlayedWhenRadioOff();
        }

        if (!started) {
            started = true;
            OnRadioStart();
            this.SendEvent<OnRadioProgramStart>(new OnRadioProgramStart() {
                radioContent = GetRadioContent(),
                channel = channel,
               // programType = programType
            });
        }

        if ((!radioModel.GetIsSpeaking(channel) || ended)&& !startDelayEnded) {
            startDelayEnded = true;
            delayEnded = false;
            this.GetSystem<ITimeSystem>().AddDelayTask(1f, () => {
                delayEnded = true;
            });
        } 
        
        if(delayEnded) {
            return EventState.End;
        }

        return EventState.Running;
    }

    protected abstract void OnRadioStart();
    
    protected abstract void OnPlayedWhenRadioOff();

    
}


