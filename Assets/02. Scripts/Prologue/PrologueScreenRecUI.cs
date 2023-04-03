using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public class PrologueScreenRecUI : AbstractMikroController<MainGame> {
   private GameObject panel;

   private void Awake() {
      panel = transform.Find("Panel").gameObject;
      this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
     
   }

   private void OnNewDay(OnNewDay e) {
      if(e.Day==0){
         panel.gameObject.SetActive(true);
      }
      else {
         panel.gameObject.SetActive(false);
      }
   }
}
