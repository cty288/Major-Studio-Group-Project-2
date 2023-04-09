using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _02._Scripts.SexyCard;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SexyCardUIPanel : OpenableUIPanel
{
    private List<MaskableGraphic> images;
    private List<TMP_Text> texts;
    private Dictionary<MaskableGraphic, float> imageAlpha = new Dictionary<MaskableGraphic, float>();
    private GameObject panel;
    private Image sexyCardSprite;
    private TMP_Text sexyCardText;

    protected override void Awake() {
        base.Awake();
        images = GetComponentsInChildren<MaskableGraphic>(true).ToList();
        texts = GetComponentsInChildren<TMP_Text>(true).ToList();
        panel = transform.Find("Panel").gameObject;
        sexyCardSprite = panel.transform.Find("SexyCardImage").GetComponent<Image>();
        sexyCardText = panel.transform.Find("PhoneText").GetComponent<TMP_Text>();
        
        foreach (var image in images) {
            imageAlpha.Add(image, image.color.a);
        }
        Hide(0.5f);
        this.RegisterEvent<OnSexyCardUIPanelOpened>(OnSexyCardUIPanelOpened).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnSexyCardUIPanelOpened(OnSexyCardUIPanelOpened e) {
        
        ShowPage();
        Show(0.5f);
    }

    private void ShowPage() {
        SexyCardModel sexyCardModel = this.GetModel<SexyCardModel>();
        sexyCardSprite.sprite = sexyCardModel.SexyCardSprite;
        sexyCardText.text = sexyCardModel.SexyPersonPhoneNumber;
        this.GetModel<TelephoneNumberRecordModel>().AddOrEditRecord(sexyCardModel.SexyPersonPhoneNumber, "Sexy Card");
    }

    public override void OnShow(float time) {
        panel.gameObject.SetActive(true);
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
        images.ForEach((image => image.DOFade(0, time)));
        texts.ForEach((text => text.DOFade(0, time)));
		
		
        this.Delay(time, () => {
            if (this) { {
                    panel.gameObject.SetActive(false);
                }
            }
        });
    }

    public override void OnDayEnd() {
        Hide(0.5f);
    }
}
