using System;
using System.Collections;
using System.Collections.Generic;
using Crosstales.RTVoice.Model.Enum;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class RandomStuffRadio : RadioEvent {
    public RandomStuffRadio(TimeRange startTimeRange, RadioMessage message) : base(startTimeRange, message.Content, message.SpeakSpeed, message.Gender, AudioMixerList.Singleton.AudioMixerGroups[message.MixerIndex]) {
        if (mixer == null) {
            this.mixer = AudioMixerList.Singleton.AudioMixerGroups[1];
        }
    }


    public override float TriggerChance { get; } = 0.4f;
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

    protected override void OnRadioStart() {
       
    }
}
