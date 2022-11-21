using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class RadioModel : AbstractModel {

    public List<AlienDescriptionData> DescriptionDatas { get; } = new List<AlienDescriptionData>();

    public bool IsSpeaking { get; set; } = false;
    protected override void OnInit() {
        
    }


}
