using System;
using UnityEngine;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using TMPro;

public class ClockObject : AbstractMikroController<MainGame>
//AbstractMikroController<MainGame>
//MonoBehaviour
{
    public Transform hourHand;
    public Transform minutesHand;
    [SerializeField] int hour;
    [SerializeField] int minute;

    private SpriteRenderer[] sprites;
    private TMP_Text timeText;
    private DateTime lastPlayedTime;
    void Awake()
    {
        //base.Awake();
        timeText = transform.Find("TimeCanvas/Time").GetComponent<TMP_Text>();
        this.GetSystem<GameTimeManager>().CurrentTime.RegisterWithInitValue(OnTimeChanged)
             .UnRegisterWhenGameObjectDestroyed(gameObject);
        sprites = GetComponentsInChildren<SpriteRenderer>(true);
        
    }

   

   

    void Update()
    {
        float minuteAngle = -(float)minute / 60 * 360;
        float hourAngle = -(float)(hour % 12) / 12 * 360 + minuteAngle/12;
        hourHand.rotation = Quaternion.Euler(0, 0, hourAngle);
        minutesHand.rotation = Quaternion.Lerp(minutesHand.rotation, Quaternion.Euler(0, 0, minuteAngle),
            0.1f);
    }

    private void OnTimeChanged(DateTime arg1, DateTime time) {
        hour = time.Hour;
        minute = time.Minute;
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");
        if (!AudioSystem.Singleton) {
            return;
        }

        if (time - lastPlayedTime <= TimeSpan.FromMinutes(1)) {
            return;
        }
        lastPlayedTime = time;
        try {
            AudioSystem.Singleton.Play2DSound("clock_tick", 0.3f);
        }
        catch (Exception e) {
           
        }
        
    }
}
