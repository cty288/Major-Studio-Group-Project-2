using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using MikroFramework.Event;
using UnityEngine;

class AvailabilityUnRegister : IUnRegister
{
	public Action<IPlace, bool> Target { get; set; }

	public Action<IPlace, bool> OnValueChanged { get; set; }

	public AvailabilityUnRegister(Action<IPlace, bool> target, Action<IPlace, bool> onValueChanged)
	{
		this.Target = target;
		this.OnValueChanged = onValueChanged;
	}

	public void UnRegister()
	{
		Target -= OnValueChanged;
	}

}

class ActivityAvailabilityUnRegister : IUnRegister
{
	public Action<IPlace, IActivity, bool> Target { get; set; }

	public Action<IPlace, IActivity, bool> OnValueChanged { get; set; }

	public ActivityAvailabilityUnRegister(Action<IPlace, IActivity, bool> target, Action<IPlace, IActivity, bool> onValueChanged)
	{
		this.Target = target;
		this.OnValueChanged = onValueChanged;
	}

	public void UnRegister()
	{
		Target -= OnValueChanged;
	}

}
public interface IPlace : ICanGetModel {
	bool IsAvailable { get; }
	public string Name { get; }
	public string DisplayName { get; }
	public string Description { get; }

	public void Init();
	
	public BindableProperty<bool> InteractedBefore { get; }

	public void EnablePermanentActivity(string activityName);
	public void EnableTemporaryActivity(string activityName, DateTime endTime);
	public void DisableActivity(string activityName);
	
	public void RegisterActivity(IActivity activity);
	
	public void OnPlaceAvailable();
	public void OnPlaceUnavailable();
	public void OnDayStart(DateTime day);

	public IUnRegister RegisterOnPlaceAvailableChanged(Action<IPlace, bool> onAvailableChanged);
	public void UnRegisterOnPlaceAvailableChanged(Action<IPlace, bool> onAvailableChanged);

	public IUnRegister RegisterOnActivityAvailableChanged(
		Action<IPlace, IActivity, bool> onActivityAvailableChanged);

	public void UnRegisterOnActivityAvailableChanged(
		Action<IPlace, IActivity, bool> onActivityAvailableChanged);

	public TActivity GetActivity<TActivity>() where TActivity : class;

	public List<IActivity> GetEnabledActivities();
}

[ES3Serializable]
public abstract class Place: ICanGetModel, IPlace, ICanGetSystem {
	public bool IsAvailable {
		get {
			return Available.Value;
		}
	}

	[field: ES3Serializable]
	public abstract string Name { get; }

	public virtual string DisplayName {
		get {
			return outdoorActivityModel.GetPlaceDescription(Name).DisplayName;
		}
	}

	public virtual string Description {
		get {
			return outdoorActivityModel.GetPlaceDescription(Name).Description;
		}
	}

	[field: ES3Serializable]
	public BindableProperty<bool> InteractedBefore { get; protected set; } = new BindableProperty<bool>(false);


	#region Set On Init()

	protected OutdoorActivityModel outdoorActivityModel;
	protected OutdoorActivitySystem outdoorActivitySystem;
	protected Action<IPlace, bool> onAvailableChanged;
	protected Action<IPlace, IActivity, bool> onActivityAvailableChanged;

	#endregion

	#region Savable Datas
	[ES3Serializable]
	protected Dictionary<string, IActivity> registeredActivities = new Dictionary<string, IActivity>();
	[ES3Serializable]
	protected Dictionary<DateTime, HashSet<string>> enabledActivities = new Dictionary<DateTime, HashSet<string>>(); //reference to registered acti

	[field: ES3Serializable]
	protected BindableProperty<bool> Available { get; set; } = new BindableProperty<bool>(false);
	#endregion
	

	
	public Place() {
		//Init();
	}
	

	public void Init() {
		outdoorActivityModel = this.GetModel<OutdoorActivityModel>();
		this.GetSystem<OutdoorActivitySystem>(system => {
			outdoorActivitySystem = system;
		});
		if(registeredActivities.Count == 0) {
			OnRegisterActivities();
		}
		else {
			foreach (IActivity activity in registeredActivities.Values) {
				activity.Init();
				//activity.IsAvailable.RegisterOnValueChaned(OnActivityAvailableChanged);
			}
		}

		Available.RegisterOnValueChaned(OnAvailableChanged);
		OnInit();
	}
	
	public List<IActivity> GetEnabledActivities() {
		List<IActivity> enabledActivities = new List<IActivity>();
		foreach (var activity in registeredActivities.Values) {
			if (activity.IsAvailable) {
				enabledActivities.Add(activity);
			}
		}

		return enabledActivities;
	}



