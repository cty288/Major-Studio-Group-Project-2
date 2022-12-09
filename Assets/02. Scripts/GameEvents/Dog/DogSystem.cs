using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

public struct OnDogDie {
    
}
public class DogSystem : AbstractSystem
{
    private GameTimeManager gameTimeManager;
    private GameEventSystem gameEventSystem;
    public bool isDogAlive { get; private set; } = true;
    public bool HaveDog { get; private set; } = false;
  
    
    protected override void OnInit() {
        gameTimeManager = this.GetSystem<GameTimeManager>();
        gameEventSystem = this.GetSystem<GameEventSystem>();
        gameTimeManager.OnDayStart += OnEndOfDay;
        isDogAlive = true;
        this.RegisterEvent<OnDogGet>(OnGetDog);
    }

    private void OnGetDog(OnDogGet e) {
        HaveDog = true;
    }

    private void OnEndOfDay(int day) {
        if (day == 1) {
            SpawnDog(Random.Range(2, 5));
        }
    }

    public void KillDog() {
        if (HaveDog) {
            isDogAlive = false;
            this.SendEvent<OnDogDie>();
        }
    }

    private void SpawnDog(int eventStartDay)
    {
        BodyInfo targetBody = DogKnockEvent.GenerateDog();

        int knockDoorTimeInterval = 3;
        int knockTime = Random.Range(6, 9);
        DateTime dogStartTime = gameTimeManager.CurrentTime.Value.AddDays(eventStartDay);
        int hour = Random.Range(22, 24);
        int minute = Random.Range(20, 40);

        dogStartTime = new DateTime(dogStartTime.Year, dogStartTime.Month, dogStartTime.Day, hour, minute, 0);

        DateTime dogEventEndTime = dogStartTime.AddMinutes(Random.Range(20, 40));

        Debug.Log("Dog Event Start Time: " + dogStartTime);
        gameEventSystem.AddEvent(new DogKnockEvent(
            new TimeRange(dogStartTime, dogEventEndTime), targetBody, knockDoorTimeInterval,
            knockTime,
            1, null, null));
    }

   
}
