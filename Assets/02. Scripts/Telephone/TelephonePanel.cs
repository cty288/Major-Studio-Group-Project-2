using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using UnityEngine;
using UnityEngine.UI;


public class TelephonePanel : OpenableUIPanel {
    private GameObject panel;
    private Button backButton;
    protected override void Awake() {
        base.Awake();
        panel = transform.Find("Panel").gameObject;
        backButton = panel.transform.Find("BackButton").GetComponent<Button>();
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnBackButtonClicked() {
        Hide();
    }

    public override void Show() {
        panel.gameObject.SetActive(true);
    }

    public override void Hide() {
        panel.gameObject.GetComponent<Animator>().CrossFade("Stop", 0.5f);
        this.Delay(0.5f, () => {
            panel.gameObject.SetActive(false);
        });
    }

    public override void OnDayEnd() {
       Hide();
    }
}
