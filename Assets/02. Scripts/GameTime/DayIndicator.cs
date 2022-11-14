using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayIndicator : MonoBehaviour{
    private Animator animator;
    private TMP_Text dayIndicator;
    private int lastFoodCount;
    private void Awake() {
        dayIndicator = transform.Find("DayIndicator").GetComponent<TMP_Text>();
        animator = GetComponent<Animator>();
        GameTimeManager.Singleton.OnDayStart += OnDayStart;
    }

    private void OnDestroy() {
        GameTimeManager.Singleton.OnDayStart -= OnDayStart;
    }

    private void OnDayStart(int day) {
        dayIndicator.text = $"Day {day}\n22:00";
        animator.CrossFade("Show", 1.5f);
    }

    private void OnShowFoodIndicator() {

    }

    private void OnFoodIndicatorDecrease() {

    }

    private void OnHideFoodIndicator() {

    }
}
