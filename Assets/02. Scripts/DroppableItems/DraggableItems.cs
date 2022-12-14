using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DraggableItems : AbstractMikroController<MainGame>, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler{
    private Vector2 dragStartPos;
    private Bounds tableBounds;

    private DateTime pointerDownTime;

    public static DraggableItems CurrentDroppingItem;

    public AbstractDroppableItemContainerViewController Container;

    [SerializeField] private bool canDrag = true;

    protected virtual void Awake() {
        if (Container != null) {
            Container.JoinCollider(GetComponent<Collider2D>());
        }
    }

    public void SetBounds(Bounds bounds) {
        tableBounds = bounds;
    }

    public void OnDrag(PointerEventData eventData) {
        if (!canDrag) {
            return;
        }
        var pos = Camera.main.ScreenToWorldPoint(eventData.position);
        pos.z = 0;
        transform.position = pos;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (!canDrag) {
            return;
        }

        if (Container) {
            SetLayer(Container.CurrentMaxLayer++);
        }
        dragStartPos = transform.position;
        CurrentDroppingItem = this;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!canDrag) {
            return;
        }
        var pos = transform.position;
        if (!tableBounds.Contains(pos))
        {
            transform.DOMove(dragStartPos, 0.5f);
        }

        CurrentDroppingItem = null;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownTime = DateTime.Now;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if ((DateTime.Now - pointerDownTime).TotalSeconds < 0.2f)
        {
            OnClick();
        }
    }
    public abstract  void SetLayer(int layer);

    protected abstract void OnClick();

    public abstract void OnThrownToRubbishBin();
}
