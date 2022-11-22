using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DraggableItems : AbstractMikroController<MainGame>, IDragHandler, IBeginDragHandler, IEndDragHandler {
    private Vector2 dragStartPos;
    private Bounds tableBounds;
    
   

    public static DraggableItems CurrentDroppingItem;

    public void SetBounds(Bounds bounds) {
        tableBounds = bounds;
    }

    public void OnDrag(PointerEventData eventData) {
        var pos = Camera.main.ScreenToWorldPoint(eventData.position);
        pos.z = 0;
        transform.position = pos;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        dragStartPos = transform.position;
        CurrentDroppingItem = this;
    }

    public void OnEndDrag(PointerEventData eventData) {
        var pos = transform.position;
        if (!tableBounds.Contains(pos))
        {
            transform.DOMove(dragStartPos, 0.5f);
        }

        CurrentDroppingItem = null;
    }

    public abstract  void SetLayer(int layer);
}
