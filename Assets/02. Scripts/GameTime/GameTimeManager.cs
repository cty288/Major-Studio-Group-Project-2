using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using MikroFramework.Singletons;
using MikroFramework.TimeSystem;
using MikroFramework.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct OnNewDay {
    public DateTime Date;
}
public class GameTimeManager : AbstractSystem, ISystem {

    private Func<bool> beforeEndOfTodayEvent = null;
    private int day = 0;

    public Action<int> OnDayStart = null;
    public BindableProperty<DateTime> CurrentTime = new BindableProperty<DateTime>(new DateTime(2022, 11, 12, 22, 0, 0));
    private GameStateModel gameStateModel;

    public SimpleRC LockTime { get; } = new SimpleRC();
 
    public void RegisterBeforeEndOfTodayEvent(Func<bool> beforeEndOfTodayEvent) {
        this.beforeEndOfTodayEvent = beforeEndOfTodayEvent;
    }

    public void NextDay() {
        day++;
        beforeEndOfTodayEvent = null;
        OnDayStart?.Invoke(day);
        this.GetSystem<ITimeSystem>().AddDelayTask(3f, () => {
            if (gameStateModel.GameState != GameState.End) {
                this.SendEvent<OnNewspaperUIPanelOpened>(
                    new OnNewspaperUIPanelOpened() { Newspaper = null, IsOpen = false });
                CurrentTime.Value = CurrentTime.Value.AddDays(1);
                CurrentTime.Value = new DateTime(CurrentTime.Value.Year, CurrentTime.Value.Month, CurrentTime.Value.Day, 22, 0, 0);
                this.SendEvent<OnNewDay>(new OnNewDay()
                {
                    Date = CurrentTime.Value
                });
                this.GetSystem<ITimeSystem>().AddDelayTask(5f, StartTimer);
            }
        });
    }

    private void StartTimer() {
        if (SceneManager.GetActiveScene().name != "MainGame") {
            return;
        }
        CoroutineRunner.Singleton.StartCoroutine(GameTimerCoroutine());
    }

    private IEnumerator GameTimerCoroutine() {
        while (true) {
            if (gameStateModel.GameState == GameState.End) {
                break;
            }
            if (SceneManager.GetActiveScene().name != "MainGame") {
                break;
            }
            yield return new WaitForSeconds(1f);
            if (LockTime.RefCount > 0) {
                continue;
            }
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
   

    protected override void OnInit() {
        gameStateModel = this.GetModel<GameStateModel>();
        //NextDay();
    }

    #endregion

}
