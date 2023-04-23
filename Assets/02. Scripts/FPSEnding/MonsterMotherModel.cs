using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace _02._Scripts.FPSEnding {
	public class MonsterMotherModel : AbstractSavableModel{
		[field: ES3Serializable]
		public BodyTimeInfo MotherBodyTimeInfo { get; private set; }

		[field: ES3Serializable]
		public BindableProperty<bool> isFightingMother { get; private set; } = new BindableProperty<bool>(false);

		public BindableProperty<int> MotherHealth { get; private set; } = new BindableProperty<int>(3);
		
		

		protected override void OnInit() {
			base.OnInit();
			if(MotherBodyTimeInfo == null) {
				MotherBodyTimeInfo = new BodyTimeInfo(9999,
					BodyInfo.GetRandomBodyInfo(BodyPartDisplayType.Shadow, true, Random.Range(0.1f,0.9f),
						new NormalKnockBehavior(4, 6, null, "FPS_Monster"), null, 40),
					true);
				MotherBodyTimeInfo.BodyInfo.VoiceTag.VoiceGroup = AudioMixerList.Singleton.AudioMixerGroups[4];
				MotherBodyTimeInfo.BodyInfo.VoiceTag.VoiceType = Gender.MALE;
			}
		}
	}
}