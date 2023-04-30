using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

public enum PlayerControlType {
	Normal,
	Screenshot,
	BountyHunting,
	Choosing
}

public struct OnAddControlType {
	public PlayerControlType controlType;
}

public struct OnRemoveControlType {
	public PlayerControlType controlType;
}

public class PlayerControlModel : AbstractModel {
	protected HashSet<PlayerControlType> currentControlTypes = new HashSet<PlayerControlType>()
		{PlayerControlType.Normal};
	
	protected override void OnInit() {
		currentControlTypes.Clear();
		currentControlTypes.Add(PlayerControlType.Normal);
	}
	
	public void AddControlType(PlayerControlType type) {
		currentControlTypes.Add(type);
		currentControlTypes.Remove(PlayerControlType.Normal);
		this.SendEvent<OnAddControlType>(new OnAddControlType(){controlType = type});
	}
	
	public void RemoveControlType(PlayerControlType type) {
		currentControlTypes.Remove(type);
		if (currentControlTypes.Count == 0) {
			currentControlTypes.Add(PlayerControlType.Normal);
		}
		this.SendEvent<OnRemoveControlType>(new OnRemoveControlType(){controlType = type});
	}
	
	public bool HasControlType(PlayerControlType type) {
		return currentControlTypes.Contains(type);
	}
	
	public bool IsNormal() {
		return currentControlTypes.Contains(PlayerControlType.Normal);
	}
}
