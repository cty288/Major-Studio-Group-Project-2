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
    private DogModel dogModel;


    protected override void OnInit() {
        gameTimeManager = this.GetSystem<GameTimeManager>();
        gameEventSystem = this.GetSystem<GameEventSystem>();
        dogModel = this.GetModel<DogModel>();
        
        gameTimeManager.OnDayStart += OnEndOfDay;
        
        this.RegisterEvent<OnDogGet>(OnGetDog);
    }

    private void OnGetDog(OnDogGet e) {
        dogModel.HaveDog = true;
    }

    private void OnEndOfDay(int day, int hour) {
        if (day == 1) {
            SpawnDog(Random.Range(2, 5));
        }
    }

    public void KillDog() {
        if (dogModel.HaveDog) {
            dogModel.isDogAlive = false;
            this.SendEvent<OnDogDie>();
        }
    }

    private void SpawnDog(int eventStartDay)
    {
        int knockDoorTimeInterval = 3;
        int knockTime = Random.Range(6, 9);
        BodyInfo targetBody = DogKnockEvent.GenerateDog(knockDoorTimeInterval, knockTime);

       
        DateTime dogStartTime = gameTimeManager.CurrentTime.Value.AddDays(eventStartDay);
        int hour = Random.Range(gameTimeManager.NightTimeStart, 24);
        int minute = Random.Range(20, 40);

        dogStartTime = new DateTime(dogStartTime.Year, dogStartTime.Month, dogStartTime.Day, hour, minute, 0);

        DateTime dogEventEndTime = dogStartTime.AddMinutes(Random.Range(20, 40));

        Debug.Log("Dog Event Start Time: " + dogStartTime);
        gameEventSystem.AddEvent(new DogKnockEvent(
            new TimeRange(dogStartTime, dogEventEndTime), targetBody,
            1));
    }

   
}
