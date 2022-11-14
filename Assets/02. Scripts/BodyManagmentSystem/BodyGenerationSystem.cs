using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.BindableProperty;
using MikroFramework.TimeSystem;
using UniRx.InternalUtil;

using UnityEngine;

public class BodyGenerationSystem : AbstractSystem {
    public BindableProperty<BodyInfo> CurrentOutsideBody = new BindableProperty<BodyInfo>();
    private BodyInfo TodayAlien;
    private int dayNum;
    private BodyManagmentSystem bodyManagmentSystem;

    private float knockDoorCheckTimeInterval = 10f;
    private float knockDoorChance = 0.2f;
    private float nonAlienChance = 0.5f;

    private float knockWaitTimeSinceDayStart = 10f;
    private Coroutine knockDoorCheckCoroutine;
    protected override void OnInit() {
        this.GetSystem<ITimeSystem>().AddDelayTask(0.1f, () => {
            AudioSystem.Singleton.Initialize(null);
            AudioSystem.Singleton.MasterVolume = 1f;
            AudioSystem.Singleton.MusicVolume = 1f;
            AudioSystem.Singleton.SoundVolume = 1f;
        });
       
       GameTimeManager.Singleton.OnDayStart += OnEndOfDay;
        this.RegisterEvent<OnNewBodyInfoGenerated>(OnNewBodyInfoGenerated);
        bodyManagmentSystem = this.GetSystem<BodyManagmentSystem>();
    }

    private void OnNewBodyInfoGenerated(OnNewBodyInfoGenerated e) {
        TodayAlien = bodyManagmentSystem.allBodyTimeInfos[Random.Range(0, bodyManagmentSystem.allBodyTimeInfos.Count)]
            .BodyInfo;
    }


    private void OnEndOfDay(int day) {
        if (knockDoorCheckCoroutine != null) {
            CoroutineRunner.Singleton.StopCoroutine(knockDoorCheckCoroutine);
            
        }

        CurrentOutsideBody.Value = null;
        dayNum = day;
        this.GetSystem<ITimeSystem>().AddDelayTask(knockWaitTimeSinceDayStart, () => {
            knockDoorCheckCoroutine = CoroutineRunner.Singleton.StartCoroutine(KnockDoorCheck());
        });
    }

    private IEnumerator KnockDoorCheck() {
        if (dayNum > 3) {
            while (true) {
                yield return new WaitForSeconds(knockDoorCheckTimeInterval);
                if (Random.Range(0f, 1f) <= knockDoorChance) {
                    break;
                }
            }

            //spawn body outside!
            if (Random.Range(0f, 1f) <= nonAlienChance) {
                CurrentOutsideBody.Value = BodyInfo.GetBodyInfoOpposite(TodayAlien, 0.7f, 0.5f, true);
            }else {
                CurrentOutsideBody.Value = TodayAlien;
            }

            Debug.Log("Start Knock");
            //alien start knocking
            int knockDoorTimeInterval = 3;
            int knockTime = Random.Range(6, 9);

            for (int i = 0; i < knockTime; i++) {
                string clipName = $"knock_{Random.Range(1, 8)}";
                AudioSource source = AudioSystem.Singleton.Play2DSound(clipName, 1, false);
                yield return new WaitForSeconds(source.clip.length + knockDoorTimeInterval);
            }

            CurrentOutsideBody.Value = null;
            knockDoorCheckCoroutine = CoroutineRunner.Singleton.StartCoroutine(KnockDoorCheck());
        }
    }
}
