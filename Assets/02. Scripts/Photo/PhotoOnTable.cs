using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.UI;

public class PhotoOnTable : DraggableItems, IHaveBodyInfo {
	
	protected SpriteRenderer spriteRenderer;
	protected Canvas containerCanvas;
	protected Canvas hintCanvas;
	protected PhotoSaveModel photoSaveModel;

	[ES3Serializable] protected string savedPhotoID;
	
	
	protected RawImage photoRawImage;
	public List<BodyInfo> BodyInfos { get; set; } = new List<BodyInfo>();
	
	protected override void Awake() {
		base.Awake();
		spriteRenderer = GetComponent<SpriteRenderer>();
		containerCanvas = transform.Find("PhotoContainer").GetComponent<Canvas>();
		hintCanvas = transform.Find("Hint").GetComponent<Canvas>();
		photoSaveModel = this.GetModel<PhotoSaveModel>();
		photoRawImage = containerCanvas.transform.Find("Image").GetComponent<RawImage>();
	}

	private void Start() {
		if(!string.IsNullOrEmpty(savedPhotoID)) {
			SetPhotoID(savedPhotoID);
		}
	}

	public override void SetLayer(int layer) {
		spriteRenderer.sortingOrder = layer;
		containerCanvas.sortingOrder = layer + 1;
		hintCanvas.sortingOrder = layer * 10;
	}
	
	public void SetPhotoID(string photoID) {
		savedPhotoID = photoID;
		CropInfo info = photoSaveModel.GetPhoto(savedPhotoID);
		Texture2D texture = photoSaveModel.GetPhotoTexture(info);
		photoRawImage.texture = texture;
		BodyInfos = info.StoredBodyInfoId;
	}
	
	protected override void OnClick() {
		this.SendCommand<OpenPhotoPanelCommand>(new OpenPhotoPanelCommand() {Content = savedPhotoID });
	}

	public override void OnThrownToRubbishBin() {
		photoSaveModel.RemovePhoto(savedPhotoID);
	}

	
}

public struct OnPhotoOpened {
	public string Content;
}

public class OpenPhotoPanelCommand : AbstractCommand<OpenPhotoPanelCommand> {
	public string Content;
	public OpenPhotoPanelCommand() { }
	protected override void OnExecute()
	{
		this.SendEvent<OnPhotoOpened>(
			new OnPhotoOpened() { Content = Content});
	}
}
