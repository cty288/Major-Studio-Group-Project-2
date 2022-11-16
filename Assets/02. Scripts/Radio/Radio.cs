using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MikroFramework.Event;
using MikroFramework.Architecture;
using MikroFramework;
using MikroFramework.AudioKit;
public class Radio : AbstractMikroController<MainGame>
{
    public Speaker speaker;
    [SerializeField]
    private AudioClip radioOpenSound;
    public BodyInfo targetAlien;

    void Start()
    {
        this.RegisterEvent<OnNewBodyInfoGenerated>(PlayRadioInformation).UnRegisterWhenGameObjectDestroyed(gameObject);
    }


    public void PlayRadioInformation()//播放radio info
    {
        AudioSource aus = AudioSystem.Singleton.Play2DSound(radioOpenSound, 1);
        this.Delay(radioOpenSound.length, () => {
            speaker.Speak(AlienDescriptionFactory.GetRadioDescription(targetAlien, 1));
        });
    }



    private void PlayRadioInformation(OnNewBodyInfoGenerated e)
    {
        int day = this.GetSystem<GameTimeManager>().Day;
        float properRate = 0;
        if (day <= 5) {
             properRate = 1;
        }
        else
        {
            properRate = 2f - (Mathf.Log(day) / Mathf.Log(4));
            properRate = Mathf.Max(properRate, 0);
        }

        BodyGenerationSystem BGsys = this.GetSystem<BodyGenerationSystem>();
        BodyManagmentSystem BMsys = this.GetSystem<BodyManagmentSystem>();

        if (Random.value <= properRate)
        {
            targetAlien = BGsys.TodayAlien;
        }
        else
        {
            targetAlien = BMsys.allBodyTimeInfos[Random.Range(0, BMsys.allBodyTimeInfos.Count)].BodyInfo;
        }
    }
}