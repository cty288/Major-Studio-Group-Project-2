using System;
using System.Collections;
using System.Collections.Generic;

using _02._Scripts.FashionCatalog;
using _02._Scripts.GameEvents.Camera;
using _02._Scripts.GameTime;
using _02._Scripts.Notebook;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.ResKit;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Table :  AbstractDroppableItemContainerViewController {
    [SerializeField] private GameObject newspaperPrefab;
    [SerializeField] private GameObject bountyHunterGiftPrefab;
    [SerializeField] private List<GameObject> photoPrefabList;
    [SerializeField] private List<GameObject> crumbledPaperList;

    [SerializeField] private List<GameObject> fashionBookList;

    [SerializeField] private GameObject cameraPrefab;

    
    
    private NewspaperViewController todayNewspaper;
    

   
    protected override void Awake() {
        base.Awake();
        this.RegisterEvent<OnNewspaperGenerated>(OnNewspaperGenerated).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<SpawnTableItemEvent>(OnSpawnItem).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnBountyHunterKillCorrectAlien>(OnBountyHunterKillCorrectAlien).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnNewPhotoTaken>(OnNewPhotoTaken).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnNoteDeleted>(OnNoteDeleted).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnCameraReceive>(OnCameraReceive).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnFashionCatalogGenerated>(OnFashionCatalogGenerated).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnFashionCatalogGenerated(OnFashionCatalogGenerated e) {
        GameObject book = SpawnItem(fashionBookList[Random.Range(0, fashionBookList.Count)]);
        GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
        book.GetComponent<FashionCatalogViewController>().SetContent(e.BodyPartIndicesUpdateInfo.Time, gameTimeModel.Week);

       
    }

    private void OnCameraReceive(OnCameraReceive e) {
        GameObject camera = SpawnItem(cameraPrefab);
    }

    private void OnNoteDeleted(OnNoteDeleted e) {
        if (!e.SpawnTrash) {
            return;
        }
        GameObject litter = SpawnItem(crumbledPaperList[Random.Range(0, crumbledPaperList.Count)]);
    }

    private void OnNewPhotoTaken(OnNewPhotoTaken e) {
        GameObject photo = SpawnItem(photoPrefabList[Random.Range(0, photoPrefabList.Count)]);
        photo.GetComponent<PhotoOnTable>().SetPhotoID(e.CropInfo.ID);
    }

    private void OnBountyHunterKillCorrectAlien(OnBountyHunterKillCorrectAlien e) {
        GameObject gift = SpawnItem(bountyHunterGiftPrefab);
    }


    private void OnSpawnItem(SpawnTableItemEvent obj) {
        SpawnItem(obj.Prefab);
    }

    private void OnNewspaperGenerated(OnNewspaperGenerated e) {
        if (todayNewspaper) {
            todayNewspaper.StopIndicateTodayNewspaper();
        }

        
        todayNewspaper = SpawnItem(newspaperPrefab).GetComponent<NewspaperViewController>();
        todayNewspaper.StartIndicateTodayNewspaper();
        todayNewspaper.SetContent(e.Newspaper);
    }
}
