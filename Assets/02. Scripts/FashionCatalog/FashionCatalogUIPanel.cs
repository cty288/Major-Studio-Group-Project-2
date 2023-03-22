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

public class FashionCatalogUIPanel : OpenableUIPanel {
	private GameObject panel;
	private TMP_Text dateText;
	
	private List<MaskableGraphic> images;
	private List<TMP_Text> texts;
	private Dictionary<MaskableGraphic, float> imageAlpha = new Dictionary<MaskableGraphic, float>();

	private Transform photoContainer;
	private FashionCatalogModel fashionCatalogModel;
	
	private List<GameObject> spawnedBodies = new List<GameObject>();
	
	[SerializeField] private GameObject photoPrefab;

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
		
		this.RegisterEvent<OnFashionCatalogUIPanelOpened>(OnFashionCatalogUIPanelOpened)
			.UnRegisterWhenGameObjectDestroyed(gameObject);
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

		var info = fashionCatalogModel.GetBodyPartIndicesUpdateInfo(date);
		Dictionary<BodyPartType, HashSet<int>> bodyPartIndices = info.AvailableBodyPartIndices;
		
		List<int> headIndices = bodyPartIndices[BodyPartType.Head].ToList();
		List<int> bodyIndices = bodyPartIndices[BodyPartType.Body].ToList();
		for (int i = 0; i < info.BodyPartCount; i++) {
			BodyPartPrefabInfo headInfo = AlienBodyPartCollections.Singleton.GetBodyPartCollectionByBodyType(BodyPartType.Head)
				.HeightSubCollections[0].NewspaperBodyPartDisplays.HumanTraitPartsPrefabs[headIndices[i]]
				.GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo(0);
			
			BodyPartPrefabInfo mainBodyInfo = AlienBodyPartCollections.Singleton.GetBodyPartCollectionByBodyType(BodyPartType.Body)
				.HeightSubCollections[0].NewspaperBodyPartDisplays.HumanTraitPartsPrefabs[bodyIndices[i]]
				.GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo(0);

			BodyInfo bodyInfo = BodyInfo.GetBodyInfo(null, mainBodyInfo, headInfo, HeightType.Tall, null, null,
				BodyPartDisplayType.Newspaper, false);

			GameObject spawnedBody = AlienBody.BuildNewspaperAlienBody(bodyInfo, 0, i+100,1f);
			spawnedBodies.Add(spawnedBody);
			
			Camera camera = spawnedBody.GetComponentInChildren<Camera>();
			RenderTexture renderTexture = camera.targetTexture;

			RawImage rawImage = Instantiate(photoPrefab, photoContainer).GetComponent<RawImage>();
			rawImage.texture = renderTexture;
			rawImage.GetComponentInChildren<TMP_Text>(true).text = GetShortDescription(bodyInfo);
			
			//imageContainers[i].GetComponent<BountyHuntingSelector>().SetHintText(GetShortDescription(bodyInfo));
			
		}
		Show(0.5f);
		
	}
	
	public static string GetShortDescription(BodyInfo bodyInfo) {
        
        List<IAlienTag> tags = new List<IAlienTag>();


        List<BodyPartPrefabInfo> bodyPartPrefabInfos = new List<BodyPartPrefabInfo>();
        bodyPartPrefabInfos.AddRange(bodyInfo.HeadInfoPrefab.GetSubBodyPartInfos());
        bodyPartPrefabInfos.AddRange(bodyInfo.MainBodyInfoPrefab.GetSubBodyPartInfos());
        
        
        
        
        int temp = 0;
        for (int i = 0; i < bodyInfo.HeadInfoPrefab.AllTags.Count; i++) {
            if (temp == 2) {
                break;
            }
            IAlienTag alienTag = bodyInfo.HeadInfoPrefab.AllTags[i];
            List<string> shortDescriptions = alienTag.GetShortDescriptions();
            if (shortDescriptions?.Count > 0) {
                tags.Add(alienTag);
                temp++;
            }
        }

        
        temp = 0;
        
        for (int i = 0; i < bodyInfo.MainBodyInfoPrefab.AllTags.Count; i++) {
            if (temp == 2) {
                break;
            }
            IAlienTag alienTag = bodyInfo.MainBodyInfoPrefab.AllTags[i];
            List<string> shortDescriptions = alienTag.GetShortDescriptions();
            if (shortDescriptions?.Count > 0) {
                tags.Add(alienTag);
                temp++;
            }
        }
        

        StringBuilder stringBuilder = new StringBuilder();
        int addedCount = 0;
        for (int i = 0; i < tags.Count; i++) {
            if (addedCount == 4) {
                break;
            }
            string shortDescription = tags[i].GetShortDescriptions()[0];
            if(!String.IsNullOrEmpty(shortDescription)) {
                if (addedCount == 0) {
                    stringBuilder.Append(shortDescription);
                }
                else {
                    stringBuilder.Append("\n").Append(shortDescription);
                }
                addedCount++;
            }
        }

        return stringBuilder.ToString();
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
		//AudioSystem.Singleton.Play2DSound("put_down_newspaper");
		images.ForEach((image => image.DOFade(0, time)));
		texts.ForEach((text => text.DOFade(0, time)));
		this.Delay(time, () => {
			if (this) { {
					//destroy all children of photoContainer
					foreach (Transform child in photoContainer) {
						Destroy(child.gameObject);
					}

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
