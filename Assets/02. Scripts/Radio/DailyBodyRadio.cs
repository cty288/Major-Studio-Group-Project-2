﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _02._Scripts.AlienInfos;
using _02._Scripts.Radio.RadioScheduling;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

    //public struct OnConstructDescriptionDatas {

//}
public class DailyBodyRadio : ScheduledRadioEvent<RadioTextContent> {
    [field: ES3Serializable]
    protected override bool DayEndAfterFinish { get; set; } = true;

        [field: ES3Serializable]
        protected RadioTextContent radioContent { get; set; }

        [field: ES3Serializable] protected float speakRate;
        [field: ES3Serializable] protected Gender speakerGender;
        [field: ES3Serializable] protected AudioMixerGroup mixer;

      //  protected bool noMonsterToday = false;
        protected override RadioTextContent GetRadioContent() {
            return radioContent;
        }
        protected override void SetRadioContent(RadioTextContent radioContent) {
            this.radioContent = radioContent;
        }
        private Coroutine radioCorruptCheckCoroutine;
         public DailyBodyRadio(TimeRange startTimeRange, float speakRate, Gender speakGender, AudioMixerGroup mixer) :
             base(startTimeRange, null ,
             RadioChannel.FM96) {
             this.speakRate = speakRate;
             this.speakerGender = speakGender; 
             this.mixer = mixer;
           
         }

         protected void SetRadioContent() {
             if (radioModel.DescriptionDatas.Count == 0) {
                 RadioSchedulingSystem radioSchedulingSystem = this.GetSystem<RadioSchedulingSystem>();
                 if (!radioModel.HasDescriptionDatasToday) {
                     if (!radioSchedulingSystem.NoMonsterTodayAnnounced) {
                         radioSchedulingSystem.NoMonsterTodayAnnounced = true;
                         SetRadioContent(new RadioTextContent("We have no monster today. Enjoy your day!",
                             this.speakRate, this.speakerGender, this.mixer));
                     } else {
                         SetRadioContent(new RadioTextContent("", this.speakRate, this.speakerGender, this.mixer));
                     }
                 }
                 else {
                     SetRadioContent(new RadioTextContent("", this.speakRate, this.speakerGender, this.mixer));
                 }
                 return;
             }
             
             AlienDescriptionData descriptionData = radioModel.DescriptionDatas[0];
             radioModel.DescriptionDatas.RemoveAt(0);
             SetRadioContent(new RadioTextContent(
                 AlienDescriptionFactory.GetRadioDescription(descriptionData.BodyInfo, descriptionData.Reality, this.GetModel<AlienNameModel>().AlienName),
                 this.speakRate, this.speakerGender, this.mixer));
         }
        
    


         protected override void OnScheduledRadioStartPlay() {
             base.OnScheduledRadioStartPlay();
             SetRadioContent();
         }

         public DailyBodyRadio(): base(){}
         [field: ES3Serializable]
         protected override RadioProgramType ProgramType { get; set; } = RadioProgramType.DailyDeadBody;

         [field: ES3Serializable]
        public override float TriggerChance { get; } = 1;
        
        
        public override void OnEnd() {
            base.OnEnd();
            if (radioCorruptCheckCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(radioCorruptCheckCoroutine);
            }
            
        }

        
        protected override ScheduledRadioEvent<RadioTextContent> OnGetNextRadioProgramMessage(TimeRange nextTimeRange, bool playSuccess) {

            return new DailyBodyRadio(nextTimeRange,
                Random.Range(0.85f, 1.2f), Random.Range(0, 2) == 0 ? Gender.MALE : Gender.FEMALE,
                AudioMixerList.Singleton.AudioMixerGroups[1]);
        }

    
        protected override void OnRadioStart() {
            radioCorruptCheckCoroutine = CoroutineRunner.Singleton.StartCoroutine(RadioCorruptCheck());
        }

        protected override void OnPlayedWhenRadioOff() {
            
        }

        private IEnumerator RadioCorruptCheck()
        {
            while (true) {
                yield return new WaitForSeconds(Random.Range(15f, 25f));
                if (Random.Range(0, 100) <= 10) {
                    //EndRadio();
                    break;
                }
            }
        }
}

