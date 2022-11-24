using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class GetResourceEvent : GameEvent {
    private IPlayerResource resource;
    private int count = 1;
    public GetResourceEvent(IPlayerResource resource, int count, TimeRange startTimeRange) : base(startTimeRange) {
        this.resource = resource;
        this.count = count;
    }

    public override GameEventType GameEventType { get; } = GameEventType.General;
    public override float TriggerChance { get; } = 1;
    public override void OnStart() {
        
    }

    public override EventState OnUpdate() {
        this.GetSystem<PlayerResourceSystem>().AddResource(resource, count);
        Debug.Log("Get Resource");
        return EventState.End;
    }

    public override void OnEnd() {
      
    }

    public override void OnMissed() {
      
    }
}
