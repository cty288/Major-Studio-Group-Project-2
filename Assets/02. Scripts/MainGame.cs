using System.Collections;
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.GameEvents.BountyHunter;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.ResKit;
using MikroFramework.TimeSystem;
using UnityEngine;

public class MainGame : Architecture<MainGame> {

    protected List<AbstractSavableModel> savableModels = new List<AbstractSavableModel>();
    protected List<AbstractSavableSystem> savableSystems = new List<AbstractSavableSystem>();

    protected const bool IsSave = true;
    protected override void Init() {
        
        this.RegisterExtensibleUtility<ResLoader>(ResLoader.Allocate());
        
        
        
        this.RegisterModel<PlayerControlModel>();
        this.RegisterModel<GameStateModel>();
        this.RegisterModel<GameSceneModel>();
        this.RegisterModel<RadioModel>();
        this.RegisterModel<BodyGenerationModel>();
        this.RegisterModel<BodyTagInfoModel>();
        this.RegisterModel<BodyKnockPhraseModel>();
        this.RegisterModel<NewspaperModel>();
        this.RegisterModel<GameTimeModel>();
        this.RegisterModel<BodyModel>();
        this.RegisterModel<PhotoSaveModel>();
        this.RegisterModel<PlayerResourceModel>();
        this.RegisterModel<BountyHunterModel>();

        this.RegisterSystem<ITimeSystem>(new TimeSystem());
        this.RegisterSystem<GameTimeManager>();
        this.RegisterSystem<GameEventSystem>();
        this.RegisterSystem<BodyGenerationSystem>();
        this.RegisterSystem<BodyManagmentSystem>();
        this.RegisterSystem<NewspaperSystem>();
        this.RegisterSystem<PlayerResourceSystem>();
        this.RegisterSystem<TelephoneSystem>();
        this.RegisterSystem<MerchantSystem>();
        this.RegisterSystem<BountyHunterSystem>();
        this.RegisterSystem<ElectricitySystem>();
        this.RegisterSystem<DogSystem>();
    }
    
    protected void RegisterModel<T>() where T : class, IModel, new() {
        T model = null;
        
        if (typeof(T).IsSubclassOf(typeof(AbstractSavableModel)) && IsSave) {
            model = ES3.Load<AbstractSavableModel>("Model_" + typeof(T).Name, "models.es3", new T() as AbstractSavableModel) as T;
            this.RegisterModel<T>(model);
            (model as AbstractSavableModel)?.OnLoad();
            savableModels.Add(model as AbstractSavableModel);
        }
        else {
            model = new T();
            this.RegisterModel<T>(model);
        }
       
        
    }
    
    protected void RegisterSystem<T>() where T : class, ISystem, new() {
        T system = null;
        
        if (typeof(T).IsSubclassOf(typeof(AbstractSavableSystem)) && IsSave) {
            system = ES3.Load<AbstractSavableSystem>("System_" + typeof(T).Name, "systems.es3", new T() as AbstractSavableSystem) as T;
            this.RegisterSystem<T>(system);
            (system as AbstractSavableSystem)?.OnLoad();
            savableSystems.Add(system as AbstractSavableSystem);
        }
        else {
            system = new T();
            this.RegisterSystem<T>(system);
        }
       
       
    }

    public void SaveGame() {
        if (!IsSave) {
            return;
        }
        foreach (AbstractSavableModel savableModel in savableModels) {
            savableModel.Save();
        }
        foreach (AbstractSavableSystem savableSystem in savableSystems) {
            savableSystem.Save();
        }
        ES3AutoSaveMgr.Current.Save();
    }

   
}
