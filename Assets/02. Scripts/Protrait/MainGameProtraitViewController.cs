using System.Collections;
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public class MainGameProtraitViewController : DraggableItems
{
    [SerializeField] protected Table table;
    [SerializeField] protected OpenableUIPanel photoPanel;
    
    protected override void Awake() {
        base.Awake();
        this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
        gameObject.SetActive(false);
      
    }

    private void OnNewDay(OnNewDay e) {
        if (e.Day == 0) {
            gameObject.SetActive(false);
        }

        if (e.Day == 1) {
            gameObject.SetActive(true);
            table.AddItem(this);
        }
    }

    public override void SetLayer(int layer) {
      
    }

    protected override void OnClick() {
        photoPanel.Show(0.5f);
    }

    public override void OnThrownToRubbishBin() {
        Destroy(this.gameObject);
    }
}
