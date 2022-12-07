using System;
using System.Collections;
using System.Collections.Generic;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class BountyHunterPhone : TelephoneContact {
    private BountyHunterSystem bountyHunterSystem;
   
    
    private GameTimeManager gameTimeManager;
    private AudioMixerGroup mixer;
    private Coroutine waitingForInteractionCoroutine;
   
    private GameEventSystem gameEventSystem;
    private bool talkedBefore = false;
    private bool failedLastTime = false;

    private int provideCorrectInfoCount = 0;
 
    private BodyManagmentSystem bodyManagmentSystem;
    
    private bool lastPersonDead = false;
    public bool GetAvailable() {
        return bountyHunterSystem.NextAvailableDate.Date <= gameTimeManager.CurrentTime.Value.Date;
    }
    public BountyHunterPhone() {
        speaker = GameObject.Find("BountyHunterSpeaker").GetComponent<Speaker>();
        bountyHunterSystem = this.GetSystem<BountyHunterSystem>();
        gameTimeManager = this.GetSystem<GameTimeManager>();
        this.mixer = speaker.GetComponent<AudioSource>().outputAudioMixerGroup;
        gameEventSystem = this.GetSystem<GameEventSystem>();
        
    }
    public override bool OnDealt() {
        return gameTimeManager.CurrentTime.Value.Date >= bountyHunterSystem.NextAvailableDate.Date;
    }

    protected override void OnStart() {
       
        bodyManagmentSystem = this.GetSystem<BodyManagmentSystem>();
        bountyHunterSystem.ContactedBountyHunter = true;
        List<string> welcomes = new List<string>();
        welcomes.Add(
            "Howdy! I'm the Bounty Hunter! I can reward you foods if you give me correct information about those creatures! Do you have any information about them?");
        if (lastPersonDead) {
            welcomes.Clear();
            welcomes.Add("I can't find the last person you told me about. I'm not sure if they are dead or not. I'll give you a chance to report me a new one again.");
        }
        else {
            if (talkedBefore) {
                welcomes.Clear();
                if (failedLastTime) {
                    welcomes.Add(
                        "Hey! You almost killed me last time! Don't give me sources that you are not sure! I will give you one more chance -- do you have any REAL information about these creatures?");
                }
                else {
                    welcomes.Add("Hey my friend! What brought you here again? Do you have any information about those creatures?");
                    welcomes.Add("Hey, this is the bounty hunter you¡¯ve been collaborating with. Do you have any information about those creatures?");
                    welcomes.Add("Good job on taking down that thing! What new clues do you have today?");
                }

            }
        }
       
        speaker.Speak(welcomes[Random.Range(0, welcomes.Count)], mixer, "Bounty Hunter", OnWelcomeEnd);
    }

    private void OnWelcomeEnd() {
        bountyHunterSystem.IsBountyHunting.Value = true;
        TopScreenHintText.Singleton.Show(
            "Please select any information related to the unknown creatures to report to the bounty hunter.\n\nPossible information includes: figure outside / newspaper photos");
        this.RegisterEvent<OnBodyHuntingSelect>(OnBodyHuntingSelect);
        waitingForInteractionCoroutine = CoroutineRunner.Singleton.StartCoroutine(WaitingInteraction());
    }

    private void OnBodyHuntingSelect(OnBodyHuntingSelect e) {
     
        this.UnRegisterEvent<OnBodyHuntingSelect>(OnBodyHuntingSelect);
        if (waitingForInteractionCoroutine != null) {
            CoroutineRunner.Singleton.StopCoroutine(waitingForInteractionCoroutine);
            waitingForInteractionCoroutine = null;
        }
        bountyHunterSystem.IsBountyHunting.Value = false;
        TopScreenHintText.Singleton.Hide();

        //remove this body from the manager no matter what
        BodyInfo info = e.bodyInfo;
        DateTime current = gameTimeManager.CurrentTime.Value;
        current = current.AddDays(1);
        DateTime tomorrow = new DateTime(current.Year, current.Month, current.Day , 22, 0, 0);
        lastPersonDead = info.IsDead;
        
        if (!info.IsDead) {
            talkedBefore = true;
            if (info.IsAlien) {

                bountyHunterSystem.NextAvailableDate = gameTimeManager.CurrentTime.Value.AddDays(1);
              
                failedLastTime = false;
                provideCorrectInfoCount++;
                if (provideCorrectInfoCount == 1 && bountyHunterSystem.FalseClueCount <= bountyHunterSystem.MaxFalseClueCountForQuest)
                {
                    DateTime questStartTime = tomorrow.AddMinutes(Random.Range(30, 60));
                    DateTime questEndTime = new DateTime(questStartTime.Year, questStartTime.Month, questStartTime.Day, 23, 58, 0);
                    gameEventSystem.AddEvent(new BountyHunterQuestStartEvent(new TimeRange(questStartTime, questEndTime),
                        new BountyHunterQuestStartPhone(), 6));
                }

                if (info.ID == bountyHunterSystem.QuestBodyTimeInfo.BodyInfo.ID)
                {
                    bountyHunterSystem.QuestFinished = true;
                    Debug.Log("Correct Quest Info!!");
                }

            }
            else
            {
                bountyHunterSystem.FalseClueCount++;
                bountyHunterSystem.GoToJail(Random.Range(3, 5));
              
                List<string> killWrongPersonMsg = new List<string>();
                killWrongPersonMsg.Add("Sad news. One man was reported killed by a bounty hunter. He was believed to be mistaken as the unknown creature according to one of our sources. Officers are looking into this incident.");
                killWrongPersonMsg.Add("We have breaking news just came in. The police has found a dead male corpse at the riverbank this morning. A bounty hunter confessed to the police that he was responsible for this incident before the corpse was found.");
                string msg = killWrongPersonMsg[Random.Range(0, killWrongPersonMsg.Count)];
                failedLastTime = true;
                int radioTimeInterval = Random.Range(30, 60);
                BountyHunterHuntWrongPersonRadio radio = new BountyHunterHuntWrongPersonRadio(
                    new TimeRange(tomorrow.AddMinutes(radioTimeInterval),
                        tomorrow.AddMinutes(radioTimeInterval + Random.Range(30, 60))), msg, 1, Gender.MALE,
                    AudioMixerList.Singleton.AudioMixerGroups[1]);
                gameEventSystem.AddEvent(radio);

                Debug.Log("Current Time is " + current);
            }


        }



        gameEventSystem.AddEvent(new BountyHunterHuntEvent(new TimeRange(tomorrow), info));
        
        List<string> messages = new List<string>();
        messages.Add("Thank you partner! I will dig deeper into this. Once I find out the truth, I will reward you with foods!");
        messages.Add("Appreciate it buddy. I¡¯ll keep an eye on that person. I will reward you with foods once I find out the truth!");
        messages.Add("Thank you citizen! Your cooperation will make our community a better place!");
        string message = messages[Random.Range(0, messages.Count)];
        speaker.Speak(message, mixer, "Bounty Hunter", OnEndingSpeak);


        if (bountyHunterSystem.FalseClueCount >= bountyHunterSystem.FalseClueCountForPolice) {
            
            DateTime policeEventStartTime = tomorrow.AddMinutes(Random.Range(30, 60));
            DateTime policeEventEndTime = policeEventStartTime.AddMinutes(Random.Range(20, 40));
            gameEventSystem.AddEvent(new PoliceGenerateEvent(new TimeRange(policeEventStartTime, policeEventEndTime),
                PoliceGenerateEvent.GeneratePolice(),
                3, Random.Range(5, 9), 0.8f, null, null));
        }
    }

    private void OnEndingSpeak() {
        EndConversation();
    }

    private IEnumerator WaitingInteraction() {
        while (true) {
            yield return new WaitForSeconds(30);
            if (bountyHunterSystem.IsBountyHunting.Value) {
                string noInfo = "Don't waste my time! Tell me something about the creatures!";
                speaker.Speak(noInfo, mixer, "Bounty Hunter", null);
            }
        }
      
    }
    protected override void OnHangUp() {
        End();
    }

    protected override void OnEnd() {
        End();
    }

    private void End() {
        bountyHunterSystem.IsBountyHunting.Value = false;
        TopScreenHintText.Singleton.Hide();
        this.UnRegisterEvent<OnBodyHuntingSelect>(OnBodyHuntingSelect);
        if (waitingForInteractionCoroutine != null) {
            CoroutineRunner.Singleton.StopCoroutine(waitingForInteractionCoroutine);
            waitingForInteractionCoroutine = null;
        }
    }
}
