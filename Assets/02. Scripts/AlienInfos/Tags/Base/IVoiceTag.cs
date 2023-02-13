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

		public IVoiceTag GetOpposite();
	}

	public class VoiceTag : IVoiceTag, ICanGetModel {
		
		public int VoiceIndex { get; protected set; }
		public float VoiceSpeed { get; protected set; }
		public Gender VoiceType { get; protected set; }
		public IVoiceTag GetOpposite() {
			int voiceIndex = Random.Range(0, AudioMixerList.Singleton.AlienVoiceGroups.Count);
			float voiceSpeed = 0;
			if (VoiceSpeed > 1.1f) {
				voiceSpeed = Random.Range(0.7f, 0.9f);
			}else if (VoiceSpeed > 0.9f) {
				voiceSpeed = Random.Range(0.9f, 1.1f);
			}
			else {
				voiceSpeed = Random.Range(1.1f, 1.3f);
			}

			Gender voiceType = VoiceType == Gender.MALE ? Gender.FEMALE : Gender.MALE;
			return new VoiceTag(voiceIndex, voiceSpeed, voiceType);
		}

		public VoiceTag(int voiceIndex, float voiceSpeed, Gender voiceType) {
			VoiceIndex = voiceIndex;
			VoiceSpeed = voiceSpeed;
			VoiceType = voiceType;
		}
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
					voiceList.AddRange(new []{"fast speed", "normal speed", "slow speed"});
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
				string.Format(voiceDescription[index], Random.Range(0, 2) == 0 ? "male" : "female");

			string speedDescription = bodyTagInfoModel.GetRealRadioDescription("Voice_Speed")[index];
			string voiceParam = GetVoiceString(index);
			
			
			string voiceDescriptionStringWithSpeed = string.Format(speedDescription, voiceParam);

			var detailDescriptions = bodyTagInfoModel.GetRealRadioDescription($"Voice_Type_{index}");

			string detailDescription = detailDescriptions.Count > 0
				? detailDescriptions[Random.Range(0, detailDescriptions.Count)]
				: "";
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