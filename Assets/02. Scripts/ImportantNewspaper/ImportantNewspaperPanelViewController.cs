using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.ImportantNewspaper;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImportantNewspaperPanelViewController : OpenableUIPanel {
	
	private List<MaskableGraphic> images;
	private List<TMP_Text> texts;
	private Dictionary<MaskableGraphic, float> imageAlpha = new Dictionary<MaskableGraphic, float>();
	private GameObject panel;
	private Transform contentPanel;
	private ImportantNewspaperModel model;
	private ImportantNewspaperInfo newsInfo;
	private int currentPage = 0;
	
	protected Button lastPageButton;
	protected Button nextPageButton;

	[ES3Serializable] private Dictionary<int, int> lastOpenedPage = new Dictionary<int, int>();
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
		this.RegisterEvent<OnImportantNewspaperUIPanelOpened>(OnImportantNewspaperUIPanelOpened)
			.UnRegisterWhenGameObjectDestroyed(gameObject);
		model = this.GetModel<ImportantNewspaperModel>();
		lastPageButton = transform.Find("Panel/LastPage").GetComponent<Button>();
		nextPageButton = transform.Find("Panel/NextPage").GetComponent<Button>();
		
		lastPageButton.onClick.AddListener(OnLastPageButtonClicked);
		nextPageButton.onClick.AddListener(OnNextPageButtonClicked);
	}

	private void OnNextPageButtonClicked() {
		if (currentPage < newsInfo.News.Count - 1) {
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

	private void OnImportantNewspaperUIPanelOpened(OnImportantNewspaperUIPanelOpened e) {
		newsInfo = model.GetNewspaperInfo(e.Week);
		currentPage = 0;
		if (lastOpenedPage.ContainsKey(e.Week)) {
			currentPage = lastOpenedPage[e.Week];
		}
		else {
			lastOpenedPage.Add(e.Week, currentPage);
		}
		ShowPage(currentPage);
		Show(e.Week);
	}

	private void ShowPage(int page) {
		if (newsInfo==null || page < 0 || page >= newsInfo.News.Count) {
			return;
		}

		lastOpenedPage[newsInfo.Week] = page;
		DestroyContent();
		ImportantNewspaperPageFactory.Singleton.GetPageObject(newsInfo.News[page], contentPanel, newsInfo.Week);
		lastPageButton.gameObject.SetActive(page > 0);
		nextPageButton.gameObject.SetActive(page < newsInfo.News.Count - 1);
	}

	private void Show(int week) {
		Show(0.5f);
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
		AudioSystem.Singleton.Play2DSound("pick_up_newspaper");
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
