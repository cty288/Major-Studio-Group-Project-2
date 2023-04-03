using System.Collections;
using System.Collections.Generic;
using _02._Scripts.ChoiceSystem;
using DG.Tweening;
using MikroFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TelephoneChoicePanel : ChoiceSelectionViewController {
	public override ChoiceType ChoiceType { get; } = ChoiceType.Telephone;
	[SerializeField] protected GameObject choicePrefab;
	protected Transform panel;
	protected Transform choiceContainer;

	protected override void Awake() {
		base.Awake();
		panel = transform.Find("Panel");
		choiceContainer = panel.Find("BG");
		Hide();
	}

	protected override void OnDestroyCurrentChoiceGroup(List<ChoiceOptionViewController> currentViewControllers) {
		Hide();
		foreach (ChoiceOptionViewController viewController in currentViewControllers) {
			Destroy(viewController.gameObject);
		}
	}

	protected override List<ChoiceOptionViewController> OnCreateCurrentChoiceGroup() {
		panel.gameObject.SetActive(true);
		List<ChoiceOptionViewController> choiceOptionViewControllers = new List<ChoiceOptionViewController>();
		foreach (ChoiceOption choiceOption in currentChoiceGroup.Options) {
			GameObject choice = Instantiate(choicePrefab, choiceContainer);
			choice.transform.SetAsLastSibling();
			choiceOptionViewControllers.Add(choice.GetComponent<ChoiceOptionViewController>());
		}

		return choiceOptionViewControllers;
	}

	protected void Hide() {
		panel.gameObject.SetActive(false);
	}
}
