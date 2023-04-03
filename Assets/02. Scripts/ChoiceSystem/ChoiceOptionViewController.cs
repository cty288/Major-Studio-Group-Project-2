using System;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02._Scripts.ChoiceSystem {
	public class ChoiceOptionViewController: AbstractMikroController<MainGame> {
		private ChoiceOption choiceOption;
		private Action<ChoiceOption> onClicked;
		private Button button;
		private TMP_Text text;

		private void Awake() {
			button = GetComponent<Button>();
			button.onClick.AddListener(OnClick);
			text = GetComponentInChildren<TMP_Text>(true);
		}

		

		private void OnClick() {
			if (this.choiceOption != null) {
				onClicked?.Invoke(this.choiceOption);
			}
		}

		public void SetChoiceOption(ChoiceOption choiceOption, Action<ChoiceOption> onClicked) {
			Awake();
			this.choiceOption = choiceOption;
			this.onClicked = onClicked;
			text.text = choiceOption.Text;
		}
		
		public void ResetOption() {
			this.choiceOption = null;
			this.onClicked = null;
		}
	}
}