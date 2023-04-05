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
    private PlayerResourceModel playerResourceModel;

    [SerializeField] private List<Image> foodImages;
    [SerializeField] private TMP_Text foodIndicatorText;

    
    private void Awake() {
        playerResourceModel = this.GetModel<PlayerResourceModel>();
        
        dayIndicator = transform.Find("DayIndicator").GetComponent<TMP_Text>();
        animator = GetComponent<Animator>();
        this.GetSystem<GameTimeManager>().OnDayStart += OnDayStart;


        lastFoodCount = playerResourceModel.GetResourceCount<FoodResource>();
        GameTimeManager gameTimer = this.GetSystem<GameTimeManager>();
        if (this.GetModel<OutdoorActivityModel>().HasMap) {
            dayIndicator.text = $"Day {gameTimer.Day}\n{gameTimer.DayTimeStart}:00";
        }
        else {
            dayIndicator.text = $"Day {gameTimer.Day}\n{gameTimer.NightTimeStart}:00";
        }

        if (gameTimer.Day <= 0) {
            dayIndicator.text = $"Prologue";
        }
        
        
        
        //SetFoodCount(lastFoodCount);
        //this.RegisterEvent<OnShowFood>(OnShowFood).UnRegisterWhenGameObjectDestroyed(gameObject);
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

        if (lastFoodCount > playerResourceModel.GetResourceCount<FoodResource>()) {
            int count = lastFoodCount -  playerResourceModel.GetResourceCount<FoodResource>();
            
            for (int i = lastFoodCount - count ; i < foodImages.Count; i++) {
                foodImages[i].DOFade(0, 1f).SetDelay(2);
            }
        }
        else if (lastFoodCount <  playerResourceModel.GetResourceCount<FoodResource>()) {
            int count =  playerResourceModel.GetResourceCount<FoodResource>() - lastFoodCount;
           
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

        lastFoodCount =  playerResourceModel.GetResourceCount<FoodResource>();

    }

    private void CheckDeath() {
        if (this.GetModel<GameStateModel>().GameState == GameState.End) {
            DieCanvas.Singleton.Show("You run out of food!");
        }
    }

    private void OnDayStart(int day, int hour) {
        if (day <= 0) {
            dayIndicator.text = $"Prologue";
        }
        else {
            dayIndicator.text = $"Day {day}\n{hour}:00";
        }
       
        animator.CrossFade("Show", 1.5f);
    }

    private void OnShowFoodIndicator() {
       // PlayFoodCountAnimation();
    }
    
}
