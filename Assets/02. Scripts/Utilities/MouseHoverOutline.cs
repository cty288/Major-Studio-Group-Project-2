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
    private bool isHovering = false;
    [SerializeField] private GameObject followingObject;
    
    private void Awake() {
        outlineMaterial = Material.Instantiate(outlineMaterial);
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
    }

    private void Update() {
        if (isHovering) {
            if (followingObject) {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos = new Vector3(pos.x, pos.y, 0);
                followingObject.transform.position = pos;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        spriteRenderer.material = outlineMaterial;
        isHovering = true;
        if (followingObject) {
            followingObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        spriteRenderer.material = defaultMaterial;
        isHovering = false;
        if (followingObject)
        {
            followingObject.SetActive(false);
        }
    }
}
