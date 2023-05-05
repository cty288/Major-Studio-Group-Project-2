using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework.Architecture;
using Crosstales.RTVoice;
using MikroFramework.TimeSystem;
using MikroFramework.UIKit;
using UnityEngine.SceneManagement;

using TMPro;

public class MainMenuSetting : AbstractPanelContainer {
    private List<MenuSettingsElement> menuSettingsElements;
    
    private void Awake() {
        menuSettingsElements = this.GetComponentsInChildren<MenuSettingsElement>(true).ToList();
    }

    public override void OnInit() {
        
    }

    public override void OnOpen(UIMsg msg) {
        foreach (MenuSettingsElement menuSettingsElement in menuSettingsElements) {
            menuSettingsElement.OnRefresh();
        }
    }

    public override void OnClosed() {
      
    }
}
