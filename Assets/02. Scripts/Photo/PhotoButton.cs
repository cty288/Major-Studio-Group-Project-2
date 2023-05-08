using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Stats;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhotoButton : AbstractMikroController<MainGame>, IPointerClickHandler {
	private GameObject controlHintObj;
	private PhotoSaveModel photoSaveModel;
	private PlayerControlModel playerControlModel;
	private StatsModel statsModel;
	private void Awake() {
		ScreenSpaceImageCropper.Singleton.OnStartCrop += OnStartCrop;
		ScreenSpaceImageCropper.Singleton.OnEndCrop += OnEndCrop;
		ScreenSpaceImageCropper.Singleton.OnDragMouse += OnDragMouse;
		controlHintObj = transform.Find("ControlHint").gameObject;
		photoSaveModel = this.GetModel<PhotoSaveModel>();
	
		playerControlModel = this.GetModel<PlayerControlModel>();
		photoSaveModel.HasCamera.RegisterWithInitValue(OnHasCameraChanged)
			.UnRegisterWhenGameObjectDestroyed(gameObject);
		statsModel = this.GetModel<StatsModel>();
	}

	private void OnHasCameraChanged(bool hasCamera) {
		gameObject.SetActive(hasCamera);
	}


	private void OnDragMouse() {
		this.gameObject.SetActive(false);
	}

	private void OnEndCrop() {
		this.gameObject.SetActive(true);
		controlHintObj.SetActive(false);
	}

	private void OnStartCrop() {
		this.gameObject.SetActive(true);
		controlHintObj.SetActive(true);
	}

	private void OnDestroy() {
		ScreenSpaceImageCropper.Singleton.OnStartCrop -= OnStartCrop;
		ScreenSpaceImageCropper.Singleton.OnEndCrop -= OnEndCrop;
		ScreenSpaceImageCropper.Singleton.OnDragMouse -= OnDragMouse;
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (ScreenSpaceImageCropper.Singleton.IsCropping || !playerControlModel.IsNormal()) {
			return;
		}

		ScreenSpaceImageCropper.Singleton.StartCrop(null, OnFinishCropping);
	}

	private void OnFinishCropping(CropInfo info) {
		//info.
		photoSaveModel.SavePhoto(info);
		statsModel.UpdateStat("PhotoTaken",
			new SaveData("Photos Taken", (int) statsModel.GetStat("PhotoTaken", 0) + 1));

	}
}
