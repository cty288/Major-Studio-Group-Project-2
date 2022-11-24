using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;

public class GunViewController : AbstractMikroController<MainGame> {
    private PlayerResourceSystem playerResourceSystem;
    private TMP_Text infoText;
    
    [SerializeField] private List<GameObject> bullets = new List<GameObject>();
    
    private void Awake() {
        infoText = transform.Find("Canvas/InfoText").GetComponent<TMP_Text>();
        playerResourceSystem = this.GetSystem<PlayerResourceSystem>();
        this.RegisterEvent<OnPlayerResourceNumberChanged>(OnResourceNumberChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnResourceNumberChanged(OnPlayerResourceNumberChanged e) {
        if (e.GoodsInfo.Type == typeof(BulletGoods)) {
            int count = e.GoodsInfo.Count;
            for (int i = 0; i < bullets.Count; i++) {
                bullets[i].SetActive(i < count);
            }

            infoText.text = $"{count}/6 Bullets";
        }
    }

    
}
