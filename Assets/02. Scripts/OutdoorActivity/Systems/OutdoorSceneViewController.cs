using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.ResKit;
using UnityEngine;

public class OutdoorSceneViewController : AbstractMikroController<MainGame> {
	protected ResLoader resloader;
	protected Transform sceneSpawnPoint;
	private void Awake() {
		this.RegisterEvent<OnOutdoorActivityChanged>(OnOutdoorActivityChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
		resloader = this.GetUtility<ResLoader>();
		sceneSpawnPoint = transform.Find("Scene");
	}

	private void OnOutdoorActivityChanged(OnOutdoorActivityChanged e) {
		GameObject prefab = resloader.LoadSync<GameObject>("general", e.activity.SceneAssetName);
		if (prefab) {
			GameObject scene = Instantiate(prefab, sceneSpawnPoint);
			IActivityViewController activityViewController = scene.GetComponent<IActivityViewController>();
			activityViewController.SetPlace(e.place);
		}
	}
}
