using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crosstales;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public class FoodSpawnArea : AbstractMikroController<MainGame> {
    [SerializeField] private List<Sprite> foodSprites;

    private List<SpriteRenderer> foodRenderers = new List<SpriteRenderer>();

    private PlayerResourceSystem playerResourceSystem;
    private void Awake() {
        playerResourceSystem = this.GetSystem<PlayerResourceSystem>();
        playerResourceSystem.FoodCount.RegisterOnValueChaned(OnFoodNumberChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        foodRenderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();

        OnFoodNumberChanged(0, playerResourceSystem.FoodCount.Value);
    }

    private void OnFoodNumberChanged(int oldNumber, int foodNumber) {
        if (foodNumber > oldNumber) {
            int extraFoodNumber = foodNumber - oldNumber;
            List<SpriteRenderer> disabledRenderers = foodRenderers.FindAll(r => !r.gameObject.activeInHierarchy);
            disabledRenderers.CTShuffle();
            for (int i = 0; i < extraFoodNumber; i++) {
                disabledRenderers[i].gameObject.SetActive(true);
                disabledRenderers[i].sprite = foodSprites[UnityEngine.Random.Range(0, foodSprites.Count)];
            }
        }
        else {
            int missingFoodNumber = oldNumber - foodNumber;
            List<SpriteRenderer> enabledRenderers = foodRenderers.FindAll(r => r.gameObject.activeInHierarchy);
            enabledRenderers.CTShuffle();
            for (int i = 0; i < missingFoodNumber; i++) {
                enabledRenderers[i].gameObject.SetActive(false);
            }
        }
    }
}
