﻿using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using MikroFramework.Architecture;
using Random = UnityEngine.Random;

namespace _02._Scripts.AlienInfos.Tags.Base.KnockBehavior {
	public interface IKnockBehavior: IAlienTag {
		public IEnumerator OnKnockDoor(Speaker speaker, IVoiceTag voiceTag);

		public IKnockBehavior GetOpposite();
		public void OnStopKnock();
	}

	public abstract class AbstractKnockBehavior : IKnockBehavior, ICanGetModel {
		
		public abstract string name { get; }
		
		[field: ES3Serializable]
		protected float KnockDoorTimeInterval {get; set; }
		
		[field: ES3Serializable]
		protected float KnockTime {get; set; }
		
		[field: ES3Serializable]
		protected List<string> doorKnockingPhrases {get; set; }
		
		
		public AbstractKnockBehavior(float knockDoorTimeInterval, float knockTime,
			List<string> overrideDoorKnockingPhrases) {
			KnockDoorTimeInterval = knockDoorTimeInterval;
			KnockTime = knockTime;
			doorKnockingPhrases = overrideDoorKnockingPhrases;
			if(doorKnockingPhrases==null) {
				doorKnockingPhrases = this.GetModel<BodyKnockPhraseModel>().GetPhrases(name);
			}
		}
		
		
		public virtual string GetRandomRadioDescription(out bool isReal) {
			isReal = Random.Range(0, 2) == 0;
			return GetRandomRadioDescription(isReal);
		}

		public virtual string GetRandomRadioDescription(bool isReal) {
			BodyTagInfoModel bodyTagInfoModel = this.GetModel<BodyTagInfoModel>();
			List<string> targetList;
			if (isReal) {
				targetList = bodyTagInfoModel.GetRealRadioDescription(name);
			}
			else {
				targetList = bodyTagInfoModel.GetFakeRadioDescription(name);
			}
			if(targetList==null || targetList.Count==0) {
				return null;
			}
			return targetList[Random.Range(0, targetList.Count)];
		}

		public virtual List<string> GetShortDescriptions() {
			return null;
		}

		public abstract IEnumerator OnKnockDoor(Speaker speaker, IVoiceTag voiceTag);
		
		//public abstract IEnumerator KnockDoorCoroutine(Speaker speaker, IVoiceTag voiceTag);

		public abstract IKnockBehavior GetOpposite();

		public abstract void OnStopKnock();

		public IArchitecture GetArchitecture() {
			return MainGame.Interface;
		}
	}
}