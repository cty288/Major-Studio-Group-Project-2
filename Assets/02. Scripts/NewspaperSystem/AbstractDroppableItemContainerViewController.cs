using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.ResKit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class AbstractDroppableItemContainerViewController : AbstractMikroController<MainGame> {
    
    public int CurrentMaxLayer = 1;
    protected Collider2D collider;
    protected ResLoader resLoader;
    protected PlayerResourceSystem playerResourceSystem;

    protected HashSet<Collider2D> spawnedColliders = new HashSet<Collider2D>();

    protected virtual void Awake() {
        collider = GetComponent<Collider2D>();
        resLoader = this.GetUtility<ResLoader>();
        playerResourceSystem = this.GetSystem<PlayerResourceSystem>();
    }

    public void JoinCollider(Collider2D collider2D) {
        spawnedColliders.Add(collider2D);
    }

    public GameObject SpawnItem(GameObject prefab) {
        spawnedColliders.RemoveWhere((c => !c));
        Bounds bounds = collider.bounds;

        //find a place within bounds that is not occupied by other colliders
        Vector2 tryPos = new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));
        int tryCount = 0;
        while (spawnedColliders.Count > 0 && spawnedColliders.Any(c => c.bounds.Contains(tryPos)) && tryCount < 300) {
            tryPos = new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));
            tryCount++;
        }

        //spawn a newspaper on the table within the bounds
        Vector3 position = tryPos;

        DraggableItems obj = Instantiate(prefab, position, Quaternion.identity).GetComponent<DraggableItems>();

        AddItem(obj);
        return obj.gameObject;
    }

    public void AddItem(DraggableItems obj) {
        obj.SetLayer(CurrentMaxLayer++);
        obj.SetBounds(collider.bounds);
        obj.Container = this;
        obj.OnThrownToTrashBin += OnItemThrownToTrashBin;

        if (CurrentMaxLayer > 100) {
            CurrentMaxLayer = 2;
        }

        spawnedColliders.Add(obj.GetComponent<Collider2D>());
        obj.OnAddedToContainer(this);
    }

    private void OnItemThrownToTrashBin(DraggableItems obj) { 
        obj.OnThrownToTrashBin -= OnItemThrownToTrashBin;
        spawnedColliders.Remove(obj.GetComponent<Collider2D>());
    }
}