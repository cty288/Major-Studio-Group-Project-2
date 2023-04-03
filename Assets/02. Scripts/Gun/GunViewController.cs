using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;

public class GunViewController : AbstractMikroController<MainGame> {
    private PlayerResourceModel playerResourceModel;
    private TMP_Text infoText;
    
    [SerializeField] private List<GameObject> bullets = new List<GameObject>();
    
    private void Awake() {
        infoText = transform.Find("Canvas/InfoText").GetComponent<TMP_Text>();
        playerResourceModel = this.GetModel<PlayerResourceModel>();

        
        this.RegisterEvent<OnPlayerResourceNumberChanged>(OnResourceNumberChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnNewDay(OnNewDay e) {
        if (e.Day == 0) {
            gameObject.SetActive(false);
        }
        else {
            gameObject.SetActive(true);
        }
    }

    private void Start() {
        int bulletCount = playerResourceModel.GetResourceCount<BulletGoods>();
        UpdateBullets(bulletCount);
    }

    private void OnResourceNumberChanged(OnPlayerResourceNumberChanged e) {
        if (e.GoodsInfo.Type == typeof(BulletGoods)) {
            int count = e.GoodsInfo.Count;
            UpdateBullets(count);
        }
    }

    private void UpdateBullets(int count) {
        for (int i = 0; i < bullets.Count; i++) {
            bullets[i].SetActive(i < count);
        }

        infoText.text = $"{count}/6 Bullets";
    }
    
    

    
}
