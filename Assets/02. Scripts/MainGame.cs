using System.Collections;
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.ResKit;
using MikroFramework.TimeSystem;
using UnityEngine;

public class MainGame : Architecture<MainGame> {

    protected List<AbstractSavableModel> savableModels = new List<AbstractSavableModel>();
    protected override void Init() {
        
        this.RegisterExtensibleUtility<ResLoader>(ResLoader.Allocate());
        this.RegisterSystem<ITimeSystem>(new TimeSystem());
        this.RegisterSystem<GameTimeManager>(new GameTimeManager());
        this.RegisterSystem<GameEventSystem>(new GameEventSystem());
        this.RegisterSystem<BodyGenerationSystem>(new BodyGenerationSystem());
        this.RegisterSystem<BodyManagmentSystem>(new BodyManagmentSystem());
        this.RegisterSystem<NewspaperSystem>(new NewspaperSystem());
        this.RegisterSystem<PlayerResourceSystem>(new PlayerResourceSystem());
        this.RegisterSystem<TelephoneSystem>(new TelephoneSystem());
        this.RegisterSystem<MerchantSystem>(new MerchantSystem());
        this.RegisterSystem<BountyHunterSystem>(new BountyHunterSystem());
        this.RegisterSystem<ElectricitySystem>(new ElectricitySystem());
        this.RegisterSystem<DogSystem>(new DogSystem());
        
        this.RegisterModel<GameStateModel>();
        this.RegisterModel<GameSceneModel>();
        this.RegisterModel<RadioModel>();
        this.RegisterModel<BodyGenerationModel>();
        this.RegisterModel<BodyTagInfoModel>();
        this.RegisterModel<BodyKnockPhraseModel>();
        this.RegisterModel<NewspaperModel>();
        this.RegisterModel<GameTimeModel>();

        
    }
    
    protected void RegisterModel<T>() where T : class, IModel, new() {
        T model = null;
        if (typeof(T).IsSubclassOf(typeof(AbstractSavableModel))) {
            model = ES3.Load<AbstractSavableModel>("Model_" + typeof(T).Name, "models.es3", new T() as AbstractSavableModel) as T;
            savableModels.Add(model as AbstractSavableModel);
        }
        else {
            model = new T();
        }
       
        this.RegisterModel<T>(model);
    }

    public void SaveGame() {
        foreach (AbstractSavableModel savableModel in savableModels) {
            savableModel.Save();
            
        }
        ES3AutoSaveMgr.Current.Save();
    }

   
}
