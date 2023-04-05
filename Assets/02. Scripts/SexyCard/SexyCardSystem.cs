﻿using System;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.GameTime;
using _02._Scripts.Poster.PosterEvents;
using Crosstales.RTVoice.Model.Enum;
using MikroFramework.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02._Scripts.SexyCard {
	public class SexyCardSystem: AbstractSystem {
		private SexyCardModel model;
		protected override void OnInit() {
			this.RegisterEvent<OnNewDay>(OnNewDay);
			model = this.GetModel<SexyCardModel>();
		}

		private void OnNewDay(OnNewDay e) {
			if (e.Day == 0) {
				//InitializeModel
				InitializeModel();
			}
		}

		private void InitializeModel() {
			Gender gender = (Gender) Random.Range(0, 2);
			List<Sprite> targetCardSprites = gender == Gender.MALE
				? PosterAssets.Singleton.sexyCardMenSprites
				: PosterAssets.Singleton.sexyCardWomenSprites;
			model.SexyPersonGender = gender;

			model.SexyCardSprite = targetCardSprites[Random.Range(0, targetCardSprites.Count)];

			List<GameObject> headPrefabs = AlienBodyPartCollections.Singleton.HeadBodyPartPrefabs
				.HeightSubCollections[0].ShadowBodyPartPrefabs
				.HumanTraitPartsPrefabs;

			List<int> targetHeadIndices = gender == Gender.MALE
				? PosterAssets.Singleton.sexyCardMenHeadPrefabIndices
				: PosterAssets.Singleton.sexyCardWomenHeadPrefabIndices;

			BodyPartPrefabInfo headPrefabInfo = headPrefabs[targetHeadIndices[Random.Range(0, targetHeadIndices.Count)]]
				.GetComponent<AlienBodyPartInfo>().GetBodyPartPrefabInfo();
			BodyPartPrefabInfo bodyPrefabInfo = AlienBodyPartCollections.Singleton.MainBodyPartPrefabs
				.HeightSubCollections[0].ShadowBodyPartPrefabs
				.HumanTraitPartsPrefabs[PosterAssets.Singleton.nakeBodyPrefabIndex].GetComponent<AlienBodyPartInfo>()
				.GetBodyPartPrefabInfo(0);
			BodyPartPrefabInfo legPrefabInfo = AlienBodyPartCollections.Singleton.LegBodyPartPrefabs
				.HeightSubCollections[0].ShadowBodyPartPrefabs
				.HumanTraitPartsPrefabs[PosterAssets.Singleton.nakeLegsPrefabIndex].GetComponent<AlienBodyPartInfo>()
				.GetBodyPartPrefabInfo();

			BodyInfo bodyInfo = BodyInfo.GetBodyInfo(legPrefabInfo, bodyPrefabInfo, headPrefabInfo, HeightType.Tall,
				new VoiceTag(AudioMixerList.Singleton.SexCardVoiceGroups[(int) gender], 1, gender), new SexyCardKnockBehavior(3,
					5, null),
				BodyPartDisplayType.Shadow, false);

			model.SexyPersonID = bodyInfo.ID;
			this.GetModel<BodyModel>().AddToManagedBodyInfos(bodyInfo);
			model.SexyPersonPhoneNumber = PhoneNumberGenor.GeneratePhoneNumber(7);
			this.GetSystem<TelephoneSystem>().AddContact(model.SexyPersonPhoneNumber, new SexyCardPhoneContact());
			this.GetModel<SuspectModel>().AddSuspect(bodyInfo,
				GoodsInfo.GetGoodsInfo(new BulletGoods(), 2));

			DateTime sexyCardEventDay = this.GetModel<GameTimeModel>().GetDay(Random.Range(7, 15));
			this.GetSystem<GameEventSystem>().AddEvent(new SexyCardDeliverEvent(new TimeRange(sexyCardEventDay)));

		}
	}
}