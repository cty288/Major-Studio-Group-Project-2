using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class MindfulEating : RadioEvent
{


    private Coroutine radioCorruptCheckCoroutine;
    public MindfulEating(TimeRange startTimeRange, RadioMessage message) :
        base(startTimeRange, message.Content, message.SpeakSpeed, message.Gender, AudioMixerList.Singleton.AudioMixerGroups[message.MixerIndex], RadioChannel.FoodNews)
    {
        channel = message.Channel;
    }

    public MindfulEating() : base() { }
    [field: ES3Serializable]
    public override float TriggerChance { get; } = 1;
    public override void OnEnd()
    {
        AddNextFoodInfo();
    }

    public override void OnMissed()
    {
        AddNextFoodInfo();
    }

    private void AddNextFoodInfo()
    {
        DateTime nextTime = gameTimeManager.CurrentTime.Value.AddDays(1);
        nextTime = new DateTime(nextTime.Year, nextTime.Day, 22);
        gameEventSystem.AddEvent(new MindfulEating(new TimeRange(nextTime), RadioRandomFood.Singleton.GetNextRandomFoodRadio()));
    }

    protected override void OnRadioStart()
    {
        //radioCorruptCheckCoroutine = CoroutineRunner.Singleton.StartCoroutine(RadioCorruptCheck());
    }




}




