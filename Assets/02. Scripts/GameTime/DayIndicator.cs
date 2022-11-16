using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayIndicator : AbstractMikroController<MainGame> {
    private Animator animator;
    private TMP_Text dayIndicator;
    private int lastFoodCount;
    private PlayerResourceSystem playerResourceSystem;

    [SerializeField] private List<Image> foodImages;
    [SerializeField] private TMP_Text foodIndicatorText;

    
    private void Awake() {
        playerResourceSystem = this.GetSystem<PlayerResourceSystem>();
        
        dayIndicator = transform.Find("DayIndicator").GetComponent<TMP_Text>();
        animator = GetComponent<Animator>();
        this.GetSystem<GameTimeManager>().OnDayStart += OnDayStart;
        this.GetSystem<GameTimeManager>().NextDay();
       lastFoodCount = playerResourceSystem.FoodCount.Value;
        SetFoodCount(lastFoodCount);
        this.RegisterEvent<OnShowFood>(OnShowFood).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnShowFood(OnShowFood obj) {
        OnShowFoodIndicator();
    }

    private void OnDestroy() {
        this.GetSystem<GameTimeManager>().OnDayStart -= OnDayStart;
    }


    private void SetFoodCount(int count) {
        for (int i = 0; i < foodImages.Count; i++) {
            foodImages[i].enabled = i < count;
            
        }
    }

    private void PlayFoodCountAnimation() {
        SetFoodCount(lastFoodCount);
        foodIndicatorText.DOFade(1, 1f);
        foreach (Image image in foodImages) {
            image.DOFade(1, 1f);
        }

        if (lastFoodCount > playerResourceSystem.FoodCount) {
            int count = lastFoodCount - playerResourceSystem.FoodCount;
            
            for (int i = lastFoodCount - count ; i < foodImages.Count; i++) {
                foodImages[i].DOFade(0, 1f).SetDelay(2);
            }
        }
        else if (lastFoodCount < playerResourceSystem.FoodCount) {
            int count = playerResourceSystem.FoodCount - lastFoodCount;
           
            for (int i = lastFoodCount; i < lastFoodCount + count; i++) {
                foodImages[i].DOKill();
                foodImages[i].enabled = true;
                foodImages[i].color = new Color(1, 1, 1, 0);
                foodImages[i].gameObject.SetActive(true);
                foodImages[i].DOFade(1, 1f).SetDelay(2);

            }
        }

        foreach (Image foodImage in foodImages) {
            foodImage.DOFade(0, 1f).SetDelay(4.5f);
        }

        foodIndicatorText.DOFade(0, 1f).SetDelay(4.5f);

        lastFoodCount = playerResourceSystem.FoodCount;

    }

    private void CheckDeath() {
        if (this.GetModel<GameStateModel>().GameState == GameState.End) {
            DieCanvas.Singleton.Show("You run out of food!");
        }
    }

    private void OnDayStart(int day) {
        dayIndicator.text = $"Day {day}\n22:00";
        animator.CrossFade("Show", 1.5f);
    }

    private void OnShowFoodIndicator() {
        PlayFoodCountAnimation();
    }
    
}
