using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.EventSystems;


public class DoorViewController : AbstractMikroController<MainGame>, IPointerClickHandler {

    private GameObject openDoorHint;
    
    private GameTimeModel gameTimeModel;
    private void Awake() {
        openDoorHint = transform.Find("PrologueOpenDoorHint").gameObject;
        gameTimeModel = this.GetModel<GameTimeModel>();
        gameTimeModel.CurrentTime.RegisterOnValueChaned(OnCurrentTimeChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnCurrentTimeChanged(DateTime time) {
        if(gameTimeModel.Day ==0 && gameTimeModel.CurrentTime.Value.Hour == 23 && gameTimeModel.CurrentTime.Value.Minute==59) {
            openDoorHint.SetActive(true);
        }

        if (gameTimeModel.Day > 0) {
            gameTimeModel.CurrentTime.UnRegisterOnValueChanged(OnCurrentTimeChanged);
            openDoorHint.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (SubtitleNotebookHightlightedTextRecorder.IsMouseOverUIRC.RefCount > 0) {
            return;
        }
        this.GetModel<GameSceneModel>().GameScene.Value = GameScene.Peephole;
       
    }
}
