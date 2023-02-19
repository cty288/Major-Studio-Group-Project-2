using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public struct OnNewPhotoTaken {
    public CropInfo CropInfo;
}
public class PhotoSaveModel : AbstractSavableModel {
    protected Dictionary<string, CropInfo> photoInfos;
    
    public override void OnLoad() {
        base.OnLoad();
        if (ES3.FileExists("photos")) {
            byte[] bytes = ES3.LoadRawBytes("photos");
            if (bytes != null) {
                photoInfos = ES3.Deserialize<Dictionary<string, CropInfo>>(bytes);
                Debug.Log("Loaded cropped photos" + photoInfos.Count + " photos");
            } else {
                photoInfos = new Dictionary<string, CropInfo>();
            }
        }
        else {
            photoInfos = new Dictionary<string, CropInfo>();
        }
        
    }

    public override void OnSave() {
        base.OnSave();
        ES3.SaveRaw(ES3.Serialize(photoInfos), "photos");
    }
    
    public void SavePhoto(CropInfo info) {
        if (!photoInfos.ContainsKey(info.ID)) {
            photoInfos.Add(info.ID, info);
            this.SendEvent<OnNewPhotoTaken>(new OnNewPhotoTaken() {CropInfo = info});
        }
    }
    
    public void RemovePhoto(string id) {
        if (photoInfos.ContainsKey(id)) {
            photoInfos.Remove(id);
        }
    }
    
    public CropInfo GetPhoto(string id) {
        if (photoInfos.ContainsKey(id)) {
            return photoInfos[id];
        }
        return null;
    }
    
    public Texture2D GetPhotoTexture(CropInfo info) {
        Texture2D texture =  new Texture2D(info.Width, info.Height, TextureFormat.RGB24, true);
        texture.LoadRawTextureData(info.OutputTexture);
        texture.Apply();
        return texture;
    }
}
