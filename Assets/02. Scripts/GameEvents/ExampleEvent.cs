
using System;
using _02._Scripts.GameEvents.BountyHunter;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using NHibernate.Dialect.Schema;
using UnityEngine;

public struct OnOpenNewspepr {
	
}
public class ExampleEvent : GameEvent {
	public override GameEventType GameEventType { get; } = GameEventType.BountyHunterQuestClueNotification;

	private bool isCompleted = false;
	public override float TriggerChance {
		get {
			if (this.GetModel<BountyHunterModel>().IsInJail) {
				return 0;
			}

			return 1;
		}
	}

	public ExampleEvent(TimeRange startTimeRange) : base(startTimeRange) {
		isCompleted = false;
	}

	public override void OnStart() {
		
		this.RegisterEvent<OnNewspaperUIPanelOpened>(OnOpen);
	}

	private void OnOpen(OnNewspaperUIPanelOpened e) {
		isCompleted = true;
	}

	public override EventState OnUpdate() {
		if (!isCompleted) {
			return EventState.Running;
		}

		return EventState.End;
	}
	
	

	public override void OnEnd() {
		this.UnRegisterEvent<OnNewspaperUIPanelOpened>(OnOpen);
		DateTime time = gameTimeManager.CurrentTime.Value;


		gameEventSystem.AddEvent(new DailyBodyRadio(new TimeRange(time.AddMinutes(10), time.AddMinutes(10)),
			"aba aba", 1f, Gender.MALE, null));
	}

	public override void OnMissed() {
		this.UnRegisterEvent<OnNewspaperUIPanelOpened>(OnOpen);
		DateTime time = gameTimeManager.CurrentTime.Value;

		gameEventSystem.AddEvent(new ExampleEvent(new TimeRange(time.AddMinutes(10), time.AddMinutes(10))));
	}
}
