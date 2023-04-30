using System.Collections;
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.Notebook;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.UI;

public class NotebookPhotoDroppableInfo : DroppableInfo {
    [field: ES3Serializable]
    public override bool IsDefaultOnTop { get; } = true;

    [ES3Serializable] private string photoID = "";
    

    public string PhotoID {
        get => photoID;
        set => photoID = value;
    }

    [ES3Serializable]
    private GameObject contentUIPrefab;
	
    public NotebookPhotoDroppableInfo(){}

    public NotebookPhotoDroppableInfo(GameObject prefab, string photoID) {
        this.contentUIPrefab = prefab;
        this.photoID = photoID;
    }
	
	
    public override DroppedUIObjectViewController GetContentUIObject(RectTransform parent) {
        NotebookPhoto droppedText =
            GameObject.Instantiate(contentUIPrefab).GetComponent<NotebookPhoto>() as
                NotebookPhoto;
        droppedText.SetContent(photoID);
        droppedText.transform.SetParent(parent);
        //droppedText.GetComponent<RectTransform>().localPosition = Vector3.zero;
		
        return droppedText;
    }
}
public class NotebookPhoto : DroppedUIObjectViewController, IHaveBodyInfo {
    protected PhotoSaveModel photoSaveModel;
    protected RawImage photoImage;
    protected PlayerControlModel playerControlModel;
    protected string photoID;
    protected override void Awake() {
        base.Awake();
        photoSaveModel = this.GetModel<PhotoSaveModel>();
        photoImage = GetComponentInChildren<RawImage>(true);
        playerControlModel = this.GetModel<PlayerControlModel>();
      //  bodyModel = this.GetModel<BodyModel>();
    }

    public override Vector2 GetExtent() {
        Vector3[] corners = new Vector3[4];
        RectTransform rect = GetComponent<RectTransform>();
        rect.GetWorldCorners(corners);
        Vector3 pos = corners[0];
        Vector2 size = new Vector2(
            rect.lossyScale.x * rect.rect.size.x,
            rect.lossyScale.y * rect.rect.size.y);

        return new Rect(pos, size).size * 0.5f;
    }

    protected override void OnClick() {
        if (!playerControlModel.HasControlType(PlayerControlType.Normal)) {
            return;
        }
        this.SendCommand<OpenPhotoPanelCommand>(new OpenPhotoPanelCommand() {
            Content = photoID,
            BGSprite =  GetComponent<Image>().sprite,
            BodyInfos = BodyInfos
        });
    }

    public void SetContent(string photoID) {
        this.photoID = photoID;
        photoImage.texture = photoSaveModel.GetPhotoTexture(photoSaveModel.GetPhoto(photoID));
        BodyInfos = photoSaveModel.GetPhoto(photoID).StoredBodyInfoId;
    }

    public List<BodyInfo> BodyInfos { get; set; }
}
