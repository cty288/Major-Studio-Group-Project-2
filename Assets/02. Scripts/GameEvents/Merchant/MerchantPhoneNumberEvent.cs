using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;



public struct MyEvent {
	public int Name;
	public string ABaba;

	public MyEvent(int name, string ababa) {
		this.Name = name;
		this.ABaba = ababa;
	}
}
	
public class MerchantPhoneNumberEvent : GameEvent{

   public MerchantPhoneNumberEvent(TimeRange startTimeRange) : base(startTimeRange) {
   }

   [field: ES3Serializable]
   public override GameEventType GameEventType { get; } = GameEventType.General;
   [field: ES3Serializable]
   public override float TriggerChance { get; } = 1;
   public override void OnStart() {
      
   }

   
   public MerchantPhoneNumberEvent(): base(){}
   public override EventState OnUpdate() {
	  // this.SendEvent<MyEvent>(new MyEvent("eee", 100));
	   
	   this.SendEvent<MerchantPhoneNumberEvent>(this);
       return EventState.End;
   }

   public override void OnEnd() {
       
   }

   public override void OnMissed() {
       
   }
}


