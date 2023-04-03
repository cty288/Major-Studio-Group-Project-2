using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.SexyCard;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.EventSystems;

public class SexyCardUnderDoorViewController : AbstractMikroController<MainGame>, IPointerClickHandler {
	private void Awake() {
		this.RegisterEvent<OnSexyCardDelivered>(OnSexyCardDelivered).UnRegisterWhenGameObjectDestroyed(gameObject);
		this.gameObject.SetActive(false);
	}

	private void OnSexyCardDelivered(OnSexyCardDelivered e) {
		this.gameObject.SetActive(true);
	}

	public void OnPointerClick(PointerEventData eventData) {
		
	}
}
