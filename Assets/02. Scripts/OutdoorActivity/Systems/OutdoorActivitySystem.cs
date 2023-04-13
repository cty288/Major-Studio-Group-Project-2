using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.DataStructures;
using UnityEngine;


public struct OnOutdoorActivityChanged {
   public IActivity activity;
   public IPlace place;
}

public class OutdoorActivitySystem : AbstractSavableSystem {
  
   protected Dictionary<string, IPlace> registeredPlaces { get; set; } = new Dictionary<string, IPlace>();

   protected GameTimeModel gameTimeModel;
   protected OutdoorActivityModel outdoorActivityModel;
   protected GameTimeManager gameTimeManager;
   protected GameSceneModel gameSceneModel;


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
      this.RegisterEvent<OnEndOfOutdoorDayTimeEvent>(OnEndOfOutdoorDayTimeEvent);
      gameSceneModel = this.GetModel<GameSceneModel>();
      if (registeredPlaces.Count == 0) {
         RegisterPlace(new TestArea(), false);
         RegisterPlace(new HuntingGround(), false);
      }
   }

   private void OnEndOfOutdoorDayTimeEvent(OnEndOfOutdoorDayTimeEvent e) {
      outdoorActivityModel.CurrentActivity.Value = null;
      gameSceneModel.GameScene.Value = GameScene.MainGame;
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

   public void EnterActivity(IActivity activity, IPlace place) {
      activity.OnPlayerEnter();
      gameTimeManager.LockOutdoorEventEnd.Retain();
      outdoorActivityModel.CurrentActivity.Value = activity;
      gameSceneModel.GameScene.Value = GameScene.Outdoor;
      this.SendEvent<OnOutdoorActivityChanged>(new OnOutdoorActivityChanged() {
         activity = activity,
         place = place
      });
   }
   
   public void ExitActivity(IActivity activity) {
      activity.OnPlayerLeave();
      gameTimeManager.LockOutdoorEventEnd.Release();
      outdoorActivityModel.CurrentActivity.Value = null;
      this.SendEvent<OnOutdoorActivityChanged>(new OnOutdoorActivityChanged() {
         activity = null,
         place = null
      });
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
