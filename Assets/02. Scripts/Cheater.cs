using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class Cheater : AbstractMikroController<MainGame>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        if (!Application.isEditor) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            this.GetSystem<PlayerResourceSystem>().AddFood(1);
        }
    }
}
