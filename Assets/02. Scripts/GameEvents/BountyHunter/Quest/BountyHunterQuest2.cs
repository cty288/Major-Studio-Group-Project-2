using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.GameEvents.BountyHunter;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.TimeSystem;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;



[ES3Serializable]
public class BountyHunterQuest2Notification : BountyHunterQuestClueNotification {
    public BountyHunterQuest2Notification(TimeRange startTimeRange, BountyHunterQuestClueNotificationContact notificationContact, int callWaitTime, DateTime clueHappenTime) : base(startTimeRange, notificationContact, callWaitTime, clueHappenTime) {
    }
    
    public BountyHunterQuest2Notification(): base(){}

    protected override BountyHunterQuestClueNotification GetSameEvent(TimeRange startTimeRange, TelephoneContact contact, int callWaitTime,
        DateTime clueHappenTime) {
        
        return new BountyHunterQuest2Notification(startTimeRange, (BountyHunterQuest2ClueNotificationNotificationContact) contact, callWaitTime, clueHappenTime);
    }

    protected override BountyHunterQuestClueNotification GetNextEvent(TimeRange startTimeRange, int callWaitTime, DateTime clueHappenTime) {
        return new BountyHunterQuest1ClueNotification(startTimeRange, new BountyHunterQuest1ClueNotificationNotificationContact(), callWaitTime,
            clueHappenTime);
    }

    protected override BountyHunterQuestClueEvent GetClueEvent(TimeRange happenTimeRange) {
        TimeRange newTimeRange = new TimeRange(happenTimeRange.StartTime, happenTimeRange.StartTime.AddMinutes(10));
        string location = BountyHunterQuestClueInfoRadioAreaEvent.GetRandomLocation();
        return new BountyHunterQuest2ClueEvent(newTimeRange,location,
            BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, false, false,
                new NormalKnockBehavior(Random.Range(1,3), UnityEngine.Random.Range(3,6),
                    new List<string>(){}), this.GetModel<BodyModel>().AvailableBodyPartIndices), null, null);
    }
}

public class BountyHunterQuest2ClueNotificationNotificationContact : BountyHunterQuestClueNotificationContact {
    public BountyHunterQuest2ClueNotificationNotificationContact(){}
    protected override void OnStart() {
        if (ClueHappenTime.Hour == 23 && ClueHappenTime.Minute >= 50) {
            ClueHappenTime = new DateTime(ClueHappenTime.Year, ClueHappenTime.Month, ClueHappenTime.Day, 23,
                Random.Range(40, 50), 0);
        }
        string hourIn12_1 = String.Format("{0: h}", ClueHappenTime);

        bool alreadyNotified = this.GetModel<BountyHunterModel>().QuestBodyClueAllHappened;

        string welcome =
            $"Hey friend, I hope you got some clues. I have gathered more information about the <color=yellow>location</color> of a dead body killed by the creature we are looking for." +
            $" Between <color=yellow>{hourIn12_1}:{ClueHappenTime.Minute} and {hourIn12_1}:{ClueHappenTime.Minute+10} pm</color>," +
            $" someone will deliver a note to you. Make sure there's no one outside your home when he arrives, otherwise he might be scared away. Call me back when you find out that guy!";

        if (alreadyNotified) {
            welcome = $"Hey friend, someone reported to me that they saw that creature again this morning." +
                      $" Between <color=yellow>{hourIn12_1}:{ClueHappenTime.Minute} and {hourIn12_1}:{ClueHappenTime.Minute + 10} pm</color>," +
                      $" I will let my friend to deliver you a note again about its <color=yellow>location</color>. Make sure there's no one outside your home when he arrives, otherwise he might be scared away. Call me back when you find out that guy!";

        }
        speaker.Speak(welcome, AudioMixerList.Singleton.AudioMixerGroups[2], "Bounty Hunter", 1f, OnSpeakEnd);
    }

    protected override void OnHangUp() {
        
    }

    protected override void OnEnd() {
       
    }

    private void OnSpeakEnd(Speaker speaker) {
        this.GetModel<BountyHunterModel>().QuestBodyClueAllHappened = true;
        EndConversation();
    }
}



public class BountyHunterQuest2ClueEvent : BountyHunterQuestClueEvent
{
    [field: ES3Serializable]
    public override GameEventType GameEventType { get; } = GameEventType.BodyGeneration;
    private string location;


    protected BodyGenerationModel bodyGenerationModel;

