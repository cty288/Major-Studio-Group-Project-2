using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.FPSEnding;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class DoorViewController : AbstractMikroController<MainGame>, IPointerClickHandler {

    private GameObject openDoorHint;
    
    private GameTimeModel gameTimeModel;

    [SerializeField] protected Sprite doorOpenSprite;
    private SpriteRenderer spriteRenderer;
    private Sprite doorCloseSprite;
    
    private MonsterMotherModel monsterMotherModel;
    private TMP_Text doorClickHintText;
    private bool needToCloseDoor = false;
    
    private void Awake() {
        openDoorHint = transform.Find("PrologueOpenDoorHint").gameObject;
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCloseSprite = spriteRenderer.sprite;
        gameTimeModel = this.GetModel<GameTimeModel>();
        doorClickHintText = transform.Find("Canvas").GetComponentInChildren<TMP_Text>(true);
        monsterMotherModel = this.GetModel<MonsterMotherModel>();
        gameTimeModel.CurrentTime.RegisterOnValueChaned(OnCurrentTimeChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        monsterMotherModel.isFightingMother.RegisterOnValueChaned(OnFightingMotherChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnNewDay(OnNewDay e) {
        CloseDoor();
    }

    private void CloseDoor() {
        needToCloseDoor = false;
        doorClickHintText.text = "Use the Peephole";
        spriteRenderer.sprite = doorCloseSprite;
        this.GetModel<GameStateModel>().IsDoorOpened = false;
    }

    private void OnFightingMotherChanged(bool isFighting) {
        if (isFighting) {
            spriteRenderer.sprite = doorOpenSprite;
            doorClickHintText.text = "";
            this.GetModel<GameStateModel>().IsDoorOpened = true;
        }
        else {
            doorClickHintText.text = "Close the door";
            needToCloseDoor = true;
            
        }
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

        if (monsterMotherModel.isFightingMother) {
            return;
        }

        if (needToCloseDoor) {
            CloseDoor();
            return;
        }
        this.GetModel<GameSceneModel>().GameScene.Value = GameScene.Peephole;
       
    }
}
