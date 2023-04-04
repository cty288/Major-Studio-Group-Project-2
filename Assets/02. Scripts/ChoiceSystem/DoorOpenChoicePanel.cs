using System.Collections.Generic;
using UnityEngine;

namespace _02._Scripts.ChoiceSystem {
	public class DoorOpenChoicePanel: ChoiceSelectionViewController {
		public override ChoiceType ChoiceType { get; } = ChoiceType.Outside;
		[SerializeField] protected GameObject choicePrefab;
		protected Transform panel;
		protected Transform choiceContainer;

		protected override void Awake() {
			base.Awake();
			panel = transform.Find("Panel");
			choiceContainer = panel.Find("BG");
			Hide();
		}
		protected void Hide() {
			panel.gameObject.SetActive(false);
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
	}
}