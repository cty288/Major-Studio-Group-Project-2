using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;

namespace _02._Scripts.AlienInfos.Tags.Base {
	public interface IVoiceTag: IAlienTag {
		public int VoiceIndex { get; }
		public float VoiceSpeed { get; }
		public Gender VoiceType { get; }
	}

	public abstract class VoiceTag : IVoiceTag, ICanGetModel {
		
		public int VoiceIndex { get; protected set; }
		public float VoiceSpeed { get; protected set; }
		public Gender VoiceType { get; protected set; }
		
		public VoiceTag(int voiceIndex) {
			//get a random voicetype
			VoiceType = (Gender) Random.Range(0, 2);
			VoiceSpeed = Random.Range(0.7f, 1.4f);
			VoiceIndex = voiceIndex;
		}
		
		public string GetRandomRadioDescription(out bool isReal) {
			isReal = Random.Range(0, 2) == 0;
			return isReal ? GetRandomRadioDescription(true) : GetRandomRadioDescription(false);
		}

		protected string GetVoiceString(int index) {
			List<string> voiceList = new List<string>();
			switch (index) {
				case 0:
					voiceList.AddRange(new []{"fast voice", "normal voice", "slow voice"});
					break;
				case 1:
					voiceList.AddRange(new []{"fast", "normal", "slow"});
					break;
			}

			int speedIndex = 0;
			if (VoiceSpeed > 1.1f) {
				speedIndex = 0;
			}
			else if (VoiceSpeed > 0.9f) {
				speedIndex = 1;
			}
			else {
				speedIndex = 2;
			}

			return voiceList[speedIndex];
		}

		public string GetRandomRadioDescription(bool isReal) {
			BodyTagInfoModel bodyTagInfoModel = this.GetModel<BodyTagInfoModel>();
			List<string> voiceDescription = bodyTagInfoModel.GetRealRadioDescription("Voice_Gender");
			int index = Random.Range(0, voiceDescription.Count);
			string voiceDescriptionString =
				string.Format(voiceDescription[index], Random.Range(0, 2) == 0 ? "male " : "female ");

			string speedDescription = bodyTagInfoModel.GetRealRadioDescription("Voice_Speed")[index];
			string voiceParam = GetVoiceString(index);
			
			
			string voiceDescriptionStringWithSpeed = string.Format(speedDescription, voiceParam);

			var detailDescriptions = bodyTagInfoModel.GetRealRadioDescription($"Voice_Type_{index}");
			string detailDescription = detailDescriptions[Random.Range(0, detailDescriptions.Count)];
			return voiceDescriptionString + voiceDescriptionStringWithSpeed + detailDescription;
		}

		public List<string> GetShortDescriptions() {
			return null;
		}


		public IArchitecture GetArchitecture() {
			return MainGame.Interface;
		}
	}
}