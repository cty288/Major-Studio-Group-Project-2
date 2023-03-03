using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using MikroFramework.Architecture;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NotebookDragger : AbstractMikroController<MainGame>, IDragHandler, IEndDragHandler {
    private Vector2 initialPosition;
    public Transform Parent;
    public Action OnTear;
    
    private void Awake() {
        
        initialPosition = transform.parent.parent.parent.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Parent.transform.position = eventData.position;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(Vector2.Distance(transform.position, initialPosition)) < 300f) {
            Parent.transform.DOMove(initialPosition, 0.3f);
        }
        else {
            Parent.transform.position = initialPosition;
            OnTear?.Invoke();
        }
    }
}
public class NotebookWritePage : AbstractMikroController<MainGame>, IDragHandler, IEndDragHandler 
{
    private Vector2 initialLocalPosition;
    private Vector2 initialPosition;
    public Action OnTear;

    [SerializeField] private Transform target;
    
    private void Awake() {
        
        initialLocalPosition = target.localPosition;
        initialPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData) {
        Vector2 posWorld = Camera.main.ScreenToWorldPoint(eventData.position);
        target.position = posWorld;
        Vector3 localPos = target.localPosition;
        target.localPosition = new Vector3(localPos.x, localPos.y, 0);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(target.position.y - initialPosition.y) < 2.5f) {
            target.DOLocalMove(initialLocalPosition, 0.3f);
        }
        else {
            target.localPosition = initialLocalPosition;
            OnTear?.Invoke();
        }
    }
}
