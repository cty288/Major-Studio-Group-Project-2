using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class RubbishBinViewController : AbstractMikroController<MainGame> {
    [SerializeField] private Material outlineMaterial;
    private SpriteRenderer spriteRenderer;
    private Material defaultMaterial;
    private NewspaperSystem newspaperSystem;
    [SerializeField] private GameObject hintCanvas;

    private NewspaperViewController newspaperHovering;
    private void Awake()
    {
        outlineMaterial = Material.Instantiate(outlineMaterial);
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
        newspaperSystem = this.GetSystem<NewspaperSystem>();
    }

    private void Update() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0);
        //check if the mouse is over the sprite
        if (spriteRenderer.bounds.Contains(mousePosition)) {
            if (newspaperSystem.CurrentHoldingNewspaper) {
                Highlight();
                newspaperHovering = newspaperSystem.CurrentHoldingNewspaper;
            }
            else {
                if (newspaperHovering) {
                   
                    StopHighlight();
                    Destroy(newspaperHovering.gameObject);
                    newspaperSystem.CurrentHoldingNewspaper = null;
                    newspaperHovering = null;
                }
            }
        }
        else {
            StopHighlight();
            newspaperHovering = null;
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
