using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameEvents.Camera;
using _02._Scripts.GameTime;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using UnityEngine;

public class VintageCameraViewController : DraggableItems {
	protected SpriteRenderer spriteRenderer;
	protected PhotoSaveModel photoSaveModel;
	protected Canvas canvas;
	protected GameObject useCameraHint;

	[SerializeField]
	private AbstractDroppableItemContainerViewController table;
	
	[SerializeField]
	private Vector3 originalLocalPosition = new Vector3(5, -0.5f, 0);

	[SerializeField] private AudioClip cam_shutter;
	
	private PlayerControlModel playerControlModel;
	protected int cameraDay = 3;
	protected override void Awake() {
		base.Awake();
		spriteRenderer = GetComponent<SpriteRenderer>();
		photoSaveModel = this.GetModel<PhotoSaveModel>();
		canvas = transform.Find("HintCanvas").GetComponent<Canvas>();
		useCameraHint = transform.Find("UseCameraHint").gameObject;
		
		ScreenSpaceImageCropper.Singleton.OnStartCrop += OnStartCrop;
		ScreenSpaceImageCropper.Singleton.OnEndCrop += OnEndCrop;
		ScreenSpaceImageCropper.Singleton.OnDragMouse += OnDragMouse;
		
		playerControlModel = this.GetModel<PlayerControlModel>();

		this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
		cameraDay = int.Parse(this.GetModel<HotUpdateDataModel>().GetData("CameraDay").values[0]);
	}

	private void OnNewDay(OnNewDay e)
	{
		if (e.Day == cameraDay)
		{
			DateTime eventTime = new DateTime(e.Date.Year, e.Date.Month, e.Date.Day, 22, 0, 0);
			this.GetSystem<GameEventSystem>().AddEvent(new ReceiveCamera(new TimeRange(eventTime, eventTime)));
		}
	}

	private void OnDestroy() {
		ScreenSpaceImageCropper.Singleton.OnStartCrop -= OnStartCrop;
		ScreenSpaceImageCropper.Singleton.OnEndCrop -= OnEndCrop;
		ScreenSpaceImageCropper.Singleton.OnDragMouse -= OnDragMouse;
	}

	private void OnDragMouse() {
		useCameraHint.SetActive(false);
	}

	private void OnEndCrop() {
		useCameraHint.SetActive(false);
	}

	private void OnStartCrop() {
		useCameraHint.SetActive(true);
	}

	protected override void Start() {
		base.Start();
		gameObject.SetActive(false);
		photoSaveModel.HasCamera.RegisterWithInitValue(OnHasCameraChanged)
			.UnRegisterWhenGameObjectDestroyed(gameObject);
	}

	private void OnHasCameraChanged(bool hasCamera) {
		if (hasCamera) {
			table.AddItem(this);
		}
	}

	public override void SetLayer(int layer) {
		spriteRenderer.sortingOrder = 2000;
		canvas.sortingOrder = 4000;
	}

	protected override void OnClick() {
		if (ScreenSpaceImageCropper.Singleton.IsCropping || !playerControlModel.IsNormal()) {
			return;
		}

		ScreenSpaceImageCropper.Singleton.StartCrop(null, OnFinishCropping);
	}
	
	private void OnFinishCropping(CropInfo info) {
		//info.
		AudioSystem.Singleton.Play2DSound(cam_shutter);
		photoSaveModel.SavePhoto(info);
	}

	public override void OnThrownToRubbishBin() {
		photoSaveModel.HasCamera.Value = false;
		
		transform.localPosition = originalLocalPosition;
		gameObject.SetActive(false);
	}

	public override void OnAddedToContainer(AbstractDroppableItemContainerViewController container) {
		base.OnAddedToContainer(container);
		
		gameObject.SetActive(true);
	}
}
