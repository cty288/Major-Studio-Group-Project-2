﻿using System;
using _02._Scripts.Poster.PosterEvents;
using UnityEngine;
using UnityEngine.UI;

namespace _02._Scripts.Poster.PosterContentPanels {
	public class RawImagePosterContentViewController: MonoBehaviour {
		private Image image;
		private void Awake() {
			image = GetComponent<Image>();
		}

		public void SetContent(RawImagePoster content) {
			Awake();
			image.sprite = PosterAssets.Singleton.GetPosterSprite(content.SpriteIndex);
		}
	}
}