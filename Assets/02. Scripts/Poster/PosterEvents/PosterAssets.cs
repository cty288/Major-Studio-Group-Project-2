using System.Collections.Generic;
using _02._Scripts.Poster.PosterContentPanels;
using MikroFramework.Singletons;
using UnityEngine;

namespace _02._Scripts.Poster.PosterEvents {
	public class PosterAssets: MonoMikroSingleton<PosterAssets> {
		[field: SerializeField] private List<Sprite> posterSprites;
		[field: SerializeField] private List<GameObject> contentPagePrefabs;

		public List<Sprite> PosterSprites => posterSprites;


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