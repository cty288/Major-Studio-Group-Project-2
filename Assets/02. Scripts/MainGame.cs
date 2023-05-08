using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos;
using _02._Scripts.ArmyEnding;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.ChoiceSystem;
using _02._Scripts.CultEnding;
using _02._Scripts.Electricity;
using _02._Scripts.FashionCatalog;
using _02._Scripts.FPSEnding;
using _02._Scripts.GameEvents.BountyHunter;
using _02._Scripts.GameEvents.FoodBorrowEvent;
using _02._Scripts.GameEvents.Merchant;
using _02._Scripts.GameTime;
using _02._Scripts.ImportantNewspaper;
using _02._Scripts.KFC;
using _02._Scripts.Notebook;
using _02._Scripts.Poster;
using _02._Scripts.Radio;
using _02._Scripts.Radio.RadioScheduling;
using _02._Scripts.Radio.RadioScheduling.DorchaDaily;
using _02._Scripts.Radio.RadioScheduling.GhostStory;
using _02._Scripts.SexyCard;
using _02._Scripts.Stats;
using _02._Scripts.SurvivalGuide;
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
        
        this.RegisterModel<AlienNameModel>();
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
        this.RegisterModel<MerchantModel>();
        this.RegisterModel<ElectricityModel>();
        this.RegisterModel<DogModel>();
        this.RegisterModel<NotebookModel>();
        this.RegisterModel<OutdoorActivityModel>();
        this.RegisterModel<HotUpdateDataModel>();
        this.RegisterModel<FashionCatalogModel>();
        this.RegisterModel<RadioSchedulingModel>();
        this.RegisterModel<ImportantNewsTextModel>();
        this.RegisterModel<ImportantNewspaperModel>();
        this.RegisterModel<PosterModel>();
        this.RegisterModel<RadioDialogueDataModel>();
        this.RegisterModel<GhostStoryModel>();
        this.RegisterModel<SexyCardModel>();
        this.RegisterModel<SuspectModel>();
        this.RegisterModel<TelephoneNumberRecordModel>();
        this.RegisterModel<FoodBorrowModel>();
        this.RegisterModel<SurvivalGuideModel>();
        this.RegisterModel<ArmyEndingModel>();
        this.RegisterModel<MonsterMotherModel>();
        this.RegisterModel<CultEndingModel>();
        this.RegisterModel<StatsModel>();
        
        
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
        this.RegisterSystem<OutdoorActivitySystem>();
        this.RegisterSystem<FashionCatalogSystem>();
        this.RegisterSystem<RadioSchedulingSystem>();
        this.RegisterSystem<ImportantNewspaperSystem>();
        this.RegisterSystem<PosterSystem>();
        this.RegisterSystem<DorchaDailySystem>();
        this.RegisterSystem<GhostStorySystem>();
        this.RegisterSystem<ChoiceSystem>();
        this.RegisterSystem<SexyCardSystem>();
        this.RegisterSystem<SuspectSystem>();
        this.RegisterSystem<FoodBorrowSystem>();
        this.RegisterSystem<KFCSystem>();
        this.RegisterSystem<CultEndingSystem>();
        
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
    
    public static void ClearSave() {
        
        ES3.DeleteFile("models.es3");
        ES3.DeleteFile("systems.es3");
        ES3.DeleteFile("photos");
        ES3.DeleteFile("SaveFile.es3");
    }

   
}
