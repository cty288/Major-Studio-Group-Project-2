using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

public enum GameState {
    Playing,
    End
}
public class GameStateModel : AbstractModel {
    public BindableProperty<GameState> GameState { get; } = new BindableProperty<GameState>(global::GameState.Playing);

    protected override void OnInit() {
        
    }
}
