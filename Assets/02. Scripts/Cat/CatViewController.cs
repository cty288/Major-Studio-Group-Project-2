using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Cat;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class CatViewController : AbstractMikroController<MainGame>, IPointerClickHandler {
	private void Awake() {
		this.RegisterEvent<OnCatSpawn>(OnCatSpawn).UnRegisterWhenGameObjectDestroyed(gameObject);
		this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
		gameObject.SetActive(false);
	}

	private void OnNewDay(OnNewDay obj) {
		if (gameObject.activeInHierarchy) {
			if (Random.Range(0f, 1f) < 0.4f) {
				gameObject.SetActive(false);
			}
		}
	}

	private void OnCatSpawn(OnCatSpawn obj) {
		gameObject.SetActive(true);
	}

	public void OnPointerClick(PointerEventData eventData) {
		gameObject.SetActive(false);
		AudioSystem.Singleton.Play2DSound("Cat");
	}
}
