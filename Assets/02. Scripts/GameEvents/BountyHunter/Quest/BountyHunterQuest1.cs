using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public  class BountyHunterQuest1ClueNotification : BountyHunterQuestClueNotification {
    public BountyHunterQuest1ClueNotification(TimeRange startTimeRange, BountyHunterQuestClueNotificationContact notificationContact, int callWaitTime, DateTime clueHappenTime) 
        : base(startTimeRange, notificationContact, callWaitTime, clueHappenTime) {
    }

    
    protected override BountyHunterQuestClueNotification GetSameEvent(TimeRange startTimeRange, TelephoneContact contact, int callWaitTime,
        DateTime clueHappenTime) {
        return new BountyHunterQuest1ClueNotification(startTimeRange, (BountyHunterQuestClueNotificationContact) contact,
            callWaitTime, clueHappenTime);
    }

    protected override BountyHunterQuestClueNotification GetNextEvent(TimeRange startTimeRange,  int callWaitTime,
        DateTime clueHappenTime) {
        Debug.Log("BountyHunterQuest1ClueNotification.GetNextEvent");
        return new BountyHunterQuest2Notification(startTimeRange, new BountyHunterQuest2ClueNotificationNotificationContact(), callWaitTime,
            clueHappenTime);
    }

    protected override BountyHunterQuestClueEvent GetClueEvent(TimeRange happenTimeRange) {
        return new BountyHunterQuest1ClueEvent(happenTimeRange, Random.Range(2, 11));
    }

   
}

public class BountyHunterQuest1ClueNotificationNotificationContact : BountyHunterQuestClueNotificationContact
{
    protected override void OnStart() {
        string hourIn12 = String.Format("{0: h}", ClueHappenTime);

        bool alreadyNotified = this.GetSystem<BountyHunterSystem>().QuestBodyClueAllHappened;
        
        
        string welcome = $"Buddy, I got some clues about the time when a victim was killed by the creature we are looking for. At <color=yellow>{hourIn12}:{ClueHappenTime.Minute} pm</color>," +
                         $" pay attention to the flashlights outside your home. The number of flashlights will indicate which <color=yellow>hour</color> the victim died in PM. Don't forget call me back when you find out the murderer! Good luck!";

        if (alreadyNotified) {
            welcome = $"Buddy, someone reported a new occurrence of that creature! At <color=yellow>{hourIn12}:{ClueHappenTime.Minute} pm</color>," +
                      $" pay attention to the flashlights outside your home. As always, the number of flashlights will still indicate which <color=yellow>hour</color> in PM. Don't forget call me back when you find out the murderer! Good luck!";
        }
        speaker.Speak(welcome, AudioMixerList.Singleton.AudioMixerGroups[2], "Bounty Hunter", OnSpeakEnd);
    }

    private void OnSpeakEnd() {
        BountyHunterSystem bountyHunterSystem = this.GetSystem<BountyHunterSystem>();
        if (!bountyHunterSystem.QuestBodyClueAllHappened) {
            
            DateTime nextStartTime = ClueHappenTime.AddDays(1);
            nextStartTime = new DateTime(nextStartTime.Year, nextStartTime.Month, nextStartTime.Day, Random.Range(22, 24),
                Random.Range(10, 45), 0);
            DateTime nextEndTime = new DateTime(nextStartTime.Year, nextStartTime.Month, nextStartTime.Day,
                23, 50, 0);

            this.GetSystem<GameEventSystem>().AddEvent(new BountyHunterQuestAlienSpawnEvent(
                new TimeRange(nextStartTime, nextEndTime),
                bountyHunterSystem.QuestBodyTimeInfo.BodyInfo, Random.Range(2f, 4f), Random.Range(4, 8), 1));

            Debug.Log("Target quest body will be spawned at " + nextStartTime);
        }
        EndConversation();
    }

    protected override void OnHangUp() {
       
    }

    protected override void OnEnd() {
        
    }
}

public struct OnFlashlightFlash {

}

public class BountyHunterQuest1ClueEvent : BountyHunterQuestClueEvent {
    private int flashlightTime;
    private int flashedTime = 0;
    
