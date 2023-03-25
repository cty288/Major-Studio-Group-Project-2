using System.Collections.Generic;
using MikroFramework.Singletons;
using UnityEngine;

namespace _02._Scripts.Radio {
	public class RadioContentPlayerFactory : MonoMikroSingleton<RadioContentPlayerFactory> {
		[SerializeField]
		private List<GameObject> RadioContentPlayerPrefabs;


		public GameObject SpawnPlayerPrefabByType(RadioContentType type, Transform parent) {
			int index = (int) type;
			GameObject prefab = RadioContentPlayerPrefabs[index];
			GameObject player = Instantiate(prefab, parent);
			return player;
		}
	}
}