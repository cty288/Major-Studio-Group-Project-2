using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using MikroFramework.Singletons;
using UnityEngine;

public class GameTimeManager : MonoMikroSingleton<GameTimeManager>, ISystem {

    private Func<bool> beforeEndOfTodayEvent = null;
    private int day = 0;

    public Action<int> OnDayStart = null;
    public BindableProperty<DateTime> CurrentTime = new BindableProperty<DateTime>(new DateTime(2022, 11, 12, 22, 0, 0));

    private void Start() {
        NextDay();
    }

    public void RegisterBeforeEndOfTodayEvent(Func<bool> beforeEndOfTodayEvent) {
        this.beforeEndOfTodayEvent = beforeEndOfTodayEvent;
    }

    private void NextDay() {
        day++;
        beforeEndOfTodayEvent = null;
        OnDayStart?.Invoke(day);
        this.Delay(3f, () => {
            CurrentTime.Value = new DateTime(2022, 11, 12, 22, 0, 0);
            this.Delay(3f, StartTimer);
        });
    }

    private void StartTimer() {
        StartCoroutine(GameTimerCoroutine());
    }

    private IEnumerator GameTimerCoroutine() {
        while (true) {
            yield return new WaitForSeconds(1f);
            CurrentTime.Value = CurrentTime.Value.AddMinutes(1);
            if (CurrentTime.Value.Hour == 23 && CurrentTime.Value.Minute == 59) {
                OnDayEnd();
                break;
            }
        }
    }

    private void OnDayEnd() {
        if (beforeEndOfTodayEvent == null) {
            NextDay();
        }else {
            UntilAction action = UntilAction.Allocate(beforeEndOfTodayEvent);
            action.OnEndedCallback += NextDay;
            action.Execute();
        }
    }


    #region Framework
    private IArchitecture architecture;
    public IArchitecture GetArchitecture()
    {
        return architecture;
    }

    public void SetArchitecture(IArchitecture architecture)
    {
        this.architecture = architecture;
    }

    public void Init()
    {

    }


    #endregion

}
