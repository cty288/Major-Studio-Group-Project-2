using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.EventSystems;

public class DogInRoom : AbstractMikroController<MainGame>, IPointerClickHandler
{
    public SpriteRenderer spriteRenderer;
    public Sprite dogAliveSprite, dogDeadSprite;
    private GameObject dogCanvas;

    //[SerializeField] private OpenableUIPanel panel;

    private void Awake()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        dogCanvas = transform.Find("dogCanvas").gameObject;
        dogCanvas.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Init()
    {
        spriteRenderer.sprite = dogAliveSprite;
        this.GetSystem<DogSystem>().isDogAlive = true;
    }

    public void OnPointerClick(PointerEventData eventData) {
        //panel.Show();
    }

    public void Sacrifice()
    {
        spriteRenderer.sprite = dogDeadSprite;
        this.GetSystem<DogSystem>().isDogAlive = false;
    }
}
