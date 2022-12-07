using System;
using System.Collections;
using System.Collections.Generic;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

public class PoliceGenerateEvent : BodyGenerationEvent {
    public PoliceGenerateEvent(TimeRange startTimeRange, BodyInfo bodyInfo, float knockDoorTimeInterval, int knockTime, float eventTriggerChance, Action onEnd, Action onMissed, string overrideAudioClipName = null) :
        base(startTimeRange, bodyInfo, knockDoorTimeInterval, knockTime, eventTriggerChance, onEnd, onMissed, overrideAudioClipName) {
        Debug.Log("A police event is generated. The time is between " + startTimeRange.StartTime + " and " + startTimeRange.EndTime);
    }

    public static BodyInfo GeneratePolice() {
        HeightType height = Random.Range(0, 2) == 0 ? HeightType.Short : HeightType.Tall;
        AlienBodyPartInfo body = AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(BodyPartDisplayType.Shadow, BodyPartType.Body, false,
            height);
        AlienBodyPartInfo leg = AlienBodyPartCollections.Singleton.GetRandomBodyPartInfo(BodyPartDisplayType.Shadow, BodyPartType.Legs, false,
            height);
        AlienBodyPartInfo head = AlienBodyPartCollections.Singleton.SpecialBodyPartPrefabs.HeightSubCollections[0]
            .ShadowBodyPartPrefabs.HumanTraitPartsPrefabs[0].GetComponent<AlienBodyPartInfo>();

        return BodyInfo.GetBodyInfo(leg, body, head, height, Gender.MALE, BodyPartDisplayType.Shadow, false);

    }

    public override void OnMissed() {
        base.OnMissed();
        OnMissedOrNotOpen();
    }

    protected override void OnNotOpen() {
        base.OnNotOpen();
        OnMissedOrNotOpen();
    }

    private void OnMissedOrNotOpen() {
        DateTime tomorrow = gameTimeManager.CurrentTime.Value.AddDays(1);
        DateTime policeEventStartTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 22, Random.Range(30,50), 0);
        DateTime policeEventEndTime = policeEventStartTime.AddMinutes(Random.Range(20, 40));
        gameEventSystem.AddEvent(new PoliceGenerateEvent(new TimeRange(policeEventStartTime, policeEventEndTime),
            PoliceGenerateEvent.GeneratePolice(),
            3, Random.Range(5, 9), 0.8f, null, null));
    }

    protected override Func<bool> OnOpen() {
        onClickPeepholeSpeakEnd = false;
        Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
        string message =
            "Good day, mister. Recently we have found several bounty-hunter-related cases involving innocent civilians. " +
            "Several of those cases showed that the evidence was provided by you." +
            "Here¡¯s the search warrant, and we are taking you in for further inquiries. " +
            "Apologies and thank you for your cooperation.";
        speaker.Speak(message, AudioMixerList.Singleton.AudioMixerGroups[3], "???", OnFinishSpeak, 1.15f, 1.2f);
        return () => onClickPeepholeSpeakEnd;
    }

    private void OnFinishSpeak() {
        DieCanvas.Singleton.Show("You are arrested by the police!");
        this.GetModel<GameStateModel>().GameState.Value = GameState.End;
        this.GetSystem<BodyGenerationSystem>().StopCurrentBody();
    }
}

