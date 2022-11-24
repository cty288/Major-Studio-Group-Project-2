using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.ResKit;
using UnityEngine;
using UnityEngine.EventSystems;

public class Table :  AbstractDroppableItemContainerViewController {
    [SerializeField] private GameObject newspaperPrefab;
    [SerializeField] private GameObject bountyHunterGiftPrefab;
  
    private NewspaperViewController todayNewspaper;
    

   
    protected override void Awake() {
        base.Awake();
        this.RegisterEvent<OnNewspaperGenerated>(OnNewspaperGenerated).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<SpawnTableItemEvent>(OnSpawnItem).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnBountyHunterKillCorrectAlien>(OnBountyHunterKillCorrectAlien).UnRegisterWhenGameObjectDestroyed(gameObject);
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
