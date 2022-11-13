using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class MouseHoverOutline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private Material outlineMaterial;
    private SpriteRenderer spriteRenderer;
    private Material defaultMaterial;

    private void Awake() {
        outlineMaterial = Material.Instantiate(outlineMaterial);
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
    }


    public void OnPointerEnter(PointerEventData eventData) {
        spriteRenderer.material = outlineMaterial;
    }

    public void OnPointerExit(PointerEventData eventData) {
        spriteRenderer.material = defaultMaterial;
    }
}
