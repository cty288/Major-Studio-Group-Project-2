using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

public class RewardDeliveryViewController : DraggableItems {
	private PlayerResourceModel playerResourceModel;
	[SerializeField] protected GameObject paperPrefab;

	[ES3Serializable] protected List<GoodsInfo> goodsInfos;
	[ES3Serializable] protected string paperNote;
	[ES3Serializable] protected string noteName;
	protected override void Awake() {
		base.Awake();
		// = this.GetSystem<PlayerResourceSystem>();
		playerResourceModel = this.GetModel<PlayerResourceModel>();
	}

    

	public override void SetLayer(int layer) {
        
	}

	public void SetReward(List<GoodsInfo> goodsInfos, string paperNote, string noteName) {
		this.goodsInfos = goodsInfos;
		this.paperNote = paperNote;
		this.noteName = noteName;
		
	}
	protected override void OnClick() {
		string packageDetailText = "";
		foreach (GoodsInfo goodsInfo in goodsInfos) {
			playerResourceModel.AddResources(goodsInfo);
			packageDetailText += $"{goodsInfo.Count}x {goodsInfo.DisplayName}\n";
		}
		
		DeliveryNoteViewController note = Container.SpawnItem(paperPrefab).GetComponent<DeliveryNoteViewController>();
		note.SetContent($"Package Detail:\n{packageDetailText}\n{paperNote}", noteName);
		GameObject.Destroy(gameObject);
	}

	public override void OnThrownToRubbishBin() {
		Destroy(gameObject);
	}
}
