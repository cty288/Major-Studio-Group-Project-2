using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.EventSystems;

public class Table : AbstractMikroController<MainGame>, IPointerEnterHandler, IPointerExitHandler {
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

        
        todayNewspaper = SpawnItem(newspaperPrefab).GetComponent<NewspaperViewController>();
        todayNewspaper.StartIndicateTodayNewspaper();
        todayNewspaper.SetContent(e.Newspaper);
    }

    public GameObject SpawnItem(GameObject prefab) {
        Bounds bounds = collider.bounds;
        //spawn a newspaper on the table within the bounds
        Vector3 position = new Vector3(UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y), 0);

        DraggableItems obj = Instantiate(prefab, position, Quaternion.identity).GetComponent<DraggableItems>();

        obj.SetLayer(currentMaxLayer++);
        obj.SetBounds(collider.bounds);

        if (currentMaxLayer > 100) {
            currentMaxLayer = 2;
        }

        return obj.gameObject;
    }
    public void OnPointerEnter(PointerEventData eventData) {
        
    }

    public void OnPointerExit(PointerEventData eventData) {
      
    }
}
