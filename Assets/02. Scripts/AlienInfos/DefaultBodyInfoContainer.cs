using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using MikroFramework.Architecture;
using UnityEngine;

public class DefaultBodyInfoContainer : AbstractMikroController<MainGame>, IHaveBodyInfo {
    [field: ES3Serializable]
    public List<BodyInfo> BodyInfos { get; set; }
    
    private BodyModel bodyModel;

    private void Awake() {
        bodyModel = this.GetModel<BodyModel>();
    }

    private void Start() {
        if (BodyInfos != null) {
            for (int i = 0; i < BodyInfos.Count; i++) {
                BodyInfo info = bodyModel.GetBodyInfoByID(BodyInfos[i].ID);
                if(info!=null){
                    BodyInfos[i] = info;
                }
            }
        }
    }
}
