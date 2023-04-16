using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public class ProloguePortraitViewController : DraggableItems {
	[SerializeField] protected Table table;
	[SerializeField] protected ProloguePhotoPanel photoPanel;
	protected BodyModel bodyModel;
	protected GameTimeModel gameTimeModel;
	protected override void Awake() {
		base.Awake();
		this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
		this.RegisterEvent<OnNewBodyInfoGenerated>(OnNewBodyInfoGenerated).UnRegisterWhenGameObjectDestroyed(gameObject);
		gameObject.SetActive(false);
		bodyModel = this.GetModel<BodyModel>();
		gameTimeModel = this.GetModel<GameTimeModel>();
	}
	

	private void OnNewBodyInfoGenerated(OnNewBodyInfoGenerated e) {
		if (gameTimeModel.Day == 0) {
			BodyInfo info = BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, false, Random.Range(0.5f, 1f),
				new NormalKnockBehavior(3, 7, null), bodyModel.AvailableBodyPartIndices, 40);
			bodyModel.ProloguePlayerBodyInfo = info;
			bodyModel.AddNewBodyTimeInfoToNextDayDeterminedBodiesQueue(new BodyTimeInfo(2, info, true));
		}
	}

	private void OnNewDay(OnNewDay e) {
		if (e.Day == 0) {
			gameObject.SetActive(true);
			table.AddItem(this);
		}
		else {
			OnThrown();
		}
	}

	public override void SetLayer(int layer) {
		
	}

	protected override void OnClick() {
		photoPanel.Show(0.5f);
	}

	public override void OnThrownToRubbishBin() {
		Destroy(this.gameObject);
	}
}
