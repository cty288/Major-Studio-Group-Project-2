using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;

using UnityEngine;


public enum EventState {
    NotStart,
    Running,
    End,
    Missed
}

public class TimeRange {
    [field: ES3Serializable]
    public DateTime StartTime { get; set; }
    [field: ES3Serializable]
    public DateTime EndTime { get; set; }

    public TimeRange(DateTime startTime, DateTime endTime) {
        this.StartTime = startTime;
        this.EndTime = endTime;
    }

    public TimeRange(DateTime triggerTime) {
        this.StartTime = triggerTime;
        this.EndTime = triggerTime;
    }
}


public abstract class GameEvent: ICanGetSystem, ICanGetModel, ICanSendEvent, ICanRegisterEvent {
    
    public abstract GameEventType GameEventType { get; }
    
    
    [field: ES3Serializable]
    public EventState EventState { get; set; } = EventState.NotStart;
    
    public abstract float TriggerChance { get; }
    //public abstract Func<bool> TriggerCondition { get; }

    protected GameTimeManager gameTimeManager;

    [field: ES3Serializable]
    public TimeRange StartTimeRange { get; }

    protected GameEventSystem gameEventSystem;
    protected GameStateModel gameStateModel;

    [field: ES3Serializable]
    public bool LockedTime = false;
    public GameEvent(TimeRange startTimeRange): this() {
        this.StartTimeRange = startTimeRange;
    }

    public GameEvent() {
        gameTimeManager = this.GetSystem<GameTimeManager>();
        gameEventSystem = this.GetSystem<GameEventSystem>();
        gameStateModel = this.GetModel<GameStateModel>();
    }

    public void Start() {
        gameTimeManager.LockDayEnd.Retain();
        LockedTime = true;
        OnStart();
    }

    public EventState Update() {
        return OnUpdate();
    }

    public void End() {
        if (LockedTime) {
            gameTimeManager.LockDayEnd.Release();
        }
       
        OnEnd();
    }

    public void Miss() {
        if (LockedTime) {
            gameTimeManager.LockDayEnd.Release();
        }
        OnMissed();
    }
    
    public abstract void OnStart();
    public abstract EventState OnUpdate();
    public abstract void OnEnd();

    public abstract void OnMissed();

    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
