using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.EventSystems;


public class DoorViewController : AbstractMikroController<MainGame>, IPointerClickHandler {
   
    public void OnPointerClick(PointerEventData eventData) {
        this.GetModel<GameSceneModel>().GameScene.Value = GameScene.Peephole;
       
    }
}
