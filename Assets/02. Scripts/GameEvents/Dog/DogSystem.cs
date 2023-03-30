using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.Dog;
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
        this.RegisterEvent<OnNewDay>(OnNewDay);
        this.RegisterEvent<OnDogGet>(OnGetDog);
    }

    private void OnNewDay(OnNewDay e) {
        if (e.Day == 0) {
            GenerateMissingDogContact();
        }
			
			
        if (e.IsNewWeek) {
            if(!dogModel.SentDogBack && dogModel.isDogAlive){
                ImportantNewspaperModel newspaperModel = this.GetModel<ImportantNewspaperModel>();
                newspaperModel.AddPageToNewspaper(newspaperModel.GetWeekForNews(e.Day),
                    new MissingDogImportantNewsContent(dogModel.MissingDogPhoneNumber, dogModel.MissingDogBodyInfo), 1);
            }
				
        }
    }
    
    private void GenerateMissingDogContact() {
        int knockDoorTimeInterval = 3;
        int knockTime = Random.Range(6, 9);
        BodyInfo targetBody = GetMissingDogBodyInfo(knockDoorTimeInterval, knockTime);
        dogModel.MissingDogBodyInfo = targetBody;
        
        dogModel.MissingDogPhoneNumber = PhoneNumberGenor.GeneratePhoneNumber(7);
        TelephoneSystem telephoneSystem = this.GetSystem<TelephoneSystem>();
        //merchantModel.PhoneNumber =  PhoneNumberGenor.GeneratePhoneNumber(7);
        telephoneSystem.AddContact(dogModel.MissingDogPhoneNumber, new MissingDogContact());
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

    private void SpawnDog(int eventStartDay) {
        DateTime dogStartTime = gameTimeManager.CurrentTime.Value.AddDays(eventStartDay);
        int hour = Random.Range(gameTimeManager.NightTimeStart, 24);
        int minute = Random.Range(20, 40);

        dogStartTime = new DateTime(dogStartTime.Year, dogStartTime.Month, dogStartTime.Day, hour, minute, 0);

        DateTime dogEventEndTime = dogStartTime.AddMinutes(Random.Range(20, 40));

        Debug.Log("Dog Event Start Time: " + dogStartTime);
        gameEventSystem.AddEvent(new DogKnockEvent(
            new TimeRange(dogStartTime, dogEventEndTime), dogModel.MissingDogBodyInfo,
            1));
    }


    private BodyInfo GetMissingDogBodyInfo(float knockDoorTimeInterval, int knockTime) {
        HeightType height =HeightType.Short;
        List<GameObject> dogs = AlienBodyPartCollections.Singleton.SpecialBodyPartPrefabs.HeightSubCollections[1]
            .ShadowBodyPartPrefabs.HumanTraitPartsPrefabs;//.GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo();


        BodyPartPrefabInfo dog = dogs[Random.Range(0, dogs.Count)].GetComponent<AlienBodyPartInfo>()
            .GetBodyPartPrefabInfo(0);
        
        return BodyInfo.GetBodyInfo(null, null, dog, height, null,
            new DogKnockBehavior(knockDoorTimeInterval, knockTime, null), BodyPartDisplayType.Shadow, false);

    }

   
}
