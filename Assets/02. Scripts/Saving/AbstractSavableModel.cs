using System.Collections;
using System.Collections.Generic;using MikroFramework.Architecture;
using UnityEngine;

[ES3Serializable]
public abstract class AbstractSavableModel : AbstractModel{
	protected override void OnInit() {
		
	}

	public  void Save() {
		ES3.Save("Model_" + this.GetType().Name, this, "models.es3");
	}
}
