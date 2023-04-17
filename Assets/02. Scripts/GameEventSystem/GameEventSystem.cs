using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crosstales;
using MikroFramework.Architecture;
using NHibernate.Mapping;
using Unity.VisualScripting;
using UnityEngine;
using Array = System.Array;
using Random = UnityEngine.Random;


public class GameEventSystemUpdater : MonoBehaviour {
    public Action OnUpdate;

    private void Update() {
        OnUpdate?.Invoke();
    }
}

public enum GameEventType {
    Radio,
    BodyGeneration,
    General,
    IncomingCall,
    BountyHunterQuestClueNotification,
    BountyHunterQuestClue
}
public class GameEventSystem : AbstractSavableSystem {
    //[field: ES3Serializable]
    //[ES3NonSerializable]
    public Dictionary<DateTime, List<GameEvent>> EventDict { get; private set; } = new Dictionary<DateTime, List<GameEvent>>();
    //[field: ES3Serializable]
    //[ES3NonSerializable]
    public Dictionary<GameEventType, List<GameEvent>> AllPossibleEvents { get; private set; } = new Dictionary<GameEventType, List<GameEvent>>();
    

    private GameTimeManager gameTimeManager;

    private GameEventSystemUpdater updater;
    
    
    //[field: ES3Serializable]
    private Dictionary<GameEventType, List<GameEvent>> currentEvents = new Dictionary<GameEventType, List<GameEvent>>();

    private Array gameEventTypeValues = Enum.GetValues(typeof(GameEventType));

    public override void OnSave() {
        base.OnSave();
        
        ES3.Save("eventDict", EventDict, "systems.es3");
        ES3.Save("currentEvents", currentEvents, "systems.es3");
        ES3.Save("allPossibleEvents", AllPossibleEvents, "systems.es3");
    }

    public override void OnLoad() {
        base.OnLoad();
        if (!ES3.FileExists("systems.es3")) {
            return;
        }
        EventDict = ES3.Load<Dictionary<DateTime, List<GameEvent>>>("eventDict", "systems.es3");
        currentEvents = ES3.Load<Dictionary<GameEventType, List<GameEvent>>>("currentEvents", "systems.es3");
        AllPossibleEvents = ES3.Load<Dictionary<GameEventType, List<GameEvent>>>("allPossibleEvents", "systems.es3");
        
    }


    protected override void OnInit() {
        gameTimeManager = this.GetSystem<GameTimeManager>();
        gameTimeManager.CurrentTime.RegisterOnValueChaned(OnTimeChanged);
        
        updater = new GameObject("GameEventSystemUpdater").AddComponent<GameEventSystemUpdater>();
        updater.OnUpdate += Update;

        //GameEventType[] eventTypes = Enum.GetValues(typeof(GameEventType)).Cast<GameEventType>().ToArray();
        foreach (object value in gameEventTypeValues) {
            if (!currentEvents.ContainsKey((GameEventType) value)) {
                currentEvents.Add((GameEventType) value, new List<GameEvent>());
            }

            if (!AllPossibleEvents.ContainsKey((GameEventType) value)) {
                AllPossibleEvents.Add((GameEventType) value, new List<GameEvent>());
            }
        }
    }

    private void Update() {
        foreach (GameEventType eventType in gameEventTypeValues) {
            List<GameEvent> currentEventList = currentEvents[eventType];
            
            if (currentEventList.Count > 0) {
                List<GameEvent> toRemove = new List<GameEvent>();
                foreach (GameEvent gameEvent in currentEventList) {
                    EventState eventState = gameEvent.Update();
                    if (eventState == EventState.NotStart) {
                        currentEvents[eventType] = null;
                        AllPossibleEvents[eventType].Add(gameEvent);
                        AllPossibleEvents[eventType].CTShuffle();
                    }
                    else if (eventState == EventState.End)
                    {
                        gameEvent.End();
                        toRemove.Add(gameEvent);
                        // AllPossibleEvents.Remove(currentEvent);
                    }else if (eventState == EventState.Missed) {
                        gameEvent.Miss();
                        toRemove.Add(gameEvent);
                    }
                }
                
                foreach (GameEvent gameEvent in toRemove) {
                    currentEventList.Remove(gameEvent);
                }
               
            }
            
            
            if (AllPossibleEvents[eventType].Any()) {
                GameEvent ev = AllPossibleEvents[eventType][0];
                if (Random.Range(0, 1f) <= ev.TriggerChance) {
                    if (currentEvents[eventType].Count == 0 || ev.CanStartWithSameType) {
                        currentEvents[eventType].Add(ev);
                        AllPossibleEvents[eventType].RemoveAt(0);
                        ev.Start();
                    }
                }
                else {
                    AllPossibleEvents[eventType].RemoveAt(0);
                    ev.Miss();
                }
            }
        }
       
    }

    private void OnTimeChanged(DateTime oldTime, DateTime newTime) {
        //get all times in minutes between oldTime and newTime
        List<DateTime> times = new List<DateTime>();
        if (oldTime.Day != newTime.Day) {
            times.Add(newTime);
        }
        else {
            for (DateTime time = oldTime.AddMinutes(1); time <= newTime; time = time.AddMinutes(1)) {
                times.Add(time);
            }
        }
        

        foreach (DateTime time in times) {
            if (EventDict.ContainsKey(time)) {
                List<GameEvent> evs = EventDict[time];
                EventDict.Remove(newTime);
                foreach (GameEvent ev in evs) {
                    AllPossibleEvents[ev.GameEventType].Add(ev);
                }
           
            }
        }
        //HashSet<GameEvent> removedEvents = new HashSet<GameEvent>();
        foreach (List<GameEvent> events in AllPossibleEvents.Values) {
            events.RemoveAll((ev => {
                if (ev.StartTimeRange.EndTime < newTime) {
                    ev.Miss();
                   // removedEvents.Add(ev);
                    return true;
                }
                return false;
            }));
        }
      
        
      
        
        
        foreach (List<GameEvent> gameEvents in AllPossibleEvents.Values) {
            gameEvents.CTShuffle();
        }
       
    }

    public void AddEvent(GameEvent ev, bool nightEventOnly = true) {
        if (ev == null) return;
        TimeRange startTimeRange = ev.StartTimeRange;
        if (startTimeRange.EndTime < gameTimeManager.CurrentTime.Value) {
            ev.Miss();
            return;
        }

        if (nightEventOnly && startTimeRange.StartTime.Hour < gameTimeManager.NightTimeStart) {
            DateTime gameStartTime = new DateTime(startTimeRange.StartTime.Year, startTimeRange.StartTime.Month,
                startTimeRange.StartTime.Day, gameTimeManager.NightTimeStart, 0, 0);

            int minuteInterval = Random.Range(8, 15);
            startTimeRange.StartTime = gameStartTime.AddMinutes(minuteInterval);
            startTimeRange.EndTime = startTimeRange.StartTime.AddMinutes(5);
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
