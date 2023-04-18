using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.ArmyEnding;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cheater : AbstractMikroController<MainGame> {
    public bool enable = false;
    // Start is called before the first frame update
    void Start() {
      
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
            this.GetModel<PlayerResourceModel>().AddFood(1);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            this.GetSystem<ElectricitySystem>().AddElectricity(1);
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            BodyInfo info = BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, true, 0,
                new NormalKnockBehavior(3, Random.Range(3, 7), null), null, 40);
            
            this.GetModel<BodyModel>().AddToAllBodyTimeInfos(new BodyTimeInfo(3, info, true));
            this.GetModel<BodyGenerationModel>().CurrentOutsideBody.Value = info;
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            this.GetModel<BodyGenerationModel>().CurrentOutsideBody.Value = null;
        }

        if (Input.GetKeyDown(KeyCode.M)) {
            this.GetModel<OutdoorActivityModel>().HasMap.Value = true;
        }

        if (Input.GetKeyDown(KeyCode.T)) {
            this.GetSystem<OutdoorActivitySystem>().SetPlaceAvailable("TestArea", true);
            this.GetSystem<OutdoorActivitySystem>().SetPlaceAvailable("HuntingGround", true);
        }

        if (Input.GetKeyDown(KeyCode.U)) {
            IPlace place = this.GetSystem<OutdoorActivitySystem>().GetPlace("TestArea");
            place.EnableTemporaryActivity("TestArea_TestActivity2",
                this.GetModel<GameTimeModel>().CurrentTime.Value.AddDays(1));
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            this.GetModel<RadioModel>().UnlockChannel(RadioChannel.FM100);
        }
        

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Time.timeScale += 10;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Time.timeScale = Mathf.Max(Time.timeScale-10, 1);
        }
        

        if (Input.GetKeyDown(KeyCode.B)) {
            this.GetModel<PlayerResourceModel>().AddResource(new BulletGoods(), 1);
        }
        
        if (Input.GetKeyDown(KeyCode.G)) {
            this.GetModel<PlayerResourceModel>().AddResource(new PowerGeneratorGoods(), 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { //start army ending storyline
            GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
            DateTime armyPrologueRadioTime = gameTimeModel.GetDay(gameTimeModel.Day + 1);
            this.GetSystem<GameEventSystem>().AddEvent(new ShelterPrologueRadio(new TimeRange(armyPrologueRadioTime),
                AudioMixerList.Singleton.AudioMixerGroups[13]));
        }
    }
}
