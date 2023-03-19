using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhotoButton : AbstractMikroController<MainGame>, IPointerClickHandler {
	private GameObject controlHintObj;
	private PhotoSaveModel photoSaveModel;
	private PlayerControlModel playerControlModel;
	private void Awake() {
		ScreenSpaceImageCropper.Singleton.OnStartCrop += OnStartCrop;
		ScreenSpaceImageCropper.Singleton.OnEndCrop += OnEndCrop;
		ScreenSpaceImageCropper.Singleton.OnDragMouse += OnDragMouse;
		controlHintObj = transform.Find("ControlHint").gameObject;
		photoSaveModel = this.GetModel<PhotoSaveModel>();
	
		playerControlModel = this.GetModel<PlayerControlModel>();
		photoSaveModel.HasCamera.RegisterWithInitValue(OnHasCameraChanged)
			.UnRegisterWhenGameObjectDestroyed(gameObject);
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
		if (ScreenSpaceImageCropper.Singleton.IsCropping || playerControlModel.ControlType.Value!= PlayerControlType.Normal) {
			return;
		}

		ScreenSpaceImageCropper.Singleton.StartCrop(null, OnFinishCropping);
	}

	private void OnFinishCropping(CropInfo info) {
		//info.
		photoSaveModel.SavePhoto(info);
	}
}
