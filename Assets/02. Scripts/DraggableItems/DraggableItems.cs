using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DraggableItems : AbstractMikroController<MainGame>, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler{
    private Vector2 dragStartPos;
    [ES3Serializable]
    private Bounds tableBounds;

    private DateTime pointerDownTime;

    public static DraggableItems CurrentDroppingItem;

    public AbstractDroppableItemContainerViewController Container;

    [SerializeField] private bool canDrag = true;

    [ES3NonSerializable]
    public Action<DraggableItems> OnThrownToTrashBin;
    
    private PlayerControlModel playerControlModel;

    protected virtual void Awake() {
        playerControlModel = this.GetModel<PlayerControlModel>();
    }

    protected virtual void Start() {
        if (Container != null) {
            Container.AddItem(this);
        }
    }

    public void SetBounds(Bounds bounds) {
        tableBounds = bounds;
    }

    public void OnDrag(PointerEventData eventData) {
        if (!canDrag) {
            return;
        }
        if (playerControlModel.HasControlType(PlayerControlType.Screenshot)) {
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

        if (playerControlModel.HasControlType(PlayerControlType.Screenshot)) {
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
        if (playerControlModel.HasControlType(PlayerControlType.Screenshot)) {
            return;
        }
        
        var pos = transform.position;
        this.Delay(0.05f, () => {
            if (!tableBounds.Contains(pos) && Container && this) {
                transform.DOMove(dragStartPos, 0.5f);
            }
        });
        

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
    
    public virtual void OnAddedToContainer(AbstractDroppableItemContainerViewController container) {
       
    }

    public void OnThrown() {
        OnThrownToTrashBin?.Invoke(this);
        Container = null;
        OnThrownToRubbishBin();
        
    }

    public abstract void OnThrownToRubbishBin();
}
