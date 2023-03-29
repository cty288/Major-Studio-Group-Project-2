using System;
using MikroFramework.Architecture;

namespace _02._Scripts.GameEvents.Camera
{
    public struct OnCameraReceive
    {
        
    }
    
    
    [ES3Serializable]
    public class ReceiveCamera : GameEvent {
   
        [field: ES3Serializable]
        public override GameEventType GameEventType { get; } = GameEventType.BountyHunterQuestClueNotification;
        [field: ES3Serializable]
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