    protected Coroutine knockDoorCheckCoroutine;
    protected Coroutine nestedKnockDoorCheckCoroutine;
    
    protected BodyInfo bodyInfo;
  
    protected Action onEnd;
    protected Action onMissed;
    protected bool started = false;
   
    protected bool onClickPeepholeSpeakEnd = false;
  
    protected ITimeSystem timeSystem;
    protected AudioSource knockAudioSource;
    
    public BountyHunterQuest2ClueEvent(TimeRange startTimeRange, string location, BodyInfo bodyInfo, 
        Action onEnd, Action onMissed) : base(startTimeRange) {
        this.location = location;

        bodyGenerationModel = this.GetModel<BodyGenerationModel>();
        this.bodyInfo = bodyInfo;
       
        this.onEnd = onEnd;
        this.onMissed = onMissed;
       

       
        timeSystem = this.GetSystem<ITimeSystem>();
    }

    public override void OnStart()
    {
        this.RegisterEvent<OnOutsideBodyClicked>(OnOutsideBodyClicked);
    }

    private void OnOutsideBodyClicked(OnOutsideBodyClicked e) {
        if (e.BodyInfo.Equals(bodyInfo))
        {
            if (knockDoorCheckCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(knockDoorCheckCoroutine);
                knockDoorCheckCoroutine = null;
            }
            
            if (nestedKnockDoorCheckCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(nestedKnockDoorCheckCoroutine);
                nestedKnockDoorCheckCoroutine = null;
            }
            
            
            LoadCanvas.Singleton.LoadUntil(OnOpen, OnFinishOutsideBodyInteraction);
            if (knockAudioSource)
            {
                knockAudioSource.Stop();
            }
        }

        this.UnRegisterEvent<OnOutsideBodyClicked>(OnOutsideBodyClicked);
    }



    private void OnFinishOutsideBodyInteraction() {
        this.GetModel<GameSceneModel>().GameScene.Value = GameScene.MainGame;
        this.GetSystem<BodyGenerationSystem>().StopCurrentBody();
    }


    public override EventState OnUpdate()
    {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;

        if (gameStateModel.GameState.Value == GameState.End || (currentTime.Hour == 23 && currentTime.Minute >= 58)) {
            if (knockDoorCheckCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(knockDoorCheckCoroutine);
                knockDoorCheckCoroutine = null;
            }
            if (nestedKnockDoorCheckCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(nestedKnockDoorCheckCoroutine);
                nestedKnockDoorCheckCoroutine = null;
            }
            
            bodyGenerationModel.CurrentOutsideBody.Value = null;
            OnNotOpen();
            return EventState.End;
        }

        if (!started)
        {
            if (bodyGenerationModel.CurrentOutsideBody.Value != null)
            {
                OnNotOpen();
                return EventState.End;
            }
            started = true;
            knockDoorCheckCoroutine = CoroutineRunner.Singleton.StartCoroutine(KnockDoorCheck());
        }

        return (bodyGenerationModel.CurrentOutsideBody.Value == null && !bodyGenerationModel.CurrentOutsideBodyConversationFinishing) ? EventState.End : EventState.Running;
    }

    private void OnNotOpen() {
        
    }
    private Func<bool> OnOpen() {
        onClickPeepholeSpeakEnd = false;
        Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
        List<string> messages = new List<string>() {
            $"Hey. Got a word from my boss. He said the location was <color=yellow>{location}</color>.",
            $"Hey. My boss want to tell you that the prey appeared in <color=yellow>{location}</color>."
        };

        speaker.Speak(messages[Random.Range(0, messages.Count)], null, "Bounty Hunter",1f, OnDelivererClickedOutside, 1, 1.3f);
        
        return () => onClickPeepholeSpeakEnd;
    }

    private void OnDelivererClickedOutside(Speaker speaker) {
        timeSystem.AddDelayTask(1f, () => {
            onClickPeepholeSpeakEnd = true;
        });
    }

    public override void OnEnd() {
        base.OnEnd();
        if (knockDoorCheckCoroutine != null)
        {
            CoroutineRunner.Singleton.StopCoroutine(knockDoorCheckCoroutine);
            knockDoorCheckCoroutine = null;
        }
        
        if (nestedKnockDoorCheckCoroutine != null) {
            CoroutineRunner.Singleton.StopCoroutine(nestedKnockDoorCheckCoroutine);
            nestedKnockDoorCheckCoroutine = null;
        }
        onEnd?.Invoke();
        UnregisterListeners();
        this.UnRegisterEvent<OnOutsideBodyClicked>(OnOutsideBodyClicked);
    }


