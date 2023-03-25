using System;
using System.Collections;
using System.Collections.Generic;
using Crosstales.RTVoice.Model.Enum;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class RandomStuffRadio : RadioEvent<RadioTextContent> {
    public RandomStuffRadio(TimeRange startTimeRange, RadioTextMessageInfo textMessageInfo) :
        base(startTimeRange, new RadioTextContent( textMessageInfo.Content, textMessageInfo.SpeakSpeed, textMessageInfo.Gender, AudioMixerList.Singleton.AudioMixerGroups[textMessageInfo.MixerIndex]), RadioChannel.FM100 ) {
        if (radioContent.mixer == null) {
            radioContent.mixer = AudioMixerList.Singleton.AudioMixerGroups[1];
        }

        channel = textMessageInfo.Channel;
    }

    public RandomStuffRadio() : base() {
        if (radioContent.mixer == null) {
            this.radioContent.mixer = AudioMixerList.Singleton.AudioMixerGroups[1];
        }
    }
    [field: ES3Serializable]
    public override float TriggerChance { get; } = 0.8f;
    public override void OnEnd() {
        End();
    }

    public override void OnMissed() {
        End();
    }

    public void End() {
        DateTime currentTime = gameTimeManager.CurrentTime;
        int timeInterval = RadioRandomStuff.Singleton.RandomRadioAverageTimeInterval;
        int t = (int) (timeInterval * 0.1f);
        DateTime nextTime = currentTime.AddMinutes(Random.Range(timeInterval - t, timeInterval + t));
        gameEventSystem.AddEvent(new RandomStuffRadio(new TimeRange(nextTime), RadioRandomStuff.Singleton.GetNextRandomRadio()));
    }

    [field: ES3Serializable]
    protected override RadioTextContent radioContent { get; set; }

    protected override void OnRadioStart() {
       
    }

    protected override void OnPlayedWhenRadioOff() {
        
    }
}
