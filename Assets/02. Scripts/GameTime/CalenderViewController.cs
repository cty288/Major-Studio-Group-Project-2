using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CalenderViewController : AbstractMikroController<MainGame>, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [SerializeField] private TMP_Text monthText;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text dayOfWeekText;
    private GameTimeModel gameTimeModel;
    // [SerializeField] private WarningPanel warningPanel;
   private GameObject warningPanel;
    private GameTimeManager gameTimeManager;
    private bool isSkippingTime = false;
    private void Awake() {
        this.GetSystem<GameTimeManager>().CurrentTime.RegisterWithInitValue(OnTimeChange)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnNewDay>(OnNewDay);
        gameTimeManager = this.GetSystem<GameTimeManager>();
        gameTimeModel = this.GetModel<GameTimeModel>();
        //warningPanel = GetComponent<WarningPanel>();
        warningPanel = transform.Find("WarningCanvas").gameObject;
        warningPanel.SetActive(false);
    }

    private void OnNewDay(OnNewDay e) {
        gameObject.SetActive(e.Day >= 1);
        if (isSkippingTime) {
            isSkippingTime = false;
            LoadCanvas.Singleton.StopLoad(null);
        }
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

    public void OnPointerEnter(PointerEventData eventData) {
     //  warningPanel.Show(0);
         if (gameTimeModel.Day != 0) {
             warningPanel.SetActive(true);
         }
    }

    public void OnPointerExit(PointerEventData eventData) {
   //   warningPanel.Hide();
       if (gameTimeModel.Day != 0) {
           warningPanel.SetActive(false);
       }
    }

    public void OnPointerClick(PointerEventData eventData) {
        LoadCanvas.Singleton.Load(OnSkipTimeStart, false);
    }

    private void OnSkipTimeStart() {
        DateTime time = gameTimeManager.CurrentTime.Value;
        this.GetModel<RadioModel>().IsOn.Value = false;
        this.GetSystem<TelephoneSystem>().HangUp(false);
        isSkippingTime = true;
        
        gameTimeManager.SkipTimeTo(new DateTime(time.Year, time.Month, time.Day, 23, 58, 0));
        this.GetSystem<TelephoneSystem>().HangUp(false);
        
    }
}
