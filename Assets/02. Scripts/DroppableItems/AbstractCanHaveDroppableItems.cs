using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.Notebook;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class AbstractCanHaveDroppableItems : AbstractMikroController<MainGame>, ICanHaveDroppableItems
{ 
	private Bounds bounds;

	RaycastHit2D[] raycastResults = new RaycastHit2D[5];
	private IDroppable lastDroppable;
	protected virtual void Awake() {
		bounds = GetComponent<Collider2D>().bounds;
	}

	private void Update() {
		Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mouseWorldPosition = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0);
		if (bounds.Contains(mouseWorldPosition)) {
			if (Input.GetMouseButton(0)) {
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				var size = Physics2D.RaycastNonAlloc(ray.origin, ray.direction, raycastResults, float.PositiveInfinity);

				if (size > 0) {
					for (int i = 0; i < size; i++) {
						RaycastHit2D hit = raycastResults[i];
						var droppable = hit.collider.GetComponent<IDroppable>();
						if (droppable != null) {
							if (lastDroppable == null || lastDroppable != droppable) {
								OnEnter(droppable);
								lastDroppable = droppable;
								break;
							}
						}
					}
				}
			}
			
			
			if(Input.GetMouseButtonUp(0)) {
				if(lastDroppable!=null) {
					OnDrop(lastDroppable);
					lastDroppable.OnDropped();
				}
				
				lastDroppable = null;
			}
		}
		else {
			if (lastDroppable != null) {
				OnExit(lastDroppable);
				lastDroppable = null;
			}
		}
	}



	public abstract void OnEnter(IDroppable content);

	public abstract void OnExit(IDroppable content);

	public abstract void OnDrop(IDroppable content);
}