    private float flashTimeInterval = 1f;
    private float flashIntervalTimer = 0;
    
    
    public BountyHunterQuest1ClueEvent(TimeRange startTimeRange, int flashlightTime) : base(startTimeRange) {
        this.flashlightTime = flashlightTime;
    }

    public override void OnStart() {
        BountyHunterSystem bountyHunterSystem = this.GetSystem<BountyHunterSystem>();
        Debug.Log($"Clue Start! Flash Time: {flashedTime}");
        float chanceForNewspaperShowBody = Random.Range(0f, 1f);
        if (!bountyHunterSystem.QuestBodyClueAllHappened) {
            chanceForNewspaperShowBody = 1;
        }
        if (chanceForNewspaperShowBody > 0.6f) {
            this.GetSystem<BodyManagmentSystem>()
                .AddNewBodyTimeInfoToNextDayDeterminedBodiesQueue(bountyHunterSystem.QuestBodyTimeInfo);
            Debug.Log("Tomorrow's Quest Body will be shown in newspaper!");
        }
    }

    private void Flash() {
        this.SendEvent<OnFlashlightFlash>();
    }

    public override EventState OnUpdate() {
        flashIntervalTimer += Time.deltaTime;
        if (flashIntervalTimer >= flashTimeInterval) {
            flashIntervalTimer = 0;
            flashedTime++;
            Flash();
        }

        return flashedTime >= flashlightTime ? EventState.End : EventState.Running;
    }

    protected override BountyHunterQuestClueInfoEvent GetClueInfoEvent(TimeRange happenTimeRange, bool isRealClue, DateTime startDate) {
        return new BountyHunterQuestClueInfoRadioEvent(happenTimeRange, "Bounty Hunter Quest Clue Radio", 1.2f,
            Gender.MALE, AudioMixerList.Singleton.AudioMixerGroups[1], isRealClue, startDate, flashlightTime);
    }
}


public class BountyHunterQuestClueInfoRadioEvent : BountyHunterQuestClueInfoEvent {
    private int dieTime;
    public BountyHunterQuestClueInfoRadioEvent(TimeRange startTimeRange, string speakContent, float speakRate, Gender speakGender, AudioMixerGroup mixer, bool isReal, DateTime startDate, int dieTime) : base(startTimeRange, speakContent, speakRate, speakGender, mixer, isReal, startDate) {
        this.dieTime = dieTime;
        BodyInfo info = this.GetSystem<BountyHunterSystem>().QuestBodyTimeInfo.BodyInfo;
        this.speakContent = Radio(info, isReal);
    }
    
    protected override void OnRadioStart() {
       
    }

    private  string Radio(BodyInfo body, bool isReal) {
        DescriptionFormatter.Reality = 1;
        StringBuilder sb = new StringBuilder();
        bool alreadyNotified = this.GetSystem<BountyHunterSystem>().QuestBodyClueAllHappened;
        int time = dieTime;
        string name = "government official";

        if (!isReal)
        {
            DescriptionFormatter.Reality = 0;
            List<string> names = new List<string>() { "security guard", "truck driver", "citizen" };
            name = names[Random.Range(0, names.Count)];
            time = Random.Range(1, 12);
            while (time == dieTime)
            {
                time = Random.Range(1, 12);
            }
        }

        
        if (!alreadyNotified) {
            sb.AppendFormat("The speculation showed that a {1} died at {0} pm.", time, name);
            sb.AppendFormat(AlienDescriptionFactory.Formatter,
                " The CCTV captured his last image, {0:clothb}. In addition, {0:clothl}", body);
            return sb.ToString();
        }
        else {
            sb.AppendFormat("The creature who killed the {1} a few days ago was once again seen at {0} pm this afternoon.", time, name);
            sb.AppendFormat(AlienDescriptionFactory.Formatter,
                " The CCTV captured his last image. {0:clothb}. In addition, {0:clothl}", body);
            return sb.ToString();
        }
       
    }

    

    protected override GameEvent GetSameEvent(TimeRange timeRange, bool isRealClue, DateTime dateTime) {
        return new BountyHunterQuestClueInfoRadioEvent(timeRange, this.speakContent, this.speakRate, this.speakGender,
            this.mixer,
            isRealClue, dateTime, dieTime);
    }
}
