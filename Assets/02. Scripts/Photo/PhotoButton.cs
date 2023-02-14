using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhotoButton : AbstractMikroController<MainGame>, IPointerClickHandler {
	private GameObject controlHintObj;
	private void Awake() {
		ScreenSpaceImageCropper.Singleton.OnStartCrop += OnStartCrop;
		ScreenSpaceImageCropper.Singleton.OnEndCrop += OnEndCrop;
		controlHintObj = transform.Find("ControlHint").gameObject;
	}

	private void OnEndCrop() {
		controlHintObj.SetActive(false);
	}

	private void OnStartCrop() {
		controlHintObj.SetActive(true);
	}

	private void OnDestroy() {
		ScreenSpaceImageCropper.Singleton.OnStartCrop -= OnStartCrop;
		ScreenSpaceImageCropper.Singleton.OnEndCrop -= OnEndCrop;
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (ScreenSpaceImageCropper.Singleton.IsCropping) {
			return;
		}

		ScreenSpaceImageCropper.Singleton.StartCrop(null, OnFinishCropping);
	}

	private void OnFinishCropping(CropInfo info) {
		//info.
	}
}
