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


public class DeadBodyRadioIntroEvent : RadioEvent<RadioTextContent> {
    
    [field: ES3Serializable]
    protected override RadioTextContent radioContent { get; set; }

    [field: ES3Serializable] private string speakContent;
    public DeadBodyRadioIntroEvent(TimeRange startTimeRange, string speakContent, float speakRate, Gender speakGender, AudioMixerGroup mixer) :
     base(startTimeRange, new RadioTextContent(speakContent, speakRate, speakGender, mixer),
     RadioChannel.FM96) {
        this.speakContent = speakContent;
    }
         
    public DeadBodyRadioIntroEvent(): base(){}
     [field: ES3Serializable]
    public override float TriggerChance { get; } = 1;
    public override void OnEnd() {
        if (!radioModel.DescriptionDatas.Any()) {
            this.SendEvent<OnConstructDescriptionDatas>();
        }
        
        AlienDescriptionData descriptionData = radioModel.DescriptionDatas[0];
        radioModel.DescriptionDatas.RemoveAt(0);

        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        GameEventSystem eventSystem = this.GetSystem<GameEventSystem>();

        eventSystem.AddEvent(new DailyBodyRadio(
            new TimeRange(currentTime + new TimeSpan(0, 10, 0), currentTime + new TimeSpan(0, 20, 0)),
            AlienDescriptionFactory.GetRadioDescription(descriptionData.BodyInfo, descriptionData.Reality),
            Random.Range(0.85f, 1.2f), Random.Range(0, 2) == 0 ? Gender.MALE : Gender.FEMALE,
            AudioMixerList.Singleton.AudioMixerGroups[1]));

        eventSystem.AddEvent(new RandomStuffRadio(
            new TimeRange(currentTime + new TimeSpan(0, Random.Range(30, 60), 0)),
            RadioRandomStuff.Singleton.GetNextRandomRadio()));
        
        eventSystem.AddEvent(new FoodTutorialRadio(
            new TimeRange(currentTime + new TimeSpan(0, Random.Range(10, 40), 0)),
            AudioMixerList.Singleton.AudioMixerGroups[1]));

        
    }

    public override void OnMissed() {
        AddNextBodyInfo();
    }

    private void AddNextBodyInfo() {
        
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        int nextEventInterval = Random.Range(8,15);
        
        
        gameEventSystem.AddEvent(new DeadBodyRadioIntroEvent(
            new TimeRange(currentTime + new TimeSpan(0, nextEventInterval, 0),
                currentTime + new TimeSpan(0, nextEventInterval + 10, 0)),
            speakContent,
            1f, Gender.MALE, radioContent.mixer));
    }

    protected override void OnRadioStart() {
        
    }

    protected override void OnPlayedWhenRadioOff() {
        OnMissed();
    }
}

