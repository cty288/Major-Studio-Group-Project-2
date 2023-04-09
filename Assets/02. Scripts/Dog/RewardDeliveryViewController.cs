using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class RewardDeliveryViewController : DraggableItems {
	private PlayerResourceModel playerResourceModel;
	[SerializeField] protected GameObject paperPrefab;

	[ES3Serializable] protected GoodsInfo goodsInfo;
	[ES3Serializable] protected string paperNote;
	[ES3Serializable] protected string noteName;
	protected override void Awake() {
		base.Awake();
		// = this.GetSystem<PlayerResourceSystem>();
		playerResourceModel = this.GetModel<PlayerResourceModel>();
	}

    

	public override void SetLayer(int layer) {
        
	}

	public void SetReward(GoodsInfo goodsInfo, string paperNote, string noteName) {
		this.goodsInfo = goodsInfo;
		this.paperNote = paperNote;
		this.noteName = noteName;
		
	}
	protected override void OnClick() {
		playerResourceModel.AddResources(goodsInfo);
		DeliveryNoteViewController note = Container.SpawnItem(paperPrefab).GetComponent<DeliveryNoteViewController>();
		note.SetContent($"Package Detail:\n{goodsInfo.Count}x {goodsInfo.DisplayName}\n\n{paperNote}", noteName);
		GameObject.Destroy(gameObject);
	}

	public override void OnThrownToRubbishBin() {
		Destroy(gameObject);
	}
}
