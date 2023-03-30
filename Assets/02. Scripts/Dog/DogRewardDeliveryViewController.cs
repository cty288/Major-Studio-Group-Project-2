using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class DogRewardDeliveryViewController : DraggableItems {
	private PlayerResourceModel playerResourceModel;
	[SerializeField] protected GameObject paperPrefab;

	[ES3Serializable] protected int foodCount;
	protected override void Awake() {
		base.Awake();
		// = this.GetSystem<PlayerResourceSystem>();
		playerResourceModel = this.GetModel<PlayerResourceModel>();
	}

    

	public override void SetLayer(int layer) {
        
	}

	public void SetFoodCount(int count) {
		this.foodCount = count;
	}
	protected override void OnClick() {
		playerResourceModel.AddFood(foodCount);
		DeliveryNoteViewController note = Container.SpawnItem(paperPrefab).GetComponent<DeliveryNoteViewController>();
		note.SetContent($"Package Detail:\n{foodCount}x Cans\n\nThanks for finding my little doggy!", "Note From the Owner");
		GameObject.Destroy(gameObject);
	}

	public override void OnThrownToRubbishBin() {
		Destroy(gameObject);
	}
}
