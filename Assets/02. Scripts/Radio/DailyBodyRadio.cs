using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _02._Scripts.Radio.RadioScheduling;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public struct OnConstructDescriptionDatas {

}
public class DailyBodyRadio : ScheduledRadioEvent<RadioTextContent> {

        [field: ES3Serializable]
        protected RadioTextContent radioContent { get; set; }

        protected override RadioTextContent GetRadioContent() {
            return radioContent;
        }
        protected override void SetRadioContent(RadioTextContent radioContent) {
            this.radioContent = radioContent;
        }
        private Coroutine radioCorruptCheckCoroutine;
         public DailyBodyRadio(TimeRange startTimeRange, string speakContent, float speakRate, Gender speakGender, AudioMixerGroup mixer) :
             base(startTimeRange, new RadioTextContent(speakContent, speakRate, speakGender, mixer),
             RadioChannel.FM96) {
             
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
            if (!radioModel.DescriptionDatas.Any()) {
                this.SendEvent<OnConstructDescriptionDatas>();
            }
            AlienDescriptionData descriptionData = radioModel.DescriptionDatas[0];
            radioModel.DescriptionDatas.RemoveAt(0);


            return new DailyBodyRadio(nextTimeRange,
                AlienDescriptionFactory.GetRadioDescription(descriptionData.BodyInfo, descriptionData.Reality),
                Random.Range(0.85f, 1.2f), Random.Range(0, 2) == 0 ? Gender.MALE : Gender.FEMALE, radioContent.mixer);
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

