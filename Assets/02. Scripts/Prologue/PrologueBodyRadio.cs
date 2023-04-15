using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _02._Scripts.AlienInfos;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.BodyOutside;
using _02._Scripts.Prologue;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;


public class PrologueBodyRadio : RadioEvent<RadioTextContent> {
    [field: ES3Serializable]
    protected RadioTextContent radioContent { get; set; }

    protected override RadioTextContent GetRadioContent() {
        return radioContent;
    }
    protected override void SetRadioContent(RadioTextContent radioContent) {
        this.radioContent = radioContent;
    }
    private Coroutine radioCorruptCheckCoroutine;
     public PrologueBodyRadio(TimeRange startTimeRange, AudioMixerGroup mixer) :
         base(startTimeRange,new RadioTextContent("", 1, Gender.MALE, mixer),
         RadioChannel.AllChannels) {
         
         
        
         AlienDescriptionData descriptionData = radioModel.DescriptionDatas[0];
         radioModel.DescriptionDatas.RemoveAt(0);

         radioContent.SetContent(AlienDescriptionFactory.GetRadioDescription(descriptionData.BodyInfo,
             descriptionData.Reality,this.GetModel<AlienNameModel>().AlienName,
             AlienDescriptionFactory.RadioPrologue));
         
         DeliveryRadio();
         SpawnAlien();
     }
     
     public PrologueBodyRadio(): base(){}
     
     
     
     
     [field: ES3Serializable]
    public override float TriggerChance { get; } = 1;
    public override void OnEnd() {
        
        if (radioCorruptCheckCoroutine != null) {
            CoroutineRunner.Singleton.StopCoroutine(radioCorruptCheckCoroutine);
        }

       

    }

    private void DeliveryRadio() {
        DateTime currentTime = gameTimeManager.CurrentTime;
        DateTime happenTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day,
            23, 00, 0);

        gameEventSystem.AddEvent(new PrologueDeliveryRadio(
            new TimeRange(happenTime, happenTime.AddMinutes(60)),
            AudioMixerList.Singleton.AudioMixerGroups[1]));
    }

    public override void OnMissed() {
       
    }
    
    public void SpawnAlien() {
        BodyModel bodyModel = this.GetModel<BodyModel>();
        List<BodyTimeInfo> Aliens = bodyModel.Aliens;

        BodyInfo targetBody = Aliens[Random.Range(0, Aliens.Count)].BodyInfo;
       
        

        int knockDoorTimeInterval = 3;
       
        DateTime currentTime = gameTimeManager.CurrentTime;
        DateTime knockDoorTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day,
            23, 40, 0);

        gameEventSystem.AddEvent(new PrologueBodyGenerationEvent(
            new TimeRange(knockDoorTime), targetBody,
            1f));
    }

   

    protected override void OnRadioStart() {
       
    }
    protected override void OnPlayedWhenRadioOff() {
        OnMissed();
    }
   
}

