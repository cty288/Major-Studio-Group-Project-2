using System;
using System.Collections;
using System.Collections.Generic;
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

    private NewspaperViewController todayNewspaper;
    

   
    protected override void Awake() {
        base.Awake();
        this.RegisterEvent<OnNewspaperGenerated>(OnNewspaperGenerated).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<SpawnTableItemEvent>(OnSpawnItem).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnBountyHunterKillCorrectAlien>(OnBountyHunterKillCorrectAlien).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnNewPhotoTaken>(OnNewPhotoTaken).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnNewPhotoTaken(OnNewPhotoTaken e) {
        GameObject photo = SpawnItem(photoPrefabList[Random.Range(0, photoPrefabList.Count)]);
        photo.GetComponent<PhotoOnTable>().SetPhotoID(e.CropInfo.ID);
    }

    private void OnBountyHunterKillCorrectAlien(OnBountyHunterKillCorrectAlien e) {
        GameObject gift = SpawnItem(bountyHunterGiftPrefab);
        gift.GetComponent<BountyHunterGiftViewController>().FoodCount = e.FoodCount;
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
