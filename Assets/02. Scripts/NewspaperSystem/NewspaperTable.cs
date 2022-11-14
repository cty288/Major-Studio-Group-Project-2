using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewspaperTable : AbstractMikroController<MainGame>, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private GameObject newspaperPrefab;
    private int currentMaxLayer = 1;
    private Collider2D collider;

    private NewspaperViewController todayNewspaper;
    private void Awake() {
        collider = GetComponent<Collider2D>();
        this.RegisterEvent<OnNewspaperGenerated>(OnNewspaperGenerated).UnRegisterWhenGameObjectDestroyed(gameObject);

    }

    private void OnNewspaperGenerated(OnNewspaperGenerated e) {
        
        if (todayNewspaper) {
            todayNewspaper.StopIndicateTodayNewspaper();
        }

        Bounds bounds = collider.bounds;
        //spawn a newspaper on the table within the bounds
        Vector3 position = new Vector3(UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y), 0);
        GameObject newspaper = Instantiate(newspaperPrefab, position, Quaternion.identity);
        todayNewspaper = newspaper.GetComponent<NewspaperViewController>();
        todayNewspaper.StartIndicateTodayNewspaper();
        todayNewspaper.SetLayer(currentMaxLayer++);
        todayNewspaper.SetContent(e.Newspaper, collider.bounds);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Debug.Log("Table Pointer Enter");
    }

    public void OnPointerExit(PointerEventData eventData) {
        Debug.Log("Table Pointer Exit");
    }
}
