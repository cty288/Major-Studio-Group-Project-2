using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

public enum GameScene {
    MainGame,
    Peephole
}
public class GameSceneModel : AbstractModel {
    public BindableProperty<GameScene> GameScene { get; } = new BindableProperty<GameScene>(global::GameScene.MainGame);
    protected override void OnInit() {
        
    }
}
