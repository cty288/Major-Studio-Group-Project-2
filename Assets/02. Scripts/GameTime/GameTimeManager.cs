using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using _02._Scripts.Stats;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.BindableProperty;
using MikroFramework.Singletons;
using MikroFramework.TimeSystem;
using MikroFramework.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct OnNewDay {
    public DateTime Date;
    public int Day;
    public bool IsNewWeek;
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

    private StatsModel statsModel;
    protected float timeSpeed = 1f;
    protected DateTime timeSpeedUntil = DateTime.MinValue;

    public int NightTimeStart {
        get {
            return gameTimeModel.NightTimeStart;
        }
    }

    public int DayTimeStart { get { return gameTimeModel.DayTimeStart; } }

    public int DayTimeEnd { get { return gameTimeModel.DayTimeEnd; } }

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
        gameTimeModel.AddDay(out bool isNewWeek);
        hasCharmingSoundToday = true;
        //beforeEndOfTodayEvent = null;
        int startHour = OutdoorActivityModel.HasMap.Value ? DayTimeStart : NightTimeStart;
        
        OnDayStart?.Invoke(gameTimeModel.Day, startHour);
        if (gameStateModel.GameState != GameState.End) {
            NewDayStart(isNewWeek);
        }
       
    }

    private void NewDayStart(bool isNewWeek) {
        int startHour = OutdoorActivityModel.HasMap.Value ? DayTimeStart : NightTimeStart;
        this.GetSystem<ITimeSystem>().AddDelayTask(2f, () => {
            if (gameStateModel.GameState != GameState.End) {
                this.GetModel<GameSceneModel>().GameScene.Value = GameScene.MainGame;
                
                (MainGame.Interface as MainGame)?.SaveGame();
                DateTime nextDay = gameTimeModel.CurrentTime.Value.AddDays(1);
                gameTimeModel.CurrentTime.Value = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, startHour, 0, 0);

                statsModel.UpdateStat("DaySurvived",
                    new SaveData("Days Survived", (int) statsModel.GetStat("DaySurvived", -1) + 1));
                
                this.SendEvent<OnNewDay>(new OnNewDay() {
                    Date = gameTimeModel.CurrentTime.Value,
                    Day = gameTimeModel.Day,
                    IsNewWeek = isNewWeek
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

    private bool hasCharmingSoundToday = true;

    public bool HasCharmingSoundToday {
        get => hasCharmingSoundToday;
        set => hasCharmingSoundToday = value;
    }
    public void SkipTimeTo(DateTime time) {
        gameTimeModel.CurrentTime.Value = time;
    }
    private IEnumerator GameTimerCoroutine() {
        while (true) {
            if (gameStateModel.GameState == GameState.End) {
                break;
            }
            if (SceneManager.GetActiveScene().name != "MainGame") {
                break;
            }

            if (gameTimeModel.LockTime.RefCount > 0) {
                yield return null;
                continue;
            }
            yield return new WaitForSeconds(gameTimeModel.GlobalTimeFreqCurve.Evaluate(gameTimeModel.Day) / timeSpeed);


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

                if (Day > 0 && hasCharmingSoundToday) {
                    AudioSource source = AudioSystem.Singleton.Play2DSound("EndOfDay3Chimes");
                    this.GetSystem<ITimeSystem>().AddDelayTask(source.clip.length/2, () => {
                        NextDay();
                    });
                }
                else {
                    NextDay();
                }
               
               // (MainGame.Interface as MainGame)?.SaveGame();
               
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
        statsModel = this.GetModel<StatsModel>();
        //NextDay();
        
        if (Day == -1) {
            NextDay();
        }
        else {
            NewDayStart(Day % 7 == 0);
        }
        
    }

    #endregion

}
