using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.SurvivalGuide;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalGuidePanelViewController : OpenableUIPanel
{
	private List<MaskableGraphic> images;
	private List<TMP_Text> texts;
	private Dictionary<MaskableGraphic, float> imageAlpha = new Dictionary<MaskableGraphic, float>();
	private GameObject panel;
	private Transform contentPanel;
	private int currentPage = 0;

	[SerializeField] private List<GameObject> pagePrefabs;
	protected Button lastPageButton;
	protected Button nextPageButton;

	[ES3Serializable] private int lastOpenedPage = 0;

	protected override void Awake() {
		base.Awake();
		images = GetComponentsInChildren<MaskableGraphic>(true).ToList();
		texts = GetComponentsInChildren<TMP_Text>(true).ToList();
		panel = transform.Find("Panel").gameObject;
		foreach (var image in images) {
			imageAlpha.Add(image, image.color.a);
		}
		Hide(0.5f);
		contentPanel = transform.Find("Panel/PageContent");
		
		lastPageButton = transform.Find("Panel/LastPage").GetComponent<Button>();
		nextPageButton = transform.Find("Panel/NextPage").GetComponent<Button>();
		
		lastPageButton.onClick.AddListener(OnLastPageButtonClicked);
		nextPageButton.onClick.AddListener(OnNextPageButtonClicked);

		this.RegisterEvent<OpenSurvivalGuideUIPanel>(OnOpenSurvivalGuideUIPanel)
			.UnRegisterWhenGameObjectDestroyed(gameObject);
		
		this.RegisterEvent<OnNewDay>(OnNewDay)
			.UnRegisterWhenGameObjectDestroyed(gameObject);
	}

	private void OnNewDay(OnNewDay e) {
		if (e.Day == 1) {
			this.GetModel<SurvivalGuideModel>().ReceivedSurvivalGuideBefore.Value = true;
		}
	}

	private void OnOpenSurvivalGuideUIPanel(OpenSurvivalGuideUIPanel e) {
		currentPage = lastOpenedPage;
		Show(0.5f);
		ShowPage(currentPage);
	}

	private void OnNextPageButtonClicked() {
		if (currentPage < pagePrefabs.Count - 1) {
			currentPage++;
			ShowPage(currentPage);
		}
	}

	private void OnLastPageButtonClicked() {
		if (currentPage > 0) {
			currentPage--;
			ShowPage(currentPage);
		}
	}
	
	private void ShowPage(int page) {
		if (pagePrefabs==null || page < 0 || page >= pagePrefabs.Count) {
			return;
		}

		lastOpenedPage = page;
		DestroyContent();

		GameObject pageInstance = Instantiate(pagePrefabs[page], contentPanel);
		lastPageButton.gameObject.SetActive(page > 0);
		nextPageButton.gameObject.SetActive(page < pagePrefabs.Count - 1);
	}
	
	private void DestroyContent() {
		foreach (Transform spawnedBody in contentPanel) {
			Destroy(spawnedBody.gameObject);
		}
	}
	
	public override void OnShow(float time) {
		panel.gameObject.SetActive(true);
		images = GetComponentsInChildren<MaskableGraphic>(true).ToList();
		texts = GetComponentsInChildren<TMP_Text>(true).ToList();
		//AudioSystem.Singleton.Play2DSound("pick_up_newspaper");
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
					DestroyContent();
					panel.gameObject.SetActive(false);
				}
			}
		});
	}

	public override void OnDayEnd() {
		Hide(0.5f);
	}

}
