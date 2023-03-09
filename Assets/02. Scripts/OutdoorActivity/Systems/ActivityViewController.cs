using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public abstract class ActivityViewController<TActivity>: AbstractMikroController<MainGame>
where TActivity: class, IActivity {
	
	protected TActivity activity;
	protected IPlace place;
	protected OutdoorActivitySystem outdoorActivitySystem;
	protected virtual void Awake() {
		outdoorActivitySystem = this.GetSystem<OutdoorActivitySystem>();
		//place = outdoorActivitySystem.GetPlace<TPlace>();
		//activity = place.GetActivity<TActivity>();
	}

	public void SetPlace(IPlace place) {
		this.place = place;
		this.activity = this.place.GetActivity<TActivity>();
	}
}
