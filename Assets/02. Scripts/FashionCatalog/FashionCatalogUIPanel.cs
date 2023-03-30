using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.FashionCatalog;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BodyPartIndexInfo {
	public BodyPartType BodyPartType;
	public int Index;
	
	public BodyPartIndexInfo(BodyPartType bodyPartType, int index) {
		BodyPartType = bodyPartType;
		Index = index;
	}
}

public class FashionCatalogUIPanel : OpenableUIPanel {
	private GameObject panel;
	private TMP_Text dateText;
	
	private List<MaskableGraphic> images;
	private List<TMP_Text> texts;
	private Dictionary<MaskableGraphic, float> imageAlpha = new Dictionary<MaskableGraphic, float>();

	private Transform photoContainer;
	private FashionCatalogModel fashionCatalogModel;
	
	
	private List<BodyPartIndexInfo> bodyPartIndexInfos = new List<BodyPartIndexInfo>();
	private int bodyPartsPerPage = 12;
	private int currentPage = 0;

	private List<GameObject> spawnedBodies = new List<GameObject>();

	private Button lastPageButton;

	private Button nextPageButton;
	//private List<Transform> fashionElements = new List<Transform>();
	//[SerializeField] private GameObject photoPrefab;

	protected override void Awake() {
		base.Awake();
		images = GetComponentsInChildren<MaskableGraphic>(true).ToList();
		texts = GetComponentsInChildren<TMP_Text>(true).ToList();
		panel = transform.Find("Panel").gameObject;
		//backButton = panel.transform.Find("BackButton").GetComponent<Button>();
		dateText = panel.transform.Find("DateText").GetComponent<TMP_Text>();
		photoContainer = panel.transform.Find("Photos");
		foreach (var image in images) {
			imageAlpha.Add(image, image.color.a);
		}

		fashionCatalogModel = this.GetModel<FashionCatalogModel>();
		Hide(0.5f);
		bodyPartsPerPage = photoContainer.childCount;
		this.RegisterEvent<OnFashionCatalogUIPanelOpened>(OnFashionCatalogUIPanelOpened)
			.UnRegisterWhenGameObjectDestroyed(gameObject);
		lastPageButton = panel.transform.Find("LastPageButton").GetComponent<Button>();
		nextPageButton = panel.transform.Find("NextPageButton").GetComponent<Button>();
		
		lastPageButton.onClick.AddListener(OnLastPage);
		nextPageButton.onClick.AddListener(OnNextPage);
	}

	private void OnNextPage() {
		AddContentToBook(currentPage+1);
	}

	private void OnLastPage() {
		AddContentToBook(currentPage - 1);
	}

	private void OnFashionCatalogUIPanelOpened(OnFashionCatalogUIPanelOpened e) {
		if (e.IsOpen) {
			Show(e.Date, e.Week);
		}
		else {
			//Hide();
		}
	}
	
	private void Show(DateTime date, int week) {
		AudioSystem.Singleton.Play2DSound("pick_up_newspaper");
		dateText.text = $"Week\n{week}";
		SetContent(date);
		AddContentToBook(0);
		Show(0.5f);
		
	}
	
	

	private void SetContent(DateTime date) {
		var info = fashionCatalogModel.GetBodyPartIndicesUpdateInfo(date);
		Dictionary<BodyPartType, HashSet<int>> bodyPartIndices = info.AvailableBodyPartIndices;
		
		List<int> headIndices = bodyPartIndices[BodyPartType.Head].ToList();
		List<int> bodyIndices = bodyPartIndices[BodyPartType.Body].ToList();
		currentPage = 0;
		bodyPartIndexInfos = new List<BodyPartIndexInfo>();
		for (int i = 0; i < headIndices.Count; i++) {
			bodyPartIndexInfos.Add(new BodyPartIndexInfo(BodyPartType.Head, headIndices[i]));
		}
		
		for (int i = 0; i < bodyIndices.Count; i++) {
			bodyPartIndexInfos.Add(new BodyPartIndexInfo(BodyPartType.Body, bodyIndices[i]));
		}
	}


