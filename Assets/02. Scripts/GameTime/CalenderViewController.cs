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
    [SerializeField] private TMP_Text dayOfWeekText;
    private void Awake() {
        this.GetSystem<GameTimeManager>().CurrentTime.RegisterWithInitValue(OnTimeChange)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnTimeChange(DateTime time) {
        monthText.text = GetMonthAbbreviation(time.Month);
        dayText.text = time.Day.ToString();
        dayOfWeekText.text = GetDayOfWeekAbbreviation(time.DayOfWeek);
    }

    private string GetDayOfWeekAbbreviation(DayOfWeek timeDayOfWeek) {
        switch (timeDayOfWeek) {
            case DayOfWeek.Friday:
                return "Fri";
            case DayOfWeek.Monday:
                return "Mon";
            case DayOfWeek.Saturday:
                return "Sat";
            case DayOfWeek.Sunday:
                return "Sun";
            case DayOfWeek.Thursday:
                return "Thu";
            case DayOfWeek.Tuesday:
                return "Tue";
            case DayOfWeek.Wednesday:
                return "Wed";
        }
        return "";
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
