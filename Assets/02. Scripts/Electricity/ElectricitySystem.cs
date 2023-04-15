using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Electricity;
using _02._Scripts.GameEvents.Merchant;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;
using Random = UnityEngine.Random;

public class ElectricitySystemUpdater : MonoBehaviour
{
    public Action OnUpdate;
    private void Update()
    {
        OnUpdate?.Invoke();
    }
}

public struct OnNoElectricity {

}

public struct OnElectricityRecovered {

}
public class ElectricitySystem : AbstractSystem {
    private ElectricitySystemUpdater updater;

    private ElectricityModel electricityModel;
    
    private float electricityDecreaseRate = 0.002f;

    protected GameTimeManager gameTimeManager;
    
    private GameTimeModel gameTimeModel;
    protected ImportantNewspaperModel importantNewspaperModel;

    protected int powerCutoffDay = 4;
    protected override void OnInit() {
        electricityModel = this.GetModel<ElectricityModel>();
        updater = new GameObject("ElectricitySystemUpdater").AddComponent<ElectricitySystemUpdater>();
        electricityModel.Electricity.RegisterOnValueChaned(OnElectricityChange);
        gameTimeManager = this.GetSystem<GameTimeManager>();
        updater.OnUpdate += Update;
        this.RegisterEvent<OnNewDay>(OnNewDay);
        powerCutoffDay = int.Parse(this.GetModel<HotUpdateDataModel>().GetData("PowerCutoffDay").values[0]);
        gameTimeModel = this.GetModel<GameTimeModel>();
        importantNewspaperModel = this.GetModel<ImportantNewspaperModel>();

    }

    private void OnNewDay(OnNewDay e) {
        if (e.Day == 1) {
            importantNewspaperModel.AddPageToNewspaper(importantNewspaperModel.GetWeekForNews(powerCutoffDay),
                this.GetModel<ImportantNewsTextModel>().GetInfo("NoElectricity"),0);
        }
        
        if (e.Day == powerCutoffDay) {
            DateTime poweroffTime = e.Date;
            this.GetSystem<GameEventSystem>().AddEvent(new PowerCutoffRadio(new TimeRange(poweroffTime),
                AudioMixerList.Singleton.AudioMixerGroups[1]));
            this.GetSystem<GameEventSystem>().AddEvent(new MerchantGeneratorAdEvent(new TimeRange(poweroffTime.AddDays(1))));
        }
    }

    private void Update() {
        if (!gameTimeManager.IsNight || !electricityModel.PowerCutoff) {
            return;
        }
        UseElectricity(electricityDecreaseRate * Time.deltaTime);
    }
    private void OnElectricityChange(float oldElectricity, float newElectricity) {
        if (newElectricity <= 0 && oldElectricity > 0) {
            this.SendEvent<OnNoElectricity>();
        }

        if (oldElectricity <= 0 && newElectricity > 0) {
            this.SendEvent<OnElectricityRecovered>();
        }
    }

    public void AddElectricity(float amount) {
        electricityModel.Electricity.Value = Mathf.Min(electricityModel.Electricity.Value + amount, 1);
    }

    public void UseElectricity(float amount) {
        bool hasElectricityGenerator = this.GetModel<PlayerResourceModel>().HasEnoughResource<PowerGeneratorGoods>(1);
        float minElectricity = hasElectricityGenerator ? 0 : 0.01f;
        electricityModel.Electricity.Value = Mathf.Max(electricityModel.Electricity.Value - amount, minElectricity);
    }


}
