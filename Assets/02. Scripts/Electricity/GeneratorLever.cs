using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.EventSystems;

public class GeneratorLever : AbstractMikroController<MainGame>, IPointerEnterHandler, IPointerExitHandler
{
    private PointerEventData cachedEventData;
    private bool isPreparingWorking, isWorking;
    [SerializeField]
    private float z0,z1;
    public float effeciency;

    private void Start()
    {
        isPreparingWorking = false;
        z0 = 0;
        z1 = 0;
    }

    private void Update()
    {
        if ((isPreparingWorking && Input.GetMouseButton(0))|| isWorking)
        {
            SetDraggedRotation(cachedEventData);
        }
        
        if(Input.GetMouseButtonUp(0))
        {
            isWorking = false;
        }
    }

    private void SetDraggedRotation(PointerEventData eventData)
    {        
        z0 = this.transform.eulerAngles.z;
        Vector2 curScreenPosition =
            RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, transform.position);
        Vector2 directionTo = curScreenPosition - eventData.position;
        Vector2 directionFrom = directionTo - eventData.delta;
        this.transform.rotation *= Quaternion.FromToRotation(directionTo, directionFrom);

        float realAngle = Mathf.Abs(z0 - z1);
        if (realAngle > 180) {
            realAngle = 360 - realAngle;
        }

        this.GetSystem<ElectricitySystem>().AddElectricity(effeciency * realAngle * Time.deltaTime);
        Debug.Log(this.GetSystem<ElectricitySystem>().Electricity.Value);
        isWorking = true;
        z1 = z0;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPreparingWorking = true;
        cachedEventData = eventData;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPreparingWorking = false;
    }
}
