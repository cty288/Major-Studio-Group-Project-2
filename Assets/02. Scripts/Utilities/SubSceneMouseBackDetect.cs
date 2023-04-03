using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.EventSystems;

public class SubSceneMouseBackDetect : AbstractMikroController<MainGame>
{
    protected List<Collider2D> colliders = new List<Collider2D>();
    [SerializeField] private List<Collider2D> additionalColliders = new List<Collider2D>();
    private GameSceneModel gameSceneModel;
    [SerializeField] private GameScene gameScene;
    [SerializeField] private bool isUI = true;
    private PlayerControlModel playerControlModel;
    
    private void Awake() {
        gameSceneModel = this.GetModel<GameSceneModel>();
        colliders = GetComponents<Collider2D>().ToList();
        colliders.AddRange(additionalColliders);
        playerControlModel = this.GetModel<PlayerControlModel>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if(EventSystem.current.IsPointerOverGameObject()) {
                return;
            }

            if (playerControlModel.ControlType.Value == PlayerControlType.Screenshot) {
                return;
            }
            Vector2 mousePos = Input.mousePosition;
            if (!isUI) {
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            }
            
            
            bool mouseClickPanel = colliders.Any(c => c.OverlapPoint(mousePos));

            if (!mouseClickPanel && gameSceneModel.GameScene.Value == gameScene) {
                this.GetModel<GameSceneModel>().GameScene.Value = GameScene.MainGame;
            }
        }
    }
}
