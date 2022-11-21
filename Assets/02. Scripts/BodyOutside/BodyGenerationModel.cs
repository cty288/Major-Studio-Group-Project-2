using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

public class BodyGenerationModel : AbstractModel {
    public BindableProperty<BodyInfo> CurrentOutsideBody { get; } = new BindableProperty<BodyInfo>(null);
    public bool CurrentOutsideBodyConversationFinishing { get; set; } = false;
    protected override void OnInit() {
        
    }
}