	private void AddContentToBook(int currentPage) {
		foreach (Transform child in photoContainer) {
			child.gameObject.SetActive(false);
		}
		
		int startIndex = currentPage * bodyPartsPerPage;
		int endIndex = Mathf.Min(startIndex + bodyPartsPerPage, bodyPartIndexInfos.Count);
		if(startIndex >= bodyPartIndexInfos.Count) {
			return;
		}

		lastPageButton.gameObject.SetActive(true);
		nextPageButton.gameObject.SetActive(true);
		if (currentPage == 0) {
			lastPageButton.gameObject.SetActive(false);
		}

		if (endIndex == bodyPartIndexInfos.Count) {
			nextPageButton.gameObject.SetActive(false);
		}

		this.currentPage = currentPage;
		
		for (int i = startIndex; i < endIndex; i++) {
			BodyPartIndexInfo bodyPartIndexInfo = bodyPartIndexInfos[i];
			Transform photo = photoContainer.GetChild(i - startIndex);
			photo.gameObject.SetActive(true);
			
			BodyPartPrefabInfo prefabInfo = AlienBodyPartCollections.Singleton.GetBodyPartCollectionByBodyType(bodyPartIndexInfo.BodyPartType, false)
				.HeightSubCollections[0].NewspaperBodyPartDisplays.HumanTraitPartsPrefabs[bodyPartIndexInfo.Index]
				.GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo(0);
			
			BodyInfo bodyInfo = null;
			float scale = 1f;
			if (bodyPartIndexInfo.BodyPartType == BodyPartType.Head) {
				bodyInfo =  BodyInfo.GetBodyInfo(null, null, prefabInfo, HeightType.Tall, null, null,
					BodyPartDisplayType.Newspaper, false);
				scale = 1.5f;
			}else if (bodyPartIndexInfo.BodyPartType == BodyPartType.Body) {
				bodyInfo =  BodyInfo.GetBodyInfo(null, prefabInfo, null, HeightType.Tall, null, null,
					BodyPartDisplayType.Newspaper, false);
			}
			
			
			Transform imageTr = photo.transform.Find("SpawnPos/PhotoImg");
			GameObject spawnedBody = AlienBody.BuildNewspaperAlienBody(bodyInfo, 0, i + 100, 0, scale);
			Camera camera = spawnedBody.GetComponentInChildren<Camera>();
			spawnedBodies.Add(spawnedBody);
			RenderTexture renderTexture = camera.targetTexture;
			
			
			imageTr.GetComponent<RawImage>().texture = renderTexture;
			
			
			photo.transform.Find("Title").GetComponent<TMP_Text>().text = GetShortDescription(prefabInfo);
		}
		
		
	}

	public static string GetShortDescription(BodyPartPrefabInfo prefabInfo) {
        
        for (int i = 0; i < prefabInfo.AllTags.Count; i++) {
	        IAlienTag alienTag = prefabInfo.AllTags[i];
            List<string> shortDescriptions = alienTag.GetShortDescriptions();
            foreach (string shortDescription in shortDescriptions) {
	            if(!String.IsNullOrEmpty(shortDescription)) {
		            return shortDescription;
	            }
            }
        }

        
       
        return "";
    }
	

	public override void OnShow(float time) {
		panel.gameObject.SetActive(true);
		images = GetComponentsInChildren<MaskableGraphic>(true).ToList();
		texts = GetComponentsInChildren<TMP_Text>(true).ToList();
		
		images.ForEach((image => {
			if(imageAlpha.ContainsKey(image)) {
				image.DOFade(imageAlpha[image], time);
			}
			else {
				image.DOFade(1, time);
			}
			
		}));
		texts.ForEach((text => text.DOFade(1, time)));
		
	}

	public override void OnHide(float time) {
		images = GetComponentsInChildren<MaskableGraphic>(true).ToList();
		texts = GetComponentsInChildren<TMP_Text>(true).ToList();
		
		images.ForEach((image => image.DOFade(0, time)));
		texts.ForEach((text => text.DOFade(0, time)));
		
		this.Delay(time, () => {
			if (this) { {
					
					foreach (GameObject spawnedBody in spawnedBodies) {
						Destroy(spawnedBody);
					}
					panel.gameObject.SetActive(false);
				}
			}
		});
	}

	public override void OnDayEnd() {
		Hide(0.5f);
	}
}
