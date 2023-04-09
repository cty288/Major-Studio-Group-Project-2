using System.Collections.Generic;
using _02._Scripts.Dog;
using _02._Scripts.Radio.RadioScheduling;
using MikroFramework.Architecture;
using MikroFramework.ResKit;
using MikroFramework.Singletons;
using UnityEngine;

namespace _02._Scripts.ImportantNewspaper {
	public class ImportantNewspaperPageFactory: MonoMikroSingleton<ImportantNewspaperPageFactory>, ISingleton, ICanGetUtility {
		[SerializeField] protected List<Sprite> imageSprites;

		public GameObject GetPageObject(IImportantNewspaperPageContent content, Transform parent, int weekCount) {
			GameObject prefab = GetPrefabByType(content);
			if (prefab) {
				ImportantNewspaperPageContentUIPanel page = Instantiate(prefab, parent)
					.GetComponent<ImportantNewspaperPageContentUIPanel>();
				page.OnSetContent(content, weekCount);
				return page.gameObject;
			}

			return null;
		}

		public Sprite GetImageSprite(int index) {
			if (index < 0 || index >= imageSprites.Count) {
				return null;
			}

			return imageSprites[index];
		}

		private GameObject GetPrefabByType(IImportantNewspaperPageContent content) {
			ResLoader resLoader = this.GetUtility<ResLoader>();
			if (content is ImportantNewsTextInfo) {
				return resLoader.LoadSync<GameObject>("general", "TextContent");
			}

			if (content is ImportantNewspaperRadioSchedulePage) {
				return resLoader.LoadSync<GameObject>("general", "RadioScheduleContent");
			}

			if (content is MissingDogPoster) {
				return resLoader.LoadSync<GameObject>("general", "MissingDogContent");
			}

			return null;
		}

		public IArchitecture GetArchitecture() {
			return MainGame.Interface;
		}
	}
}