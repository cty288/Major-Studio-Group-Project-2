using System;
using UnityEngine;
using MikroFramework.Architecture;
using MikroFramework.Event;

public class ClockObject : AbstractMikroController<MainGame>
//AbstractMikroController<MainGame>
//MonoBehaviour
{
    public Transform hourHand;
    public Transform minutesHand;
    [SerializeField] int hour;
    [SerializeField] int minute;
    void Awake()
    {
        this.GetSystem<GameTimeManager>().CurrentTime.RegisterWithInitValue(OnTimeChanged)
             .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    void Update()
    {
        float minuteAngle = -(float)minute / 60 * 360;
        float hourAngle = -(float)(hour % 12) / 12 * 360 + minuteAngle/12;
        hourHand.rotation = Quaternion.Euler(0, 0, hourAngle);
        minutesHand.rotation = Quaternion.Euler(0, 0, minuteAngle);
    }

    private void OnTimeChanged(DateTime arg1, DateTime time)
    {
        hour = time.Hour;
        minute = time.Minute;
    }
}