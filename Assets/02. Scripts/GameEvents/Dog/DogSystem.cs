using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;

public class DogSystem : AbstractSystem
{
    private GameTimeManager gameTimeManager;
    private GameEventSystem gameEventSystem;
    public bool isDogAlive;
    public DogInRoom dog;
    
    protected override void OnInit() {
        gameTimeManager = this.GetSystem<GameTimeManager>();
        gameEventSystem = this.GetSystem<GameEventSystem>();
        isDogAlive = true;
    }

    public void SpawnDog()
    {
        dog.gameObject.SetActive(true);
        dog.Init();
    }
}
