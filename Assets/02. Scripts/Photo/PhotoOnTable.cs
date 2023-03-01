using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Notebook;
using MikroFramework.Architecture;
using NHibernate.Mapping;
using UnityEngine;
using UnityEngine.UI;

public class PhotoOnTable : DraggableItems, IHaveBodyInfo, IDroppable {
	
	protected SpriteRenderer spriteRenderer;
	protected Canvas containerCanvas;
	protected Canvas hintCanvas;
	protected Canvas newCanvas;
	protected PhotoSaveModel photoSaveModel;

	[ES3Serializable] protected string savedPhotoID;
	
	
	protected RawImage photoRawImage;
	public List<BodyInfo> BodyInfos { get; set; } = new List<BodyInfo>();
	protected PlayerControlModel playerControlModel;
	
	[ES3Serializable] protected bool opened = false;
	
	protected override void Awake() {
		base.Awake();
		spriteRenderer = GetComponent<SpriteRenderer>();
		containerCanvas = transform.Find("PhotoContainer").GetComponent<Canvas>();
		hintCanvas = transform.Find("Hint").GetComponent<Canvas>();
		photoSaveModel = this.GetModel<PhotoSaveModel>();
		photoRawImage = containerCanvas.transform.Find("Image").GetComponent<RawImage>();
		newCanvas = transform.Find("NewCanvas").GetComponent<Canvas>();
		playerControlModel = this.GetModel<PlayerControlModel>();
	}

	private void Start() {
		if(!string.IsNullOrEmpty(savedPhotoID)) {
			SetPhotoID(savedPhotoID);
		}
	}

	public override void SetLayer(int layer) {
		spriteRenderer.sortingOrder = layer;
		containerCanvas.sortingOrder = layer;
		hintCanvas.sortingOrder = layer * 1000;
		newCanvas.sortingOrder = layer * 1000;
	}
	
	public void SetPhotoID(string photoID) {
		savedPhotoID = photoID;
		CropInfo info = photoSaveModel.GetPhoto(savedPhotoID);
		Texture2D texture = photoSaveModel.GetPhotoTexture(info);
		photoRawImage.texture = texture;
		BodyInfos = info.StoredBodyInfoId;
		if (!opened) {
			newCanvas.gameObject.SetActive(true);
		}else {
			newCanvas.gameObject.SetActive(false);
		}
		
	}
	
	protected override void OnClick() {
		if (playerControlModel.ControlType.Value != PlayerControlType.Normal) {
			return;
		}
		this.SendCommand<OpenPhotoPanelCommand>(new OpenPhotoPanelCommand() {
			Content = savedPhotoID,
			BGSprite =  spriteRenderer.sprite,
			BodyInfos = BodyInfos
		});
		opened = true;
		newCanvas.gameObject.SetActive(false);
	}

	public override void OnThrownToRubbishBin() {
		photoSaveModel.RemovePhoto(savedPhotoID);
		Destroy(gameObject);
	}


	public DroppableInfo GetDroppableInfo() {
		return null;
	}

	public void OnDropped() {
		Destroy(gameObject);
	}
}

public struct OnPhotoOpened {
	public string Content;
	public Sprite BGSprite;
	public List<BodyInfo> BodyInfos;
}

public class OpenPhotoPanelCommand : AbstractCommand<OpenPhotoPanelCommand> {
	public string Content;
	public Sprite BGSprite;
	public List<BodyInfo> BodyInfos;
	
	public OpenPhotoPanelCommand() { }
	protected override void OnExecute()
	{
		this.SendEvent<OnPhotoOpened>(
			new OnPhotoOpened() {
				Content = Content,
				BGSprite = this.BGSprite,
				BodyInfos = this.BodyInfos
			});
	}
}
