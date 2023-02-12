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
}
public class GameTimeManager : AbstractSystem, ISystem {

    private Func<bool> beforeEndOfTodayEvent = null;
    public Action<int> OnDayStart = null;
    
    
    private GameStateModel gameStateModel;

    public SimpleRC LockDayEnd { get; } = new SimpleRC();
    
    private GameTimeModel gameTimeModel;

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
        
        beforeEndOfTodayEvent = null;
        OnDayStart?.Invoke(gameTimeModel.Day);
        this.GetSystem<ITimeSystem>().AddDelayTask(2f, () => {
            if (gameStateModel.GameState != GameState.End) {
                this.GetModel<GameSceneModel>().GameScene.Value = GameScene.MainGame;
                
                
                DateTime nextDay = gameTimeModel.CurrentTime.Value.AddDays(1);
                gameTimeModel.CurrentTime.Value = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 22, 0, 0);
                this.SendEvent<OnNewDay>(new OnNewDay() {
                    Date = gameTimeModel.CurrentTime.Value
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

    private IEnumerator GameTimerCoroutine() {
        while (true) {
            if (gameStateModel.GameState == GameState.End) {
                break;
            }
            if (SceneManager.GetActiveScene().name != "MainGame") {
                break;
            }
            yield return new WaitForSeconds(1f);
            

            if (!(gameTimeModel.CurrentTime.Value.Hour == 23 && gameTimeModel.CurrentTime.Value.Minute == 59)) {
                gameTimeModel.CurrentTime.Value = gameTimeModel.CurrentTime.Value.AddMinutes(1);
            }
            
            if (gameTimeModel.CurrentTime.Value.Hour == 23 && gameTimeModel.CurrentTime.Value.Minute == 59) {
                if (LockDayEnd.RefCount > 0) {
                    continue;
                }
                OnDayEnd();
                break;
            }
        }
    }

    private void OnDayEnd() {
        if (beforeEndOfTodayEvent == null) {
            (MainGame.Interface as MainGame)?.SaveGame();
            NextDay();
        }else {
            UntilAction action = UntilAction.Allocate(beforeEndOfTodayEvent);
            action.OnEndedCallback += () => {
                (MainGame.Interface as MainGame)?.SaveGame();
                NextDay();
            };
            action.Execute();
        }
    }


    #region Framework
   

    protected override void OnInit() {
        gameStateModel = this.GetModel<GameStateModel>();
        gameTimeModel = this.GetModel<GameTimeModel>();
        //NextDay();
    }

    #endregion

}
