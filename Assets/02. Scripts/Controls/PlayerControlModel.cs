using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

public enum PlayerControlType {
	Normal,
	Screenshot
}
public class PlayerControlModel : AbstractModel {
	public BindableProperty<PlayerControlType> ControlType = new BindableProperty<PlayerControlType>(global::PlayerControlType.Normal);
	
	protected override void OnInit() {
		
	}
}
