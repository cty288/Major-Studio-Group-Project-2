using System;
using UnityEngine;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;

public class ClockObject : DraggableItems
//AbstractMikroController<MainGame>
//MonoBehaviour
{
    public Transform hourHand;
    public Transform minutesHand;
    [SerializeField] int hour;
    [SerializeField] int minute;

    private SpriteRenderer[] sprites;
    private TMP_Text timeText;
    void Awake()
    {
        base.Awake();
        timeText = transform.Find("TimeCanvas/Time").GetComponent<TMP_Text>();
        this.GetSystem<GameTimeManager>().CurrentTime.RegisterWithInitValue(OnTimeChanged)
             .UnRegisterWhenGameObjectDestroyed(gameObject);
        sprites = GetComponentsInChildren<SpriteRenderer>(true);
        
    }

    public override void SetLayer(int layer) {
        foreach (SpriteRenderer sprite in sprites) {
            //sprite.sortingOrder = layer;
        }
    }

    protected override void OnClick() {
        
    }

    public override void OnThrownToRubbishBin() {
        Destroy(gameObject);
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
    }
}
