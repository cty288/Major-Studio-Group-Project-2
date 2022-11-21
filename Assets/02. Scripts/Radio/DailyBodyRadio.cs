using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

public struct OnConstructDescriptionDatas {

}
public class DailyBodyRadio : RadioEvent {


    private Coroutine radioCorruptCheckCoroutine;
    public DailyBodyRadio(TimeRange startTimeRange, string speakContent, float speakRate, Gender speakGender) : base(startTimeRange, speakContent, speakRate, speakGender) {

        }

        public override float TriggerChance { get; } = 1;
        public override void OnEnd() {
            if (radioCorruptCheckCoroutine != null) {
                CoroutineRunner.Singleton.StopCoroutine(radioCorruptCheckCoroutine);
            }
            AddNextBodyInfo();
        }

        public override void OnMissed() {
            AddNextBodyInfo();
        }

        private void AddNextBodyInfo() {
            if (!radioModel.DescriptionDatas.Any()) {
                this.SendEvent<OnConstructDescriptionDatas>();
            }

            AlienDescriptionData descriptionData = radioModel.DescriptionDatas[0];
            radioModel.DescriptionDatas.RemoveAt(0);


            DateTime currentTime = gameTimeManager.CurrentTime.Value;
            int nextEventInterval = Random.Range(7,15);


            gameEventSystem.AddEvent(new DailyBodyRadio(
                new TimeRange(currentTime + new TimeSpan(0, nextEventInterval, 0),
                    currentTime + new TimeSpan(0, nextEventInterval + 10, 0)),
                AlienDescriptionFactory.GetRadioDescription(descriptionData.BodyInfo, descriptionData.Reality),
                Random.Range(0.85f, 1.2f), Random.Range(0, 2) == 0 ? Gender.MALE : Gender.FEMALE));
        }

        protected override void OnRadioStart() {
            radioCorruptCheckCoroutine = CoroutineRunner.Singleton.StartCoroutine(RadioCorruptCheck());
        }

    private IEnumerator RadioCorruptCheck()
    {
        while (true) {
            yield return new WaitForSeconds(Random.Range(15f, 25f));
            if (Random.Range(0, 100) <= 30) {
                EndRadio();
                break;
            }
        }
    }
}

