using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;

public class CalenderViewController : AbstractMikroController<MainGame> {
    [SerializeField] private TMP_Text monthText;
    [SerializeField] private TMP_Text dayText;
    private void Awake() {
        GameTimeManager.Singleton.CurrentTime.RegisterWithInitValue(OnTimeChange)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnTimeChange(DateTime time) {
        monthText.text = GetMonthAbbreviation(time.Month);
        dayText.text = time.Day.ToString();
    }

    public static string GetMonthAbbreviation(int month) {
        switch (month) {
            case 1:
                return "Jan";
            case 2:
                return "Feb";
            case 3:
                return "Mar";
            case 4:
                return "Apr";
            case 5:
                return "May";
            case 6:
                return "Jun";
            case 7:
                return "Jul";
            case 8:
                return "Aug";
            case 9:
                return "Sep";
            case 10:
                return "Oct";
            case 11:
                return "Nov";
            case 12:
                return "Dec";
        }
        return "Jan";
    }
}
