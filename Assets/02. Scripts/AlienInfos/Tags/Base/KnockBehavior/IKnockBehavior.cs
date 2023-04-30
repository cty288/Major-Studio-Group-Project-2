using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using MikroFramework.Architecture;
using Random = UnityEngine.Random;

namespace _02._Scripts.AlienInfos.Tags.Base.KnockBehavior {
	public interface IKnockBehavior: IAlienTag {
		public IEnumerator OnKnockDoor(Speaker speaker, IVoiceTag voiceTag, bool isAlien);

		public IKnockBehavior GetOpposite();
		public void OnStopKnock();

		public void OnHitByFlashlight(Speaker speaker, IVoiceTag voiceTag, bool isAlien);
	}

	public abstract class AbstractKnockBehavior : IKnockBehavior, ICanGetModel {
		
		public abstract string TagName { get; protected set; }
		//public abstract string name { get; }
		
		[field: ES3Serializable]
		protected float KnockDoorTimeInterval {get; set; }
		
		[field: ES3Serializable]
		protected float KnockTime {get; set; }
		
		[field: ES3Serializable]
		protected List<string> doorKnockingPhrases {get; set; }
		
		[field: ES3Serializable]
		protected long bodyId {get; set; }
		
		public AbstractKnockBehavior(float knockDoorTimeInterval, float knockTime,
			List<string> overrideDoorKnockingPhrases, string overrideTagName="") {
			KnockDoorTimeInterval = knockDoorTimeInterval;
			KnockTime = knockTime;
			doorKnockingPhrases = overrideDoorKnockingPhrases;
			if(overrideTagName!="") {
				TagName = overrideTagName;
			}
			if(doorKnockingPhrases==null) {
				doorKnockingPhrases = this.GetModel<BodyKnockPhraseModel>().GetPhrases(TagName);
			}
			this.bodyId = bodyId;
		}

		public AbstractKnockBehavior() {
			
		}


		

		public virtual string GetRandomRadioDescription(string alienName,out bool isReal) {
			isReal = Random.Range(0, 2) == 0;
			return GetRandomRadioDescription(alienName, isReal);
		}

		public virtual string GetRandomRadioDescription(string alienName,bool isReal) {
			BodyTagInfoModel bodyTagInfoModel = this.GetModel<BodyTagInfoModel>();
			List<string> targetList;
			if (isReal) {
				targetList = bodyTagInfoModel.GetRealRadioDescription(TagName, alienName);
			}
			else {
				targetList = bodyTagInfoModel.GetRealRadioDescription(TagName, alienName);
			}
			if(targetList==null || targetList.Count==0) {
				return null;
			}
			return targetList[Random.Range(0, targetList.Count)];
		}
		
		public List<string> GetFakeRadioDescription() {
			return null;
		}


		public virtual List<string> GetShortDescriptions() {
			return null;
		}

		public abstract IEnumerator OnKnockDoor(Speaker speaker, IVoiceTag voiceTag, bool isAlien);
		
		//public abstract IEnumerator KnockDoorCoroutine(Speaker speaker, IVoiceTag voiceTag);

		public abstract IKnockBehavior GetOpposite();

		public abstract void OnStopKnock();
		public abstract void OnHitByFlashlight(Speaker speaker, IVoiceTag voiceTag, bool isAlien);

			public IArchitecture GetArchitecture() {
			return MainGame.Interface;
		}
	}
}