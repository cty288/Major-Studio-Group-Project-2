using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.GameTime;
using _02._Scripts.Notebook;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.ResKit;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewspaperPhotoViewController : AbstractMikroController<MainGame>, IDroppable, IPointerClickHandler {

	public BodyInfo BodyInfo;
	[SerializeField] private Transform notebookViewController;
	[SerializeField] private NotebookPanel notebookPanel;
	[SerializeField] private GameObject flyingPhotoPrefab;
	[SerializeField] private GameObject notebookContentUIPrefab;
	
	private PlayerControlModel playerControlModel;
	private GameTimeModel gameTimeModel;
	private void Awake() {
		playerControlModel = this.GetModel<PlayerControlModel>();
		gameTimeModel = this.GetModel<GameTimeModel>();
	}

	public DroppableInfo GetDroppableInfo() {
		return new DroppableNewspaperBodyInfo(BodyInfo, notebookContentUIPrefab);
	}

	public void OnDropped() {
		
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (!playerControlModel.HasControlType(PlayerControlType.Normal)) {
			return;
		}
		//Vector2 notebookScreenPos = Vector2.zero;

		if (notebookViewController) {
		//	notebookScreenPos = Camera.main.WorldToScreenPoint(notebookViewController.transform.position);
			RawImage spawnedFlyingPhoto = GameObject
				.Instantiate(flyingPhotoPrefab, transform.parent)
				.GetComponent<RawImage>();
			spawnedFlyingPhoto.transform.SetAsLastSibling();
			spawnedFlyingPhoto.texture = GetComponent<RawImage>().texture;
			spawnedFlyingPhoto.transform.DOMove(notebookViewController.transform.position, 0.5f).OnComplete(() => {
				Destroy(spawnedFlyingPhoto.gameObject);
			});
		}

		notebookPanel.AddContent(gameTimeModel.CurrentTime.Value, GetDroppableInfo(), !notebookPanel.IsShow);
	}
}