	private void OnAvailableChanged(bool available) {
		onAvailableChanged?.Invoke(this, available);
	}

	protected IActivity GetActivity(string activityName) {
		if (registeredActivities.ContainsKey(activityName)) {
			return registeredActivities[activityName];
		}
		else {
			return null;
		}
	}
	
	protected void DisableActivity(IActivity activity) {
		foreach (var enabledActivity in enabledActivities) {
			if (enabledActivity.Value.Contains(activity.Name)) {
				enabledActivity.Value.Remove(activity.Name);
				if (activity.IsAvailable) {
					activity.IsAvailable.Value = false;
					activity.OnActivityUnavailable();
					onActivityAvailableChanged?.Invoke(this, activity, false);
				}
				
				return;
			}
		}
	}
	
	public void EnablePermanentActivity(string activityName){
		DateTime endTime = DateTime.MaxValue;
		EnableTemporaryActivity(activityName, endTime);
	}

	
	public void EnableTemporaryActivity(string activityName, DateTime endTime) {
		IActivity activity = GetActivity(activityName);
		if (activity != null) {
			if (activity.IsAvailable) {
				return;
			}
			
			if (!enabledActivities.ContainsKey(endTime)) {
				enabledActivities.Add(endTime, new HashSet<string>());
			}
			
			
			enabledActivities[endTime].Add(activity.Name);
			activity.IsAvailable.Value = true;
			activity.OnActivityAvailable();
			onActivityAvailableChanged?.Invoke(this, activity, true);
		}
	}

	public void DisableActivity(string activityName) {
		IActivity activity = GetActivity(activityName);
		if (activity != null) {
			DisableActivity(activity);
		}
	}
	
	

	public void RegisterActivity(IActivity activity)  {
		if (!registeredActivities.ContainsKey(activity.Name)) {
			//activity.SetPlace(this as T);
			registeredActivities.Add(activity.Name, activity );
			activity.Init();
		}
	}
	
	

	public void OnPlaceAvailable() {
		Available.Value = true;
		OnAvailable();
	}

	public void OnPlaceUnavailable() {
		Available.Value = false;
		OnUnavailable();
	}

	public void OnDayStart(DateTime day) {
		DateTime yesterday = day.AddDays(-1);
		OnDayEnd(yesterday);
		
		HashSet<string> activitiesToDisable = null;
		if (enabledActivities.ContainsKey(day)) {
			activitiesToDisable = enabledActivities[day];
			foreach (string activityName in activitiesToDisable) {
				DisableActivity(activityName);
			}

			activitiesToDisable.Clear();
		}

		foreach (KeyValuePair<string, IActivity> registeredActivity in registeredActivities) {
			registeredActivity.Value.OnDayStart(day);
		}
		
		OnDayStarts(day);
	}

	public void OnDayEnd(DateTime day) {
		foreach (KeyValuePair<string,IActivity> registeredActivity in registeredActivities) {
			registeredActivity.Value.OnDayEnd(day);
		}
		OnDayEnds(day);
	}

	public IUnRegister RegisterOnPlaceAvailableChanged(Action<IPlace, bool> onAvailableChanged) {
		this.onAvailableChanged += onAvailableChanged;
		return new AvailabilityUnRegister(this.onAvailableChanged, onAvailableChanged);
	}

	public void UnRegisterOnPlaceAvailableChanged(Action<IPlace, bool> onAvailableChanged) {
		this.onAvailableChanged -= onAvailableChanged;
	}

	public IUnRegister RegisterOnActivityAvailableChanged(Action<IPlace, IActivity, bool> onActivityAvailableChanged) {
		this.onActivityAvailableChanged += onActivityAvailableChanged;
		return new ActivityAvailabilityUnRegister(this.onActivityAvailableChanged, onActivityAvailableChanged);
	}

	public void UnRegisterOnActivityAvailableChanged(Action<IPlace, IActivity, bool> onActivityAvailableChanged) {
		this.onActivityAvailableChanged -= onActivityAvailableChanged;
	}

	public TActivity GetActivity<TActivity>() where TActivity : class{
		foreach (IActivity activity in registeredActivities.Values) {
			if (activity is TActivity) {
				return (TActivity) activity;
			}
		}

		return null;
	}


	public IArchitecture GetArchitecture() {
		return MainGame.Interface;
	}

	protected abstract void OnRegisterActivities();
	protected abstract void OnInit();
	
	protected abstract void OnAvailable();
	protected abstract void OnUnavailable();
	
	protected abstract void OnDayStarts(DateTime day);
	protected abstract void OnDayEnds(DateTime day);
}
