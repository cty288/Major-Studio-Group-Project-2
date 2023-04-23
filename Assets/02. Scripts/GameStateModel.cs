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

    [field: ES3Serializable] public bool IsDoorOpened { get; set; } = false;

    protected override void OnInit() {
        
    }
}
