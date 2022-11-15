using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

public class PlayerResourceSystem : AbstractSystem {
    public BindableProperty<int> FoodCount { get; } = new BindableProperty<int>(4);
    private int maxFoodCount = 8;
    protected override void OnInit() {
        this.GetSystem<GameTimeManager>().OnDayStart += OnDayEnd;
    }

    public void AddFood(int count) {
        FoodCount.Value = Mathf.Min(maxFoodCount, FoodCount.Value + count);
    }
    private void OnDayEnd(int day) {
        if (day <= 1) {
            return;
        }
        FoodCount.Value = Mathf.Max(0, FoodCount.Value - 1);
        if (FoodCount.Value <= 0) {
            this.GetModel<GameStateModel>().GameState.Value = GameState.End;
        }
    }
}
