using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

public interface IActivity {
    string Name { get; }
    public string DisplayName { get; }
    public string Description { get; }
    
    public string SceneAssetName { get; }
    BindableProperty<bool> IsAvailable { get; set; }
   // void SetPlace(T place);
    void OnDayStart(DateTime date);
    void OnDayEnd(DateTime date);
    void OnPlayerEnter();
    void OnPlayerLeave();
    void OnActivityAvailable();
    void OnActivityUnavailable();

    void Init();
}
[ES3Serializable]
public abstract class Activity : IActivity, ICanGetModel, ICanGetSystem{
    protected OutdoorActivitySystem outdoorActivitySystem;
    
    [field: ES3Serializable]
    public abstract string Name { get; protected set; }

    public virtual string DisplayName {
        get {
            return outdoorActivityModel.GetActivityDescription(Name).DisplayName;
        }
    }

    public virtual string Description {
        get {
            return outdoorActivityModel.GetActivityDescription(Name).Description;
        }
    }
    [field: ES3Serializable]
    public abstract string SceneAssetName { get; }

    [field: ES3Serializable]
    public BindableProperty<bool> IsAvailable { get; set; } = new BindableProperty<bool>(false);

   // protected T Place {
     //   get {
            
     // }
    //}

    protected OutdoorActivityModel outdoorActivityModel;

    public Activity() {
        
    }

    public void Init() {
        outdoorActivityModel = this.GetModel<OutdoorActivityModel>();
        this.GetSystem<OutdoorActivitySystem>((system => {
            this.outdoorActivitySystem = system;
        }));
        OnInit();
    }



    public void OnDayStart(DateTime date) {
        OnDayStarts(date);
    }

    public void OnDayEnd(DateTime date) {
        OnDayEnds(date);
    }

    public void OnPlayerEnter() {
        OnEnterPlayer();
    }

    public void OnPlayerLeave() {
        OnLeavePlayer();
    }

    public void OnActivityAvailable() {
        OnAvailable();
    }

    public void OnActivityUnavailable() {
        OnUnavailable();
    }
    
    protected abstract void OnInit();
    protected abstract void OnDayStarts(DateTime date);
    protected abstract void OnDayEnds(DateTime date);
    protected abstract void OnEnterPlayer();
    protected abstract void OnLeavePlayer();
    protected abstract void OnAvailable();
    protected abstract void OnUnavailable();
    
    
    public IArchitecture GetArchitecture() {
        return MainGame.Interface;
    }
}
    

