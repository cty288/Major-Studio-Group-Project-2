using System.Collections;
using System.Collections.Generic;
using MikroFramework.Singletons;
using UnityEngine;

public class FashionCatalogAssets : MonoMikroSingleton<FashionCatalogAssets> {
	[SerializeField] private List<Sprite> fashionCatalogSprites = new List<Sprite>();
	
	public Sprite GetSprite(int index) {
		return fashionCatalogSprites[index];
	}
	
	public int GetSpriteCount() {
		return fashionCatalogSprites.Count;
	}
}
