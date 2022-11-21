using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public struct OnRadioEnd {

}
public abstract class RadioEvent : GameEvent, ICanGetModel, ICanSendEvent {
    public override GameEventType GameEventType { get; } = GameEventType.Radio;
    protected GameStateModel gameStateModel;
    protected RadioEvent(TimeRange startTimeRange) : base(startTimeRange) {
        gameStateModel = this.GetModel<GameStateModel>();
    }

    public override EventState OnUpdate() {
        DateTime currentTime = gameTimeManager.CurrentTime.Value;
        if ((currentTime.Hour == 23 && currentTime.Minute >= 50) || gameStateModel.GameState.Value == GameState.End) {
            EndRadio();
            return EventState.End;
        }

        return EventState.Running;
    }

    protected void EndRadio() {
        this.SendEvent<OnRadioEnd>();
    }
}


