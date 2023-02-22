using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class RadioModel : AbstractSavableModel {

    [field: ES3Serializable]
    public List<AlienDescriptionData> DescriptionDatas { get; } = new List<AlienDescriptionData>();
    [field: ES3Serializable]
    public bool IsSpeaking { get; set; } = false;
    protected override void OnInit() {
        
    }


}
