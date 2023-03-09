using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.DataStructures;
using UnityEngine;

public class OutdoorActivitySystem : AbstractSavableSystem {
  
   protected Dictionary<string, IPlace> registeredPlaces { get; set; } = new Dictionary<string, IPlace>();

   protected GameTimeModel gameTimeModel;
   protected OutdoorActivityModel outdoorActivityModel;
   protected GameTimeManager gameTimeManager;


   public override void OnLoad() {
      base.OnLoad();
      if (!ES3.FileExists("systems.es3")) {
         return;
      }
      registeredPlaces = ES3.Load<Dictionary<string, IPlace>>("registeredPlaces", "systems.es3");
      foreach (IPlace place in registeredPlaces.Values) {
         place.Init();
      }
   }

   public override void OnSave() {
      base.OnSave();
      ES3.Save("registeredPlaces", registeredPlaces, "systems.es3");
      
   }

   protected override void OnInit() {
      base.OnInit();
      this.RegisterEvent<OnNewDay>(OnNewDay);
      outdoorActivityModel = this.GetModel<OutdoorActivityModel>();
      gameTimeManager = this.GetSystem<GameTimeManager>();
      if (registeredPlaces.Count == 0) {
         RegisterPlace(new TestArea(), false);
      }
   }
   
   public T GetPlace<T>() where T : class, IPlace {
      foreach (IPlace place in registeredPlaces.Values) {
         if (place is T) {
            return (T) place;
         }
      }

      return null;
   }
   
   public IPlace GetPlace(Type type){
      foreach (IPlace place in registeredPlaces.Values) {
         if (place.GetType() == type) {
            return place;
         }
      }

      return null;
   }

   public IPlace GetPlace(string name) {
      if (registeredPlaces.ContainsKey(name)) {
         return registeredPlaces[name];
      }

      return null;
   }

   private void OnNewDay(OnNewDay e) {
      foreach (KeyValuePair<string,IPlace> registeredPlace in registeredPlaces) {
         registeredPlace.Value.OnDayStart(e.Date.Date);
      }
   }


   public void RegisterPlace(IPlace place, bool startAvailable) {
      if (!registeredPlaces.ContainsKey(place.Name)) {
         registeredPlaces.Add(place.Name, place);
         place.Init();
         if (startAvailable) {
            SetPlaceAvailable(place.Name, true);
         }
      }
   }

   public void EnterActivity(IActivity activity) {
      activity.OnPlayerEnter();
      gameTimeManager.LockOutdoorEventEnd.Retain();
   }
   
   public void ExitActivity(IActivity activity) {
      activity.OnPlayerLeave();
      gameTimeManager.LockOutdoorEventEnd.Release();
   }
   
   public void SetPlaceAvailable(string placeName, bool available) {
      if (registeredPlaces.ContainsKey(placeName)) {
         IPlace place = registeredPlaces[placeName];
         if (available) {
            place.OnPlaceAvailable();
         }
         else {
            place.OnPlaceUnavailable();
         }
      }
   }
   
   public void EnableActivity(string placeName, string activityName,DateTime endTime) {
      if (registeredPlaces.ContainsKey(placeName)) {
         IPlace place = registeredPlaces[placeName];
         place.EnableTemporaryActivity(activityName, endTime);
      }
   }
   
   
}
