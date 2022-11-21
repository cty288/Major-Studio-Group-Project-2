using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEditor.PackageManager;
using UnityEngine;


public enum EventState {
    NotStart,
    Running,
    End
}

public class TimeRange {
    public DateTime StartTime { get; set; }
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


public abstract class GameEvent: ICanGetSystem, ICanGetModel {
    public abstract GameEventType GameEventType { get; }
    public EventState EventState { get; set; } = EventState.NotStart;
    public abstract float TriggerChance { get; }
    //public abstract Func<bool> TriggerCondition { get; }

    protected GameTimeManager gameTimeManager;

    public TimeRange StartTimeRange { get; }

    protected GameEventSystem gameEventSystem;
    protected GameStateModel gameStateModel;
    public GameEvent(TimeRange startTimeRange) {
        gameTimeManager = this.GetSystem<GameTimeManager>();
        this.StartTimeRange = startTimeRange;
        gameEventSystem = this.GetSystem<GameEventSystem>();
        gameStateModel = this.GetModel<GameStateModel>();
    }

    public void Start() {
        gameTimeManager.LockDayEnd.Retain();
        OnStart();
    }

    public EventState Update() {
        return OnUpdate();
    }

    public void End() {
        gameTimeManager.LockDayEnd.Release();
        OnEnd();
    }
    
    public abstract void OnStart();
    public abstract EventState OnUpdate();
    public abstract void OnEnd();

    public abstract void OnMissed();

    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
