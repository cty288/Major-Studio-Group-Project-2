using System.Collections;
using System.Collections.Generic;using MikroFramework.Architecture;
using UnityEngine;

[ES3Serializable]
public abstract class AbstractSavableSystem : AbstractSystem{
	protected override void OnInit() {
		
	}

	public void Save() {
		ES3.Save("System_" + this.GetType().Name, this, "systems.es3");
		OnSave();
	}
	
	public virtual void OnLoad() {
		
	}
	
	public virtual void OnSave() {
		
	}
}
