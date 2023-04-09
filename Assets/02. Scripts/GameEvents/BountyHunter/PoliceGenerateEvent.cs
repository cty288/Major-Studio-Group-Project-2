using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

public class PoliceGenerateEvent : BodyGenerationEvent {
    public PoliceGenerateEvent(TimeRange startTimeRange, BodyInfo bodyInfo,float eventTriggerChance, Action onEnd, Action onMissed) :
        base(startTimeRange, bodyInfo, eventTriggerChance) {
        Debug.Log("A police event is generated. The time is between " + startTimeRange.StartTime + " and " + startTimeRange.EndTime);
    }
    
    public PoliceGenerateEvent(){}
    public static BodyInfo GeneratePolice() {
        HeightType height = HeightType.Tall;
        BodyPartPrefabInfo body = AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(BodyPartDisplayType.Shadow, BodyPartType.Body, false,
            height, false, null, 0);
        BodyPartPrefabInfo leg = AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(BodyPartDisplayType.Shadow, BodyPartType.Legs, false,
            height, false, null, 0);
        BodyPartPrefabInfo head = AlienBodyPartCollections.Singleton.SpecialBodyPartPrefabs.HeightSubCollections[0]
            .ShadowBodyPartPrefabs.HumanTraitPartsPrefabs[0].GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo();

        return BodyInfo.GetBodyInfo(leg, body, head, height,
            new VoiceTag(AudioMixerList.Singleton.AlienVoiceGroups[1], 1, Gender.MALE),
            new NormalKnockBehavior(3, Random.Range(3,7), null, "Knock_Police"),BodyPartDisplayType.Shadow, false);

    }

    public override void OnMissed() {
        OnMissedOrNotOpen();
    }

    protected override void OnNotOpen() {
        base.OnNotOpen();
        OnMissedOrNotOpen();
    }

    private void OnMissedOrNotOpen() {
        DateTime tomorrow = gameTimeManager.CurrentTime.Value.AddDays(1);
        DateTime policeEventStartTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, gameTimeManager.NightTimeStart, Random.Range(30,50), 0);
        DateTime policeEventEndTime = policeEventStartTime.AddMinutes(Random.Range(20, 40));
        gameEventSystem.AddEvent(new PoliceGenerateEvent(new TimeRange(policeEventStartTime, policeEventEndTime),
            PoliceGenerateEvent.GeneratePolice(), 0.8f, null, null));
    }

    protected override Func<bool> OnOpen() {
        onClickPeepholeSpeakEnd = false;
        Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
        string message =
            "Good day, mister. Recently we have found several bounty-hunter-related cases involving innocent civilians. " +
            "Several of those cases showed that the evidence was provided by you." +
            "Here¡¯s the search warrant, and we are taking you in for further inquiries. " +
            "Apologies and thank you for your cooperation.";
        speaker.Speak(message, AudioMixerList.Singleton.AudioMixerGroups[3], "???", 1f, OnFinishSpeak, 1.15f, 1.2f);
        return () => onClickPeepholeSpeakEnd;
    }

    public override void OnEventEnd() {
        
    }

    private void OnFinishSpeak(Speaker speaker) {
        DieCanvas.Singleton.Show("You are arrested by the police!");
        this.GetModel<GameStateModel>().GameState.Value = GameState.End;
        this.GetSystem<BodyGenerationSystem>().StopCurrentBody();
    }
}

