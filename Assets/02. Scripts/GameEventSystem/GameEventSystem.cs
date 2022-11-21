using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crosstales;
using MikroFramework.Architecture;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameEventSystemUpdater : MonoBehaviour {
    public Action OnUpdate;

    private void Update() {
        OnUpdate?.Invoke();
    }
}

public enum GameEventType {
    Radio
}
public class GameEventSystem : AbstractSystem {

    public Dictionary<DateTime, List<GameEvent>> EventDict { get; } = new Dictionary<DateTime, List<GameEvent>>();

    public Dictionary<GameEventType, List<GameEvent>> AllPossibleEvents = new Dictionary<GameEventType, List<GameEvent>>();
    

    private GameTimeManager gameTimeManager;

    private GameEventSystemUpdater updater;

    private Dictionary<GameEventType, GameEvent> currentEvents = new Dictionary<GameEventType, GameEvent>();
    protected override void OnInit() {
        gameTimeManager.CurrentTime.RegisterOnValueChaned(OnTimeChanged);
        
        updater = new GameObject("GameEventSystemUpdater").AddComponent<GameEventSystemUpdater>();
        updater.OnUpdate += Update;

        //GameEventType[] eventTypes = Enum.GetValues(typeof(GameEventType)).Cast<GameEventType>().ToArray();
        foreach (object value in Enum.GetValues(typeof(GameEventType))) {
            currentEvents.Add((GameEventType) value, null);
            AllPossibleEvents.Add((GameEventType) value, new List<GameEvent>());
        }
    }

    private void Update() {
        foreach (GameEventType eventType in currentEvents.Keys) {
            GameEvent currentEvent = currentEvents[eventType];
            
            if (currentEvents[eventType]  != null) {
                EventState eventState = currentEvent.Update();
                if (eventState == EventState.NotStart) {
                    currentEvents[eventType] = null;
                    AllPossibleEvents[eventType].Add(currentEvent);
                    AllPossibleEvents[eventType].CTShuffle();
                }
                else if (eventState == EventState.End)
                {
                    currentEvent.OnEnd();
                    currentEvents[eventType] = null;
                    // AllPossibleEvents.Remove(currentEvent);
                }
            }

            if (currentEvents[eventType] == null) {
                if (AllPossibleEvents[eventType].Any()) {
                    GameEvent ev = AllPossibleEvents[eventType][0];
                    AllPossibleEvents[eventType].RemoveAt(0);
                    if (Random.Range(0, 1f) <= ev.TriggerChance)
                    {
                        currentEvents[eventType] = ev;
                        ev.OnStart();
                    }
                    else {
                        ev.OnMissed();
                    }
                }
            }
        }
       
    }

    private void OnTimeChanged(DateTime oldTime, DateTime newTime) {
        foreach (List<GameEvent> events in AllPossibleEvents.Values) {
            events.RemoveAll((ev => {
                if (ev.StartTimeRange.EndTime < newTime) {
                    ev.OnMissed();
                    return true;
                }
                return false;
            }));
        }
      
        
        if (EventDict.ContainsKey(newTime)) {
            List<GameEvent> evs = EventDict[newTime];
            EventDict.Remove(newTime);
            foreach (GameEvent ev in evs) {
                AllPossibleEvents[ev.GameEventType].Add(ev);
            }
           
        }

        foreach (List<GameEvent> gameEvents in AllPossibleEvents.Values) {
            gameEvents.CTShuffle();
        }
       
    }

    public void AddEvent(GameEvent ev) {
        TimeRange startTimeRange = ev.StartTimeRange;
        if (startTimeRange.EndTime < gameTimeManager.CurrentTime.Value) {
            ev.OnMissed();
            return;
        }

        if (!EventDict.ContainsKey(startTimeRange.StartTime)) {
            EventDict.Add(startTimeRange.StartTime, new List<GameEvent>() {ev});
        }else {
            EventDict[startTimeRange.StartTime].Add(ev);
        }

        if (startTimeRange.StartTime <= gameTimeManager.CurrentTime.Value) {
            AllPossibleEvents[ev.GameEventType].Add(ev);
        }
        
    }
}
