using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class GetResourceEvent : GameEvent {
    [field: ES3Serializable]
    private IPlayerResource resource;
    [field: ES3Serializable]
    private int count = 1;
    public GetResourceEvent(IPlayerResource resource, int count, TimeRange startTimeRange) : base(startTimeRange) {
        this.resource = resource;
        this.count = count;
    }

    public GetResourceEvent() {
        
    }

    [field: ES3Serializable]
    public override GameEventType GameEventType { get; } = GameEventType.General;
    [field: ES3Serializable]
    public override float TriggerChance { get; } = 1;
    public override void OnStart() {
        
    }

    public override EventState OnUpdate() {
        this.GetModel<PlayerResourceModel>().AddResource(resource, count);
        Debug.Log("Get Resource");
        return EventState.End;
    }

    public override void OnEnd() {
      
    }

    public override void OnMissed() {
      
    }
}
