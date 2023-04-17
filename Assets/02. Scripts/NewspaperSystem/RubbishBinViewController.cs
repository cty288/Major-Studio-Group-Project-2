using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public class RubbishBinViewController : AbstractMikroController<MainGame> {
    [SerializeField] private Material outlineMaterial;
    private SpriteRenderer spriteRenderer;
    private Material defaultMaterial;
    private NewspaperSystem newspaperSystem;
    [SerializeField] private GameObject hintCanvas;
    private Collider2D collider;
    private DraggableItems draggingItem;
    private void Awake()
    {
        outlineMaterial = Material.Instantiate(outlineMaterial);
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
        newspaperSystem = this.GetSystem<NewspaperSystem>();
        collider = GetComponent<Collider2D>();
        gameObject.SetActive(false);
        this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnNewDay(OnNewDay e) {
        gameObject.SetActive(e.Day > 0);
    }

    private void Update() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0);
        //check if the mouse is over the sprite
        if (collider.bounds.Contains(mousePosition)) {
            if (DraggableItems.CurrentDroppingItem) {
                Highlight();
                draggingItem = DraggableItems.CurrentDroppingItem;
            }
            else {
                if (draggingItem) {
                   
                    StopHighlight();
                    draggingItem.OnThrown();
                    
                    DraggableItems.CurrentDroppingItem = null;
                    draggingItem = null;
                }
            }
        }
        else {
            StopHighlight();
            draggingItem = null;
        }
    }

    public void Highlight() {
        spriteRenderer.material = outlineMaterial;
        hintCanvas.SetActive(true);
    }

    public void StopHighlight() {
        spriteRenderer.material = defaultMaterial;
        hintCanvas.SetActive(false);
    }
}
