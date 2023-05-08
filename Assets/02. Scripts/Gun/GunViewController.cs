using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.FPSEnding;
using _02._Scripts.Stats;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;

public class GunViewController : AbstractMikroController<MainGame> {
    private PlayerResourceModel playerResourceModel;
    private TMP_Text infoText;
    
    [SerializeField] private List<GameObject> bullets = new List<GameObject>();
    
    [SerializeField] private Sprite gunShootingSprite;
    private Sprite gunIdleSprite;
    private SpriteRenderer spriteRenderer;
    private StatsModel statsModel;
    private void Awake() {
        infoText = transform.Find("Canvas/InfoText").GetComponent<TMP_Text>();
        playerResourceModel = this.GetModel<PlayerResourceModel>();
        gameObject.SetActive(false);
        spriteRenderer = GetComponent<SpriteRenderer>();
        gunIdleSprite = spriteRenderer.sprite;

        this.RegisterEvent<OnPlayerResourceNumberChanged>(OnResourceNumberChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.GetModel<MonsterMotherModel>().isFightingMother.RegisterOnValueChaned(OnFightMotherChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        statsModel = this.GetModel<StatsModel>();
    }

    private void OnFightMotherChanged(bool isFighting) {
        if (isFighting) {
            spriteRenderer.sprite = gunShootingSprite;
        }
        else {
            this.Delay(1f, () => {
                spriteRenderer.sprite = gunIdleSprite;
            });
        }
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
        bool hasGun = playerResourceModel.GetResourceCount<GunResource>() > 0;
        gameObject.SetActive(hasGun);
        UpdateBullets(bulletCount);
    }

    private void OnResourceNumberChanged(OnPlayerResourceNumberChanged e) {
        if (e.GoodsInfo.Type == typeof(BulletGoods)) {
            int count = e.GoodsInfo.Count;
            UpdateBullets(count);
            int bulletNumChanged = e.NewValue - e.OldValue;
            if (bulletNumChanged > 0) {
                statsModel.UpdateStat("TotalBulletsGet",
                    new SaveData("Bullets Obtained", (int) statsModel.GetStat("TotalBulletsGet", 0) + bulletNumChanged));

            }
        }

        if (e.GoodsInfo.Type == typeof(GunResource)) {
            gameObject.SetActive(e.GoodsInfo.Count > 0);
        }
    }

    private void UpdateBullets(int count) {
        for (int i = 0; i < bullets.Count; i++) {
            bullets[i].SetActive(i < count);
        }

        infoText.text = $"{count}/6 Bullets";
    }
    
    

    
}
