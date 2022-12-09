using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;

public class DogFoodViewController : AbstractMikroController<MainGame> {
    [SerializeField] private Sprite dogDieSprite;
    private SpriteRenderer spriteRenderer;
    private TMP_Text infoText;
    private void Awake() {
        this.RegisterEvent<OnDogGet>(OnDogGet).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnDogDie>(OnDogDie).UnRegisterWhenGameObjectDestroyed(gameObject);
        spriteRenderer = GetComponent<SpriteRenderer>();
        infoText = transform.Find("Canvas/Text").GetComponent<TMP_Text>();
        gameObject.SetActive(false);
    }

    private void OnDogDie(OnDogDie e) {
        infoText.text = "My friend is gone...";
        spriteRenderer.sprite = dogDieSprite;
    }

    private void OnDogGet(OnDogGet e) {
        gameObject.SetActive(true);
    }
}
