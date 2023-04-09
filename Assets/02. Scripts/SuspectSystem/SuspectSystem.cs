using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.GameTime;
using _02._Scripts.SuspectSystem;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

public class SuspectSystem : AbstractSystem {
	protected SuspectModel model;
	protected BodyModel bodyModel;
	protected override void OnInit() {
		model = this.GetModel<SuspectModel>();
		bodyModel = this.GetModel<BodyModel>();
		this.RegisterEvent<OnBodyInfoKilled>(OnBodyKilled);
		this.RegisterEvent<OnNewDay>(OnNewDay);
	}

	private void OnNewDay(OnNewDay e) {
		if (e.Day == 1) {
			DateTime firstEventTime = this.GetModel<GameTimeModel>().GetDay(Random.Range(6, 10));
			firstEventTime = firstEventTime.AddMinutes(Random.Range(20, 50));
			this.GetSystem<GameEventSystem>().AddEvent(new PoliceNotifySuspectEvent(
				new TimeRange(firstEventTime, firstEventTime.AddMinutes(40)), PoliceGenerateEvent.GeneratePolice()));

			this.GetSystem<TelephoneSystem>().AddContact("611", new CrimialReportHotline());
		}
	}

	private void OnBodyKilled(OnBodyInfoKilled e) {
		model.RemoveBody(e.ID);
		
	}
}
