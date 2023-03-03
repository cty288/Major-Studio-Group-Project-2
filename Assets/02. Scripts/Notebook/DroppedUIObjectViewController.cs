using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Notebook;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.EventSystems;



public abstract class DroppedUIObjectViewController : AbstractMikroController<MainGame>, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler {
    public abstract Vector2 GetExtent();
    
    public DroppableInfo DroppableInfo { get; set; }
    public static DroppedUIObjectViewController CurrentDroppingItem;
    public Bounds[] Bounds { get; set; }
    
    
    private Vector2 dragStartPos;
    private DateTime pointerDownTime;

    
    [SerializeField] private bool canDrag = true;

   
    
    private PlayerControlModel playerControlModel;

    protected virtual void Awake() {
        playerControlModel = this.GetModel<PlayerControlModel>();
    }

    public void OnDrag(PointerEventData eventData) {
        if (!canDrag) {
            return;
        }
        if (playerControlModel.ControlType.Value == PlayerControlType.Screenshot) {
            return;
        }
        
        var pos = Camera.main.ScreenToWorldPoint(eventData.position);
        pos.z = 0;
        transform.position = pos;
        Vector3 localPos = transform.localPosition;
        transform.localPosition = new Vector3(localPos.x, localPos.y, 0);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (!canDrag) {
            return;
        }

        if (playerControlModel.ControlType.Value == PlayerControlType.Screenshot) {
            return;
        }
            

        transform.SetAsLastSibling();
        dragStartPos = transform.position;
        CurrentDroppingItem = this;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!canDrag) {
            return;
        }
        if (playerControlModel.ControlType.Value == PlayerControlType.Screenshot) {
            return;
        }
        
        var pos = transform.position;
        pos = new Vector3(pos.x, pos.y, 0);
        this.Delay(0.05f, () => {
            bool contains = false;
            foreach (Bounds bounds in Bounds) {
                Bounds fixedBounds = new Bounds(new Vector3(bounds.center.x, bounds.center.y, 0), bounds.size);
                if (fixedBounds.Contains(pos)) {
                    contains = true;
                }
            }
            if (!contains) {
                transform.DOMove(dragStartPos, 0.5f);
            }
            else {
                DroppableInfo.Bounds = new Bounds(transform.position, GetExtent());
            }
        });
        CurrentDroppingItem = null;
        
    }

    public void OnPointerDown(PointerEventData eventData) {
        pointerDownTime = DateTime.Now;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if ((DateTime.Now - pointerDownTime).TotalSeconds < 0.2f)
        {
            OnClick();
        }
    }
    
    protected abstract void OnClick();
    
}
