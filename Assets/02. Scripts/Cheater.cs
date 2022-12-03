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

        if (Input.GetKeyDown(KeyCode.E)) {
            this.GetSystem<ElectricitySystem>().AddElectricity(1);
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            this.GetModel<BodyGenerationModel>().CurrentOutsideBody.Value =
                BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, true);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Time.timeScale += 10;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Time.timeScale = Mathf.Max(Time.timeScale-10, 1);
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            this.GetSystem<TelephoneSystem>().IncomingCall(new TestIncomingCallContact(), 3);
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            this.GetSystem<PlayerResourceSystem>().AddResource(new BulletGoods(), 1);
        }
    }
}
