using System;
using System.Collections.Generic;
using _02._Scripts.GameTime;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;

namespace _02._Scripts.ChoiceSystem {
	public class ChoiceOption {
		public string Text;
		public Action<ChoiceOption> OnChoiceSelected;
		
		public ChoiceOption(string text, Action<ChoiceOption> onChoiceSelected) {
			Text = text;
			OnChoiceSelected = onChoiceSelected;
		}
	}


	public enum ChoiceType {
		Telephone,
		Outside
	}
	
	public class ChoiceGroup {
		public ChoiceType Type;
		public ChoiceOption[] Options;
		public ChoiceGroup(ChoiceType type, params ChoiceOption[] options) {
			Type = type;
			Options = options;
		}
	}
	
	public class ChoiceSystem: AbstractSystem {
		private PlayerControlModel playerControlModel;
		protected GameTimeModel gameTimeModel;

		private Dictionary<ChoiceType, BindableProperty<ChoiceGroup>> currentChoiceGroup =
			new Dictionary<ChoiceType, BindableProperty<ChoiceGroup>>(){
				{ChoiceType.Telephone, new BindableProperty<ChoiceGroup>()},
				{ChoiceType.Outside, new BindableProperty<ChoiceGroup>()}
			};


		public Dictionary<ChoiceType, BindableProperty<ChoiceGroup>> CurrentChoiceGroup => currentChoiceGroup;

		protected override void OnInit() {
			playerControlModel = this.GetModel<PlayerControlModel>();
		}
		
		public void StartChoiceGroup(ChoiceGroup choiceGroup) {
			if(currentChoiceGroup[choiceGroup.Type].Value==null) {
				this.GetSystem<GameTimeManager>().LockDayEnd.Retain();
			}
			currentChoiceGroup[choiceGroup.Type].Value = choiceGroup;
			playerControlModel.ControlType.Value = PlayerControlType.Choosing;
		}
		
		public void StopChoiceGroup(ChoiceType choiceType) {
			if(currentChoiceGroup[choiceType].Value!=null) {
				this.GetSystem<GameTimeManager>().LockDayEnd.Release();
			}
			currentChoiceGroup[choiceType].Value = null;
			bool isAnyChoiceGroupActive = false;
			foreach (var choiceGroup in currentChoiceGroup) {
				if (choiceGroup.Value.Value != null) {
					isAnyChoiceGroupActive = true;
					break;
				}
			}
			
			if (!isAnyChoiceGroupActive) {
				playerControlModel.ControlType.Value = PlayerControlType.Normal;
			}
		}
		
	}
}