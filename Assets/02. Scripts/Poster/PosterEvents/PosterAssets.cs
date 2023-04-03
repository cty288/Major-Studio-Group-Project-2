﻿using System;
using System.Collections.Generic;
using _02._Scripts.Poster.PosterContentPanels;
using Crosstales;
using MikroFramework.Singletons;
using UnityEngine;

namespace _02._Scripts.Poster.PosterEvents {
	public class PosterAssets: MonoMikroSingleton<PosterAssets> {
		[field: SerializeField] private List<Sprite> posterSprites;
		[field: SerializeField] private List<GameObject> contentPagePrefabs;
		[field: SerializeField] public List<Sprite> sexyCardMenSprites;
		[field: SerializeField] public List<Sprite> sexyCardWomenSprites;
		[field: SerializeField] public List<int> sexyCardMenHeadPrefabIndices;
		[field: SerializeField] public List<int> sexyCardWomenHeadPrefabIndices;
		[field: SerializeField] public int nakeBodyPrefabIndex;
		[field: SerializeField] public int nakeLegsPrefabIndex;

		public List<Sprite> PosterSprites => posterSprites;


		private void Awake() {
			Sprite firstSprite = posterSprites[0];
			posterSprites.RemoveAt(0);
			posterSprites.CTShuffle();
			posterSprites.Insert(0, firstSprite);
		}

		public Sprite GetPosterSprite(int index) {
			return posterSprites[index];
		}


		public GameObject GetContentPage(Poster poster, Transform transform) {
			if(poster is RawImagePoster rawImagePoster) {
				RawImagePosterContentViewController vc = Instantiate(contentPagePrefabs[0], transform)
					.GetComponent<RawImagePosterContentViewController>();
				
				vc.SetContent(rawImagePoster);
				return vc.gameObject;
			}

			return null;
		}
	}
}