    private void UnregisterListeners()
    {
        onEnd = null;
        onMissed = null;

    }

    public override void OnMissed() {
        base.OnMissed();
        onMissed?.Invoke();
        UnregisterListeners();
    }

    private IEnumerator KnockDoorCheck() {
        
        Speaker speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
        bodyGenerationModel.CurrentOutsideBody.Value = bodyInfo;
      
        nestedKnockDoorCheckCoroutine = CoroutineRunner.Singleton.StartCoroutine(bodyInfo.KnockBehavior?.OnKnockDoor(speaker,
            bodyInfo.VoiceTag, bodyInfo.IsAlien));
        yield return nestedKnockDoorCheckCoroutine;
        
        knockDoorCheckCoroutine = null;
        bodyGenerationModel.CurrentOutsideBody.Value = null;
        bodyInfo.KnockBehavior?.OnStopKnock();
        OnNotOpen();
    }


    protected override BountyHunterQuestClueInfoEvent GetClueInfoEvent(TimeRange happenTimeRange, bool isRealClue, DateTime startDate)
    {
        return new BountyHunterQuestClueInfoRadioAreaEvent(happenTimeRange, "Bounty Hunter Quest Clue Radio", 1.2f,
            Gender.MALE, AudioMixerList.Singleton.AudioMixerGroups[1], isRealClue, startDate, location);
    }
}




public class BountyHunterQuestClueInfoRadioAreaEvent : BountyHunterQuestClueInfoEvent {
    [field: ES3Serializable]
    protected override RadioTextContent radioContent { get; set; }
    
    [ES3Serializable]
    private string location;
    public BountyHunterQuestClueInfoRadioAreaEvent(TimeRange startTimeRange, string speakContent, float speakRate, Gender speakGender, AudioMixerGroup mixer, bool isReal, DateTime startDate, string location) : 
        base(startTimeRange, speakContent, speakRate, speakGender, mixer, isReal, startDate) {
        this.location = location;
        BodyInfo info = this.GetModel<BountyHunterModel>().QuestBodyTimeInfo.BodyInfo;
        this.radioContent.SetContent(Radio(info, isReal));
    }

    public BountyHunterQuestClueInfoRadioAreaEvent() : base() {
       
    }

    protected override void OnRadioStart() {
       
    }

    protected override void OnPlayedWhenRadioOff() {
        
    }

    private string Radio(BodyInfo body, bool isReal)
    {
        DescriptionFormatter.Reality = 1;
        StringBuilder sb = new StringBuilder();

        string name = "government official";
        string location = this.location;
        bool alreadyNotified = this.GetModel<BountyHunterModel>().QuestBodyClueAllHappened;

     
        if (!isReal)
        {
            DescriptionFormatter.Reality = 0;
            List<string> names = new List<string>() { "security guard", "truck driver", "citizen" };
            name = names[Random.Range(0, names.Count)];
            location = GetRandomLocation();
            while (location == this.location) {
                location = GetRandomLocation();
            }
        }

        if (!alreadyNotified) {
            sb.AppendFormat("A victim, who was believed to be a {1}, was found at {0}.", location, name);
            sb.AppendFormat(AlienDescriptionFactory.Formatter,
                "{0:hair}. And {0:height}", body);
        }
        else {
            sb.AppendFormat("A resident reported that they encountered the creature who killed the {1} few days ago at {0}.", location, name);
            sb.AppendFormat(AlienDescriptionFactory.Formatter,
                "{0:hair}. And {0:height}", body);
        }
        
        return sb.ToString();
    }

    public static string GetRandomLocation() {
        List<string> locations = new List<string>() {
            "Eighth Avenue",
            "Riverside Park",
            "Central Park",
            "Fernway Park",
            "Springfield Park",
            "Hammonton Street",
            "Hudson Street",
            "West 57th Street",
            "Oxford Street",
            "Mikro Street",
            "Trinity Street",
        };
        return locations[Random.Range(0, locations.Count)];
    }

    protected override GameEvent GetSameEvent(TimeRange timeRange, bool isRealClue, DateTime dateTime)
    {
        return new BountyHunterQuestClueInfoRadioAreaEvent(timeRange, this.radioContent.speakContent, this.radioContent.speakRate, this.radioContent.speakGender,
            this.radioContent.mixer,
            isRealClue, dateTime, location);
    }
}