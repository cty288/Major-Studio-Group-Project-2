using System;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;

namespace _02._Scripts.ChoiceSystem {
	public abstract class ChoiceSelectionViewController: AbstractMikroController<MainGame> {
		public abstract ChoiceType ChoiceType { get; }

		protected ChoiceSystem choiceSystem;
		protected ChoiceGroup currentChoiceGroup = null;
		private List<ChoiceOptionViewController> currentViewControllers = new List<ChoiceOptionViewController>();
		protected virtual void Awake() {
			choiceSystem = this.GetSystem<ChoiceSystem>();
			choiceSystem.CurrentChoiceGroup[ChoiceType].RegisterOnValueChaned(OnChoiceGroupChanged)
				.UnRegisterWhenGameObjectDestroyed(gameObject);
		}

		private void OnChoiceGroupChanged(ChoiceGroup oldGroup, ChoiceGroup newGroup) {
			if (newGroup == null) {
				if (currentChoiceGroup != null) {
					DestroyCurrentChoiceGroup();
				} 
				return;
			}
			
			if (newGroup.Type == this.ChoiceType) {
				if (currentChoiceGroup != newGroup && currentChoiceGroup != null) {
					DestroyCurrentChoiceGroup();
				}
				currentChoiceGroup = newGroup;
				CreateChoiceGroup(currentChoiceGroup);
				
			}
		}

		private void CreateChoiceGroup(ChoiceGroup group) {
			currentViewControllers = OnCreateCurrentChoiceGroup();
			for (int i = 0; i < group.Options.Length; i++) {
				ChoiceOption option = group.Options[i];
				ChoiceOptionViewController viewController = currentViewControllers[i];
				viewController.SetChoiceOption(option, OnOptionClicked);
			}
		}

		private void OnOptionClicked(ChoiceOption option) {
			choiceSystem.StopChoiceGroup(ChoiceType);
			option.OnChoiceSelected?.Invoke(option);
			
		}
		
		private void DestroyCurrentChoiceGroup() {
			foreach (ChoiceOptionViewController viewController in currentViewControllers) {
				viewController.ResetOption();
			}
			OnDestroyCurrentChoiceGroup(currentViewControllers);
			currentChoiceGroup = null;
			currentViewControllers.Clear();
		}

		protected abstract void OnDestroyCurrentChoiceGroup(List<ChoiceOptionViewController> currentViewControllers);
		protected abstract List<ChoiceOptionViewController> OnCreateCurrentChoiceGroup();
	}
}