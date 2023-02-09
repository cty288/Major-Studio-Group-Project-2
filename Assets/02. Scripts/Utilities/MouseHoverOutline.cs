using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class MouseHoverOutline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private Material outlineMaterial;
    private SpriteRenderer spriteRenderer;
    private MaskableGraphic image;
    
    private Material defaultMaterial;
    private bool isHovering = false;
    [SerializeField] public GameObject followingObject;
    [SerializeField] public bool followingObjectIsUI = false;
    [SerializeField] private bool moveToTopWhenHovering = false;
    private void Awake() {
        if (outlineMaterial) {
            outlineMaterial = Material.Instantiate(outlineMaterial);
        }
       
        spriteRenderer = GetComponent<SpriteRenderer>();
        image = GetComponent<MaskableGraphic>();
        if (spriteRenderer) {
            defaultMaterial = spriteRenderer.material;
        }

        if (image) {
            defaultMaterial = image.material;
        }
        
       
    }

    private void Start() {
        if (spriteRenderer) {
            spriteRenderer.material = defaultMaterial;
        }
       
    }

    private void Update() {
        if (isHovering) {
            if (followingObject) {
                if (!followingObjectIsUI) {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    pos = new Vector3(pos.x, pos.y, 0);
                    followingObject.transform.position = pos;
                }
                else {
                    Vector3 pos = Input.mousePosition;
                    pos = new Vector3(pos.x, pos.y, 0);
                    followingObject.transform.position = pos;
                }
               
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!string.IsNullOrEmpty(SubtitleHightlightedTextDragger.CurrentDraggedText)) {
            StopHovering();
            return;
        }

        StartHovering();
    }

    public void StartHovering(bool onlyChangeMaterial = false) {
        if (outlineMaterial) {
            if (spriteRenderer) {
                spriteRenderer.material = outlineMaterial;
            }
            if (image)
            {
                image.material = outlineMaterial;
            }
        }
       
        isHovering = true;

        if (onlyChangeMaterial) {
            return;
        }
        if (moveToTopWhenHovering) {
            transform.SetAsLastSibling();
        }
        if (followingObject) {
            followingObject.SetActive(true);
        }
    }

    public void StopHovering() {
        if (outlineMaterial) {
            if (spriteRenderer) {
                spriteRenderer.material = defaultMaterial;
            }
            if (image) {
                image.material = defaultMaterial;
            }
        }
        
        isHovering = false;
        if (followingObject)
        {
            followingObject.SetActive(false);
        }
    }
    public void OnPointerExit(PointerEventData eventData) {
        StopHovering();
    }
}
