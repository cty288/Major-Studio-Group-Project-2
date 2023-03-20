using System;
using MikroFramework.Architecture;

namespace _02._Scripts.GameEvents.Camera
{
    public struct OnCameraReceive
    {
        
    }
    public class ReceiveCamera : GameEvent {
   
        public override GameEventType GameEventType { get; } = GameEventType.BountyHunterQuestClueNotification;

        private bool isCompleted = false;
        public override float TriggerChance {
            get
            {
                return 1;
            }
        }

        public ReceiveCamera(TimeRange startTimeRange) : base(startTimeRange) {
            isCompleted = false;
        }

        public override void OnStart() {
            this.SendEvent<OnCameraReceive>();
        }

        public override EventState OnUpdate() {
            return EventState.End;
        }

        public override void OnEnd() {
        }

        public override void OnMissed() {
            DateTime time = gameTimeManager.CurrentTime.Value;
        
            gameEventSystem.AddEvent(new ReceiveCamera(new TimeRange(time.AddSeconds(1), time.AddSeconds(1))));
        }
    }
}