using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameTime;
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
    public int Day;
}

public struct OnEndOfOutdoorDayTimeEvent {
    public List<Func<bool>> OnEndOfDayTimeEventList;
}
public class GameTimeManager : AbstractSystem, ISystem {

    //private Func<bool> beforeEndOfTodayEvent = null;
    private List<Func<bool>> endOfDayTimeEvents = new List<Func<bool>>();
    
    public Action<int, int> OnDayStart = null;
    
    
    private GameStateModel gameStateModel;

    public SimpleRC LockDayEnd { get; } = new SimpleRC();

    public SimpleRC LockOutdoorEventEnd { get; } = new SimpleRC();
    
    private GameTimeModel gameTimeModel;

   

    protected float timeSpeed = 1f;
    protected DateTime timeSpeedUntil = DateTime.MinValue;

    public int NightTimeStart { get; private set; } = 22;
    public int DayTimeStart { get; private set; } = 9;
    
    public int DayTimeEnd { get; private set; } = 17;

    public bool IsNight {
        get {
            return gameTimeModel.CurrentTime.Value.Hour >= NightTimeStart ||
                   gameTimeModel.CurrentTime.Value.Hour < DayTimeStart;
        }
    }
    
    protected OutdoorActivityModel OutdoorActivityModel;
    public BindableProperty<DateTime> CurrentTime {
        get {
            return gameTimeModel.CurrentTime;
        }
    }
    
    public int Day {
        get {
            return gameTimeModel.Day;
        }
    }
    
    

    public void NextDay() {
        gameTimeModel.AddDay();
        
        //beforeEndOfTodayEvent = null;
        int startHour = OutdoorActivityModel.HasMap.Value ? DayTimeStart : NightTimeStart;
        
        OnDayStart?.Invoke(gameTimeModel.Day, startHour);
        if (gameStateModel.GameState != GameState.End) {
            NewDayStart();
        }
       
    }

    private void NewDayStart() {
        int startHour = OutdoorActivityModel.HasMap.Value ? DayTimeStart : NightTimeStart;
        this.GetSystem<ITimeSystem>().AddDelayTask(2f, () => {
            if (gameStateModel.GameState != GameState.End) {
                this.GetModel<GameSceneModel>().GameScene.Value = GameScene.MainGame;
                
                (MainGame.Interface as MainGame)?.SaveGame();
                DateTime nextDay = gameTimeModel.CurrentTime.Value.AddDays(1);
                gameTimeModel.CurrentTime.Value = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, startHour, 0, 0);
                
                this.SendEvent<OnNewDay>(new OnNewDay() {
                    Date = gameTimeModel.CurrentTime.Value,
                    Day = gameTimeModel.Day
                });
                this.GetSystem<ITimeSystem>().AddDelayTask(3f, StartTimer);
            }
        });
    }

    private void StartTimer() {
        if (SceneManager.GetActiveScene().name != "MainGame") {
            return;
        }
        CoroutineRunner.Singleton.StartCoroutine(GameTimerCoroutine());
    }

    public void SpeedUpTime(float multiplier, DateTime until) {
        timeSpeed = multiplier;
        timeSpeedUntil = until;
    }

    private IEnumerator GameTimerCoroutine() {
        while (true) {
            if (gameStateModel.GameState == GameState.End) {
                break;
            }
            if (SceneManager.GetActiveScene().name != "MainGame") {
                break;
            }
            yield return new WaitForSeconds(gameTimeModel.GlobalTimeFreq / timeSpeed);


            if (!(gameTimeModel.CurrentTime.Value.Hour == 23 && gameTimeModel.CurrentTime.Value.Minute == 59)
                && !(gameTimeModel.CurrentTime.Value.Hour == DayTimeEnd && gameTimeModel.CurrentTime.Value.Minute == 0)) {
                gameTimeModel.CurrentTime.Value = gameTimeModel.CurrentTime.Value.AddMinutes(1);
            }
            
            if(gameTimeModel.CurrentTime.Value >= timeSpeedUntil && timeSpeed != 1) {
                timeSpeed = 1;
            }
            
            if (gameTimeModel.CurrentTime.Value.Hour == 23 && gameTimeModel.CurrentTime.Value.Minute == 59) {
                if (LockDayEnd.RefCount > 0) {
                    continue;
                }
               // (MainGame.Interface as MainGame)?.SaveGame();
                NextDay();
                break;
            }
            
            if(gameTimeModel.CurrentTime.Value.Hour == DayTimeEnd && gameTimeModel.CurrentTime.Value.Minute == 00) {
                if (LockOutdoorEventEnd.RefCount > 0) {
                    continue;
                }
                OnOutdoorEventEnd();
                break;
            }
        }
    }

    private void OnOutdoorEventEnd() {
        endOfDayTimeEvents.Clear();
        this.SendEvent<OnEndOfOutdoorDayTimeEvent>(new OnEndOfOutdoorDayTimeEvent() {
            OnEndOfDayTimeEventList = endOfDayTimeEvents
        });
        if (endOfDayTimeEvents.Count > 0) {
            UntilAction action = UntilAction.Allocate(() => {
                bool allFinished = true;
                foreach (var func in endOfDayTimeEvents) {
                    if (!func()) {
                        allFinished = false;
                        break;
                    }
                }

                return allFinished;
            });
            
            
            action.OnEndedCallback += () => {
                CoroutineRunner.Singleton.StartCoroutine(SkipToNightActivity());
            };
            action.Execute();
        }
        else {
            CoroutineRunner.Singleton.StartCoroutine(SkipToNightActivity());
        }
    }

    private IEnumerator SkipToNightActivity() {
        while (true) {
            yield return null;
            gameTimeModel.CurrentTime.Value = gameTimeModel.CurrentTime.Value.AddMinutes(1);
            if (gameTimeModel.CurrentTime.Value.Hour == NightTimeStart) {
                break;
            }
        }
        StartTimer();
    }

 


    #region Framework
   

    protected override void OnInit() {
        gameStateModel = this.GetModel<GameStateModel>();
        gameTimeModel = this.GetModel<GameTimeModel>();
        OutdoorActivityModel = this.GetModel<OutdoorActivityModel>();
        //NextDay();
        
        if (Day == -1) {
            NextDay();
        }
        else {
            NewDayStart();
        }
        
    }

    #endregion

}
