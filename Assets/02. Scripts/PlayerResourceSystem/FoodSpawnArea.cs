using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crosstales;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;

public class FoodSpawnArea : AbstractMikroController<MainGame> {
    [SerializeField] private List<Sprite> foodSprites;

    private List<SpriteRenderer> foodRenderers = new List<SpriteRenderer>();

    private PlayerResourceModel playerResourceModel;

    private TMP_Text foodText;
    private void Awake() {
        playerResourceModel = this.GetModel<PlayerResourceModel>();
       
        playerResourceModel.FoodCount.RegisterOnValueChaned(OnFoodNumberChanged)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        foodRenderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();
        foodText = transform.Find("FoodAreaHint/Text").GetComponent<TMP_Text>();
        OnFoodNumberChanged(0, playerResourceModel.FoodCount.Value);
    }

    private void OnFoodNumberChanged(int oldNumber, int foodNumber) {
        foodText.text = $"Foods: {foodNumber}/8";
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
