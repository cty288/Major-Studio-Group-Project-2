using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using MikroFramework.Architecture;
using UnityEngine;

public class Cheater : AbstractMikroController<MainGame> {
    public bool enable = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            enable = !enable;
        }
        if (!enable) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            this.GetSystem<PlayerResourceSystem>().AddFood(1);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            this.GetSystem<ElectricitySystem>().AddElectricity(1);
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            BodyInfo info = BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, true, false,
                new NormalKnockBehavior(3, Random.Range(3, 7), null));
            
            this.GetSystem<BodyManagmentSystem>().AddToAllBodyTimeInfos(new BodyTimeInfo(3, info));
            this.GetModel<BodyGenerationModel>().CurrentOutsideBody.Value = info;
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            this.GetModel<BodyGenerationModel>().CurrentOutsideBody.Value = null;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Time.timeScale += 10;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Time.timeScale = Mathf.Max(Time.timeScale-10, 1);
        }

        if (Input.GetKeyDown(KeyCode.I)) {
           // this.GetSystem<TelephoneSystem>().IncomingCall(new TestIncomingCallContact(), 3);
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            this.GetSystem<PlayerResourceSystem>().AddResource(new BulletGoods(), 1);
        }
    }
}
